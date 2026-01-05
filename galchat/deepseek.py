from langchain_deepseek import ChatDeepSeek
from dotenv import load_dotenv

load_dotenv()

deepseek = ChatDeepSeek(
    model="deepseek-chat",
    temperature=0.2,
    max_tokens=128,
    timeout=10,
    max_retries=2
)

