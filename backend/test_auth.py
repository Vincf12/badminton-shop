#!/usr/bin/env python3
"""
Script để test authentication endpoints
"""

import requests
import json

BASE_URL = "http://localhost:8000"

def test_register():
    """Test đăng ký user mới"""
    url = f"{BASE_URL}/api/auth/register"
    data = {
        "full_name": "Test User",
        "email": "test@example.com",
        "password": "password123",
        "phone": "0123456789",
        "address": "123 Test Street"
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

def test_login():
    """Test đăng nhập"""
    url = f"{BASE_URL}/api/auth/login"
    data = {
        "username": "test@example.com",  # OAuth2PasswordRequestForm uses 'username'
        "password": "password123"
    }
    
    try:
        response = requests.post(url, data=data)
        print(f"Login Status: {response.status_code}")
        if response.status_code == 200:
            print("SUCCESS: User logged in successfully!")
            token_data = response.json()
            print(f"Token: {token_data.get('access_token', 'No token')}")
            return token_data.get('access_token')
        else:
            print(f"ERROR: {response.text}")
        return None
    except Exception as e:
        print(f"ERROR: Failed to login: {e}")
        return None

def test_me(token):
    """Test lấy thông tin user hiện tại"""
    url = f"{BASE_URL}/api/auth/me"
    headers = {"Authorization": f"Bearer {token}"}
    
    try:
        response = requests.get(url, headers=headers)
        print(f"Me Status: {response.status_code}")
        if response.status_code == 200:
            print("SUCCESS: Got user info!")
            print(f"User: {response.json()}")
        else:
            print(f"ERROR: {response.text}")
        return response
    except Exception as e:
        print(f"ERROR: Failed to get user info: {e}")
        return None

if __name__ == "__main__":
    print("Testing Authentication Endpoints...")
    print("=" * 50)
    
    # Test register
    print("1. Testing Register...")
    test_register()
    print()
    
    # Test login
    print("2. Testing Login...")
    token = test_login()
    print()
    
    # Test me endpoint
    if token:
        print("3. Testing Me endpoint...")
        test_me(token)
    else:
        print("3. Skipping Me test - no token available")
