from sqlalchemy import Column, Integer, String, TIMESTAMP, ForeignKey, Enum
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from app.db.database import Base


class Shipment(Base):
    __tablename__ = "shipments"

    shipment_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    order_id = Column(Integer, ForeignKey("orders.order_id"))
    tracking_number = Column(String(100))
    courier = Column(String(100))
    shipped_date = Column(TIMESTAMP, nullable=True)
    delivered_date = Column(TIMESTAMP, nullable=True)
    status = Column(Enum('preparing', 'shipping', 'delivered', name='shipment_status'), default='preparing')

    # Relationships
    order = relationship("Order", back_populates="shipments")


