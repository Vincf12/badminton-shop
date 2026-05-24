#!/usr/bin/env python3
"""
Script để thiết lập database local cho development
"""

import mysql.connector
from mysql.connector import Error
import os

def create_database():
    """Tạo database và user cho local development"""
    
    # Đọc file SQL schema
    schema_file = "../database/init/badminton_shop.sql"
    seed_file = "../database/init/seed.sql"
    
    try:
        # Kết nối MySQL với root user
        connection = mysql.connector.connect(
            host='localhost',
            user='root',
            password='12345678mk'  # Thay đổi password MySQL của bạn
        )
        
        if connection.is_connected():
            cursor = connection.cursor()
            
            # Tạo database
            print("Creating database...")
            cursor.execute("CREATE DATABASE IF NOT EXISTS badminton_shop CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci")
            cursor.execute("USE badminton_shop")
            
            # Đọc và thực thi schema
            if os.path.exists(schema_file):
                print("Importing schema...")
                with open(schema_file, 'r', encoding='utf-8') as file:
                    schema_sql = file.read()
                    # Thực thi từng câu lệnh
                    for statement in schema_sql.split(';'):
                        if statement.strip():
                            cursor.execute(statement)
            
            # Import seed data nếu có
            if os.path.exists(seed_file):
                print("Importing seed data...")
                with open(seed_file, 'r', encoding='utf-8') as file:
                    seed_sql = file.read()
                    for statement in seed_sql.split(';'):
                        if statement.strip():
                            cursor.execute(statement)
            
            connection.commit()
            print("Database setup completed successfully!")
            
    except Error as e:
        print(f"Error: {e}")
    finally:
        if connection.is_connected():
            cursor.close()
            connection.close()
            print("MySQL connection closed")

if __name__ == "__main__":
    print("Setting up local database...")
    print("Make sure MySQL is running and update the password in this script!")
    create_database()
