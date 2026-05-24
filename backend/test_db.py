#!/usr/bin/env python3
"""
Script để test kết nối database
"""

from app.config_local import settings
from app.db.database import engine
from sqlalchemy import text

def test_database_connection():
    """Test kết nối database"""
    try:
        with engine.connect() as connection:
            result = connection.execute(text("SELECT 1"))
            print("SUCCESS: Database connection successful!")
            print(f"Database URL: {settings.database_url}")
            return True
    except Exception as e:
        print(f"ERROR: Database connection failed: {e}")
        print(f"Database URL: {settings.database_url}")
        return False

if __name__ == "__main__":
    print("Testing database connection...")
    test_database_connection()
