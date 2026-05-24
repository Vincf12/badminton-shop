from sqlalchemy import Column, Integer, DECIMAL, TIMESTAMP, ForeignKey, Enum
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from app.db.database import Base


class Order(Base):
    __tablename__ = "orders"

    order_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    user_id = Column(Integer, ForeignKey("users.user_id"))
    order_date = Column(TIMESTAMP, server_default=func.current_timestamp())
    status = Column(Enum('pending', 'confirmed', 'shipped', 'delivered', 'cancelled', name='order_status'), default='pending')
    total_amount = Column(DECIMAL(12, 2))

    # Relationships
    user = relationship("User", back_populates="orders")
    order_details = relationship("OrderDetail", back_populates="order")
    payments = relationship("Payment", back_populates="order")
    shipments = relationship("Shipment", back_populates="order")


