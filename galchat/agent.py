import os

try:
    import tomllib
except ImportError:
    import tomli as tomllib
from galchat.utils import OptionList, get_now_time, get_model
from langchain_core.prompts import ChatPromptTemplate

current_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(current_dir, "config.toml")
with open(config_path, "rb") as f:
    config = tomllib.load(f)


class Generator:
    """选项生成器"""

    def __init__(self):
        generator_config = config['Generator']
        self.model = get_model(generator_config["model_name"])
        self.model.bind_tools([get_now_time])
        self.system_prompt = generator_config["system_prompt"]

    def str_generate(self, dialogue: str, local_user: str = None) -> OptionList:
        """输入纯文本，获取OptionList"""
        msg_ls = [
            ("system", self.system_prompt),
            ("human", "请为我提供几个回复的选项，对话记录如下：{history}")
        ]
        if local_user:
            msg_ls.append(
                ("human", f"在对话中，我是{local_user}，现在请为我提供几个回复的选项")
            )
        prompt_template = ChatPromptTemplate.from_messages(msg_ls)
        try:
            chain = prompt_template | self.model.with_structured_output(OptionList)
            result = chain.invoke({"history": dialogue})
            return result
        except Exception as e:
            return OptionList(contents=[], length=0)

    async def astr_generate(self, dialogue: str, local_user: str = None) -> OptionList:
        """异步输入纯文本，获取OptionList"""
        msg_ls = [
            ("system", self.system_prompt),
            ("human", "请为我提供几个回复的选项，对话记录如下：{history}")
        ]
        if local_user:
            msg_ls.append(
                ("human", f"在对话中，我是{local_user}，现在请为我提供几个回复的选项")
            )
        prompt_template = ChatPromptTemplate.from_messages(msg_ls)
        try:
            chain = prompt_template | self.model.with_structured_output(OptionList)
            result = await chain.ainvoke({"history": dialogue})
            return result
        except Exception as e:
            return OptionList(contents=[], length=0)


if __name__ == "__main__":
    generator = Generator()
    res = generator.str_generate("你好")
    print(res.model_dump())
