from pydantic import BaseModel, Field
from datetime import datetime
from typing import Optional, Tuple
from langchain.tools import tool

class Message(BaseModel):
    """聊天消息类"""
    id:int
    info:str
    send_time:datetime
    user_id:str
    group_id:str
    ip_addr:str

class User(BaseModel):
    """用户类"""
    id:str
    name:str
    password:str
    sex:Optional[str]
    profile:Optional[str]

class Group(BaseModel):
    """群聊类"""
    id:str
    name:str
    password:Optional[str]
    user_count:Optional[int]

class ChatOption(BaseModel):
    """对话选项类"""
    Content:str=Field(..., description="推荐用户发送的文本")
    length:int=Field(...,description="该文本的长度")

@tool
def get_text_length(text:str, *args)->int|Tuple[int, ...]:
    """输入单个文本返回该文本的长度，输入一组文本以元组返回文本的长度"""
    if args:
        lengths=[len(text)]
        for t in args:
            lengths.append(len(t))
        return tuple(lengths)
    return len(text)