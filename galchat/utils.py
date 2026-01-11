from pydantic import BaseModel, Field
from datetime import datetime
from typing import Optional, List, Iterable
from langchain.tools import tool
from galchat.llm import deepseek


def get_model(model_name: str):
    if model_name == "deepseek-chat":
        return deepseek
    # 默认模型
    return deepseek


class Message(BaseModel):
    """聊天消息类"""
    id: int
    info: str
    send_time: datetime
    user_id: str
    group_id: str
    ip_addr: str


class User(BaseModel):
    """用户类"""
    id: str
    name: str
    password: str
    sex: Optional[str]
    profile: Optional[str]


class Group(BaseModel):
    """群聊类"""
    id: str
    name: str
    password: Optional[str]
    user_count: Optional[int]


class ChatOption(BaseModel):
    """对话选项类"""
    content: str = Field(..., description="推荐用户发送的文本")
    emotion: str = Field(..., description="该文本的情感性质")


class OptionList(BaseModel):
    """选项列表类"""
    contents: List[ChatOption] = Field(..., description="选项列表")
    length: int = Field(..., description="列表长度")


@tool
def get_length(*args: Iterable) -> int | tuple[int, ...] | ValueError:
    """输入单个可叠代对象返回该可叠代对象的长度，输入一组可叠代对象则以元组返回各个可叠代对象的长度"""
    try:
        num_items = len(args)
        if num_items == 0:
            raise ValueError
        if num_items == 1:
            return len(args)
        return tuple(len(i) for i in args)
    except ValueError as e:
        return e

def _get_now_time() -> str:
    """获取当前时间"""
    return str(datetime.now())[:19]

@tool
def get_now_time() -> str:
    """获取当前时间"""
    return _get_now_time()
