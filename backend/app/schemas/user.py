from pydantic import BaseModel, EmailStr
from typing import Optional
from datetime import datetime


class UserBase(BaseModel):
    full_name: str
    email: EmailStr
    phone: Optional[str] = None
    address: Optional[str] = None


class UserCreate(UserBase):
    password: str


class UserUpdate(BaseModel):
    full_name: Optional[str] = None
    phone: Optional[str] = None
    address: Optional[str] = None


class UserResponse(UserBase):
    user_id: int
    role: str
    created_at: datetime

    class Config:
        from_attributes = True  # Pydantic v2 syntax


class UserLogin(BaseModel):
    email: EmailStr
    password: str

