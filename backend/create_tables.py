#!/usr/bin/env python3
"""
Script để tạo database tables
"""

from app.db.database import engine, Base
from app.models import *  # Import all models

def create_tables():
    """Tạo tất cả tables trong database"""
    try:
        print("Creating database tables...")
        Base.metadata.create_all(bind=engine)
        print("SUCCESS: All tables created successfully!")
        return True
    except Exception as e:
        print(f"ERROR: Failed to create tables: {e}")
        return False

if __name__ == "__main__":
    create_tables()
