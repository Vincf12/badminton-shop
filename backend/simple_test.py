#!/usr/bin/env python3
"""
Simple test for register endpoint
"""

import requests
import json

def test_register_simple():
    """Test đăng ký user mới với data đơn giản"""
    url = "http://localhost:8000/api/auth/register"
    data = {
        "full_name": "Simple User",
        "email": "simple@example.com",
        "password": "123456",
        "phone": "0987654321",
        "address": "456 Simple Street"
    }
    
    try:
        response = requests.post(url, json=data)
        print(f"Register Status: {response.status_code}")
        if response.status_code == 200:
            print("SUCCESS: User registered successfully!")
            print(f"Response: {response.json()}")
        else:
            print(f"ERROR: {response.text}")
        return response
    except Exception as e:
        print(f"ERROR: Failed to register user: {e}")
        return None

if __name__ == "__main__":
    print("Testing Simple Register...")
    test_register_simple()
