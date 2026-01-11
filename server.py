#!/usr/bin/env python3
import os
import sys
import json
import socket
import threading
import argparse
from typing import Dict, Any
from dataclasses import dataclass
from datetime import datetime

sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from galchat.utils import _get_now_time as get_now_time
from galchat.agent import Generator


@dataclass
class ServerConfig:
    """服务器配置"""
    host: str = "127.0.0.1"
    port: int = 8888
    max_clients: int = 10
    buffer_size: int = 4096


class OptionsGeneratorServer:
    """选项生成服务器"""

    def __init__(self, config: ServerConfig):
        self.config = config
        self.generator = Generator()
        self.server_socket = None
        self.running = False
        self.client_threads = []

    def start(self):
        """启动服务器"""
        try:
            # 创建socket
            self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

            # 绑定地址
            self.server_socket.bind((self.config.host, self.config.port))
            self.server_socket.listen(self.config.max_clients)

            self.running = True
            print(f"[服务器启动] 监听 {self.config.host}:{self.config.port}")
            print("[服务器启动] 等待客户端连接...")

            # 接受客户端连接
            while self.running:
                try:
                    client_socket, client_address = self.server_socket.accept()
                    print(f"[新连接] 来自 {client_address}")

                    # 为每个客户端创建线程
                    client_thread = threading.Thread(
                        target=self.handle_client,
                        args=(client_socket, client_address)
                    )
                    client_thread.daemon = True
                    client_thread.start()
                    self.client_threads.append(client_thread)

                except KeyboardInterrupt:
                    print("\n[收到关闭信号]")
                    self.stop()
                    break
                except Exception as e:
                    print(f"[接受连接错误] {e}")

        except Exception as e:
            print(f"[服务器启动失败] {e}")
        finally:
            self.cleanup()

    def handle_client(self, client_socket: socket.socket, client_address: tuple):
        """处理客户端请求"""
        try:
            while self.running:
                # 接收数据
                data = client_socket.recv(self.config.buffer_size)
                if not data:
                    print(f"[连接关闭] {client_address}")
                    break

                try:
                    # 解析JSON请求
                    request = json.loads(data.decode('utf-8'))
                    print(f"[收到请求] 来自 {client_address}: {request}")

                    # 处理请求
                    response = self.process_request(request)

                    # 发送响应
                    response_json = json.dumps(response, ensure_ascii=False)
                    client_socket.send(response_json.encode('utf-8'))
                    print(f"[发送响应] 到 {client_address}")

                except json.JSONDecodeError:
                    error_response = {
                        "status": "error",
                        "message": "无效的JSON格式",
                        "data": None
                    }
                    client_socket.send(json.dumps(error_response).encode('utf-8'))
                except Exception as e:
                    error_response = {
                        "status": "error",
                        "message": f"处理请求时出错: {str(e)}",
                        "data": None
                    }
                    client_socket.send(json.dumps(error_response).encode('utf-8'))

        except ConnectionResetError:
            print(f"[连接重置] {client_address}")
        except Exception as e:
            print(f"[客户端处理错误] {client_address}: {e}")
        finally:
            client_socket.close()

    def process_request(self, request: Dict[str, Any]) -> Dict[str, Any]:
        """处理请求并生成选项"""
        try:
            # 验证请求格式
            if "mode" not in request:
                return {
                    "status": "error",
                    "message": "缺少 mode 参数",
                    "data": None
                }

            mode = request.get("mode", 0)

            if mode == 0:  # 纯文本模式
                if "input_str" not in request:
                    return {
                        "status": "error",
                        "message": "纯文本模式需要 input_str 参数",
                        "data": None
                    }

                input_str = request["input_str"]
                local_user = request.get("local_user")
                options = self.generator.str_generate(input_str, local_user=local_user)

                return {
                    "status": "success",
                    "message": "选项生成成功",
                    "data": options.model_dump(),
                    "mode": "text",
                    "timestamp": datetime.now().isoformat()
                }

            else:
                return {
                    "status": "error",
                    "message": f"不支持的 mode 值: {mode} (仅支持 mode 0: 纯文本模式)",
                    "data": None
                }

        except Exception as e:
            return {
                "status": "error",
                "message": f"处理过程中出错: {str(e)}",
                "data": None,
                "timestamp": datetime.now().isoformat()
            }

    def stop(self):
        """停止服务器"""
        self.running = False
        print("[服务器关闭] 正在停止...")

    def cleanup(self):
        """清理资源"""
        if self.server_socket:
            self.server_socket.close()
            print("[服务器关闭] Socket已关闭")

        # 等待所有客户端线程结束
        for thread in self.client_threads:
            if thread.is_alive():
                thread.join(timeout=2)

        print("[服务器关闭] 清理完成")


class OptionsGeneratorClient:
    """选项生成客户端（可选，用于测试）"""

    def __init__(self, host: str = "127.0.0.1", port: int = 8888):
        self.host = host
        self.port = port

    def send_request(self, request: Dict[str, Any]) -> Dict[str, Any]:
        """发送请求到服务器并获取响应"""
        try:
            # 创建socket
            client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            client_socket.settimeout(10)  # 10秒超时

            # 连接到服务器
            client_socket.connect((self.host, self.port))

            # 发送请求
            request_json = json.dumps(request, ensure_ascii=False)
            client_socket.send(request_json.encode('utf-8'))

            # 接收响应
            response_data = client_socket.recv(4096)
            response = json.loads(response_data.decode('utf-8'))

            return response

        except socket.timeout:
            return {"status": "error", "message": "连接超时", "data": None}
        except ConnectionRefusedError:
            return {"status": "error", "message": "无法连接到服务器", "data": None}
        except Exception as e:
            return {"status": "error", "message": f"客户端错误: {str(e)}", "data": None}
        finally:
            if 'client_socket' in locals():
                client_socket.close()


def main():
    """主函数"""
    # 解析命令行参数
    parser = argparse.ArgumentParser(description='选项生成服务器')
    parser.add_argument('--host', type=str, default='127.0.0.1',
                        help='服务器主机地址 (默认: 127.0.0.1)')
    parser.add_argument('--port', type=int, default=8888,
                        help='服务器端口 (默认: 8888)')
    parser.add_argument('--max-clients', type=int, default=10,
                        help='最大客户端连接数 (默认: 10)')

    args = parser.parse_args()

    # 创建服务器配置
    config = ServerConfig(
        host=args.host,
        port=args.port,
        max_clients=args.max_clients
    )

    # 创建并启动服务器
    server = OptionsGeneratorServer(config)

    try:
        server.start()
    except KeyboardInterrupt:
        print("\n[服务器关闭] 用户中断")
        server.stop()
    except Exception as e:
        print(f"[服务器错误] {e}")
        return 1

    return 0


def test_client():
    """测试客户端函数（可选）"""
    # 示例1：纯文本模式
    text_request = {
        "mode": 0,
        "input_str": "今天天气很好，我们出去玩吧！"
    }

    client = OptionsGeneratorClient()

    print("测试纯文本模式:")
    response = client.send_request(text_request)
    print(json.dumps(response, indent=2, ensure_ascii=False))


if __name__ == '__main__':
    # 如果直接运行，启动服务器
    # 如果要测试客户端，可以调用 test_client()
    sys.exit(main())