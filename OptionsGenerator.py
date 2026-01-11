#!/usr/bin/env python3
import os
import sys

sys.path.append(os.path.dirname(os.path.abspath(__file__)))
import argparse
from galchat.utils import _get_now_time as get_now_time
from galchat.agent import Generator


def main():
    # 解析命令行参数
    parser = argparse.ArgumentParser(description='Generate options from messages history')
    parser.add_argument('--input_str', type=str, required=True, help='输入的文本')
    args = parser.parse_args()
    generator = Generator()
    input_str = args.input_str
    options = generator.str_generate(input_str)
    print(options.model_dump())


if __name__ == '__main__':
    sys.exit(main())
