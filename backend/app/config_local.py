from pydantic_settings import BaseSettings
from typing import Optional


class LocalSettings(BaseSettings):
    # Database Configuration for Local Development
    database_url: str = "mysql+pymysql://root:012345678mk@localhost:3306/badminton_shop"
    mysql_host: str = "localhost"
    mysql_port: int = 3306
    mysql_user: str = "root"
    mysql_password: str = "012345678mk"
    mysql_database: str = "badminton_shop"
    
    # Security
    secret_key: str = "your-secret-key-here-change-in-production-local-dev"
    algorithm: str = "HS256"
    access_token_expire_minutes: int = 30
    
    # Application
    debug: bool = True
    environment: str = "development"
    
    class Config:
        env_file = ".env.local"
        case_sensitive = False


# Use this for local development
settings = LocalSettings()
