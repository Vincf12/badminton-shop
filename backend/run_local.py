#!/usr/bin/env python3
"""
Script để chạy FastAPI backend local
"""

import uvicorn
from app.config_local import settings

if __name__ == "__main__":
    print("Starting FastAPI backend locally...")
    print(f"Environment: {settings.environment}")
    print(f"Debug mode: {settings.debug}")
    print(f"Database: {settings.mysql_database}")
    
    uvicorn.run(
        "app.main:app",
        host="0.0.0.0",
        port=8000,
        reload=True,  # Auto-reload khi có thay đổi code
        log_level="info"
    )

