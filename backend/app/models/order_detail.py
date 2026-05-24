from sqlalchemy import Column, Integer, DECIMAL, ForeignKey
from sqlalchemy.orm import relationship
from app.db.database import Base


class OrderDetail(Base):
    __tablename__ = "order_details"

    order_detail_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    order_id = Column(Integer, ForeignKey("orders.order_id"))
    product_id = Column(Integer, ForeignKey("products.product_id"))
    quantity = Column(Integer, nullable=False)
    price = Column(DECIMAL(12, 2), nullable=False)

    # Relationships
    order = relationship("Order", back_populates="order_details")
    product = relationship("Product", back_populates="order_details")


