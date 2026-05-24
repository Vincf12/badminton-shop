from sqlalchemy import Column, Integer, TIMESTAMP, ForeignKey, Enum
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from app.db.database import Base


class Payment(Base):
    __tablename__ = "payments"

    payment_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    order_id = Column(Integer, ForeignKey("orders.order_id"))
    payment_method = Column(Enum('cod', 'bank_transfer', 'credit_card', name='payment_method'), nullable=False)
    payment_status = Column(Enum('pending', 'paid', 'failed', name='payment_status'), default='pending')
    payment_date = Column(TIMESTAMP, server_default=func.current_timestamp())

    # Relationships
    order = relationship("Order", back_populates="payments")


