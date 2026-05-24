from sqlalchemy import Column, Integer, String, Text, Enum, TIMESTAMP
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from app.db.database import Base


class User(Base):
    __tablename__ = "users"

    user_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    full_name = Column(String(100), nullable=False)
    email = Column(String(100), unique=True, nullable=False, index=True)
    password_hash = Column(String(255), nullable=False)
    phone = Column(String(20))
    address = Column(Text)
    role = Column(Enum('customer', 'admin', name='user_role'), default='customer')
    created_at = Column(TIMESTAMP, server_default=func.current_timestamp())

    # Relationships
    orders = relationship("Order", back_populates="user")
    reviews = relationship("Review", back_populates="user")
