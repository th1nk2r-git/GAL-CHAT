#!/usr/bin/env python3
import os
import sys

sys.path.append(os.path.dirname(os.path.abspath(__file__)))
import argparse
from galchat.utils import get_now_time
from galchat.agent import Generator


def main():
    # 解析命令行参数
    parser = argparse.ArgumentParser(description='Generate options from messages history')
    parser.add_argument('--mode', type=int, required=False, help='输入模式，0表示纯文本输入，1表示数据库检索输入',
                        default=0)
    parser.add_argument('--input_str', type=str, required=False, help='输入的文本')
    parser.add_argument('--user_id', type=str, required=False, help='被给予选项的用户的id')
    parser.add_argument('--group_id', type=str, required=False, help='群聊id')
    parser.add_argument('--max_messages', type=int, required=False, help='最大读取的消息条数，默认为1', default=1)
    parser.add_argument('--set_datetime', type=str, required=False, help='从此时间起往回读取历史消息，默认为当前时间',
                        default=get_now_time())
    args = parser.parse_args()
    mode = args.mode
    generator = Generator()
    if mode == 0:
        input_str = args.input_str
        options = generator.str_generate(input_str)
        print(options.model_dump())
    elif mode == 1:
        user_id = args.user_id
        group_id = args.group_id
        max_messages = args.max_messages
        set_datetime = args.set_datetime
        options = generator.sql_generate(user_id, group_id, max_messages, set_datetime)
        print(options.model_dump())


if __name__ == '__main__':
    sys.exit(main())
