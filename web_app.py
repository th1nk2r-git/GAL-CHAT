import os
import sys
from typing import Optional, List
from fastapi import FastAPI, HTTPException, Request, WebSocket, WebSocketDisconnect
from fastapi.staticfiles import StaticFiles
from fastapi.responses import HTMLResponse, FileResponse
from pydantic import BaseModel
import json

# 将当前目录添加到路径以便导入 galchat
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from galchat.agent import Generator
from galchat.utils import _get_now_time as get_now_time

app = FastAPI(title="GalChat Web API")

# 聊天室内存存储
class ChatRoom:
    def __init__(self):
        self.messages = []
        self.active_connections: List[WebSocket] = []

    async def connect(self, websocket: WebSocket):
        await websocket.accept()
        self.active_connections.append(websocket)

    def disconnect(self, websocket: WebSocket):
        self.active_connections.remove(websocket)

    async def broadcast(self, message: dict):
        for connection in self.active_connections:
            await connection.send_json(message)

chat_room = ChatRoom()

# 初始化生成器
try:
    generator = Generator()
except Exception as e:
    print(f"初始化 Generator 失败: {e}")
    generator = None

# 请求模型
class ChatRequest(BaseModel):
    mode: int = 0  # 0: 纯文本
    input_str: Optional[str] = None
    max_messages: int = 10
    current_user: Optional[str] = None # 用于生成针对当前用户的建议

@app.post("/api/generate")
async def generate_options(request: ChatRequest, fastapi_request: Request):
    if generator is None:
        raise HTTPException(status_code=500, detail="Generator 未能正确初始化")
    
    # 使用客户端 IP 作为当前用户标识
    client_ip = fastapi_request.client.host if fastapi_request.client else "127.0.0.1"
    
    try:
        if request.mode == 0:
            # 如果没有提供 input_str，则使用聊天室当前记录
            input_text = request.input_str
            if not input_text:
                # 将聊天室记录拼接成文本
                lines = []
                for msg in chat_room.messages[-10:]: # 取最近10条
                    lines.append(f"{msg['user']}: {msg['text']}")
                
                input_text = "\n".join(lines)
            
            if not input_text:
                 return {
                    "status": "success",
                    "data": {"contents": [], "length": 0},
                    "timestamp": get_now_time()
                }

            result = await generator.astr_generate(input_text, local_user=client_ip)
        else:
            raise HTTPException(status_code=400, detail=f"不支持的模式: {request.mode}")
        
        return {
            "status": "success",
            "data": result.model_dump(),
            "timestamp": get_now_time(),
            "your_ip": client_ip
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.websocket("/ws/chat")
async def websocket_endpoint(websocket: WebSocket):
    client_ip = websocket.client.host if websocket.client else "127.0.0.1"
    await chat_room.connect(websocket)
    
    # 告诉客户端它的 IP，方便前端判断“我”
    await websocket.send_json({"type": "init", "your_ip": client_ip})

    # 发送历史消息
    for msg in chat_room.messages:
        await websocket.send_json(msg)
    
    try:
        while True:
            data = await websocket.receive_text()
            message_data = json.loads(data)
            # 强制使用连接的 IP
            message_data["user"] = client_ip
            message_data["type"] = "message"
            message_data["timestamp"] = get_now_time()
            chat_room.messages.append(message_data)
            await chat_room.broadcast(message_data)
    except WebSocketDisconnect:
        chat_room.disconnect(websocket)
    except Exception as e:
        print(f"WebSocket 错误: {e}")
        chat_room.disconnect(websocket)

# 挂载静态文件
os.makedirs("static", exist_ok=True)
app.mount("/static", StaticFiles(directory="static"), name="static")

@app.get("/favicon.ico", include_in_schema=False)
async def favicon():
    return FileResponse("res/favicon.ico")

@app.get("/")
async def get_index():
    with open("static/index.html", "r", encoding="utf-8") as f:
        return HTMLResponse(content=f.read())

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
