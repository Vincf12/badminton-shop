from pydantic_settings import BaseSettings
from typing import Optional


class Settings(BaseSettings):
    # Database Configuration
    database_url: str = "mysql+pymysql://app:app123@db:3306/badminton_shop"
    mysql_host: str = "db"
    mysql_port: int = 3306
    mysql_user: str = "app"
    mysql_password: str = "app123"
    mysql_database: str = "badminton_shop"
    
    # Security
    secret_key: str = "your-secret-key-here-change-in-production"
    algorithm: str = "HS256"
    access_token_expire_minutes: int = 30
    
    # Application
    debug: bool = True
    environment: str = "development"
    
    class Config:
        env_file = ".env"
        case_sensitive = False


settings = Settings()
