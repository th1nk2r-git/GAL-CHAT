import os
import tomllib
import pymysql
from galchat.utils import get_length, OptionList, get_now_time, get_model
from langchain_core.prompts import ChatPromptTemplate

current_dir = os.path.dirname(os.path.abspath(__file__))
config_path = os.path.join(current_dir, "config.toml")
with open(config_path, "rb") as f:
    config = tomllib.load(f)


class Generator:
    """选项生成器"""

    def __init__(self):
        generator_config = config['Generator']
        mysql_config = config['pymysql']
        self.model = get_model(generator_config["model_name"])
        self.model.bind_tools([get_length])
        self.connection = pymysql.connect(**mysql_config, cursorclass=pymysql.cursors.DictCursor)
        self.system_prompt = generator_config["system_prompt"]

    def str_generate(self, dialogue: str) -> OptionList:
        """输入纯文本，获取OptionList"""
        prompt_template = ChatPromptTemplate.from_messages(
            [
                ("system", self.system_prompt),
                ("human", "请为我提供几个回复的选项，对话记录如下：{history}")
            ]
        )
        try:
            chain = prompt_template | self.model.with_structured_output(OptionList)
            result = chain.invoke({"history": dialogue})
            return result
        except Exception as e:
            return OptionList(contents=[], length=0)

    def sql_generate(self, user_id: str, group_id: str, max_messages: int, set_datetime: str):
        """查询数据库中的聊天消息，获取OptionList"""
        try:
            with self.connection.cursor() as cursor:
                sql = """ \
                      SELECT u.user_name, \
                             m.message_info,
                             m.send_time
                      FROM message_table m \
                               JOIN user_table u ON m.user_id = u.user_id
                      WHERE m.group_id = %s \
                        AND m.send_time <= %s
                      ORDER BY m.send_time DESC LIMIT %s; \
                      """
                cursor.execute(sql, (group_id, set_datetime, max_messages))
                dialogues = cursor.fetchall()
                sql = """ \
                      SELECT u.user_name \
                      FROM message_table m \
                               JOIN user_table u ON m.user_id = u.user_id
                      WHERE u.user_id = %s LIMIT 1; \
                      """
                cursor.execute(sql, (user_id,))
                user_name = cursor.fetchone()["user_name"]
                sql = """ \
                      SELECT g.group_name \
                      FROM message_table m \
                               JOIN group_table g ON g.group_id = m.group_id
                      WHERE g.group_id = %s LIMIT 1; \
                      """
                cursor.execute(sql, (group_id,))
                group_name = cursor.fetchone()["group_name"]
                prompt_template = ChatPromptTemplate.from_messages(
                    [
                        ("system", self.system_prompt),
                        ("human", "我是{user_name}，请为我提供几个回复的选项，群聊{group_name}的对话记录如下：{history}")
                    ]
                )
                dialogue = "\n".join(
                    [f"{line['user_name']}({line['send_time']})：{line['message_info'].replace('\n', ' ')}" for line in
                     dialogues[::-1]])
                # print('='*10,'调试信息：数据库检索', '='*10)
                # print("对话：",dialogue)
                # print('当前用户：',user_name)
                # print('当前群聊：',group_name)
                # print('='*30)
                # return OptionList(contents=[], length=0)
                chain = prompt_template | self.model.with_structured_output(OptionList)
                result = chain.invoke({"user_name": user_name, "group_name": group_name, "history": dialogue})
                return result
        except Exception as e:
            return OptionList(contents=[], length=0)


if __name__ == "__main__":
    generator = Generator()
    res = generator.sql_generate("001", "0001", 10, get_now_time())
    print(res.model_dump())
