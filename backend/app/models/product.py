from sqlalchemy import Column, Integer, String, Text, DECIMAL, TIMESTAMP, ForeignKey
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from app.db.database import Base


class Product(Base):
    __tablename__ = "products"

    product_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    category_id = Column(Integer, ForeignKey("categories.category_id"))
    product_name = Column(String(150), nullable=False)
    brand = Column(String(100))
    price = Column(DECIMAL(12, 2), nullable=False)
    stock = Column(Integer, default=0)
    image_url = Column(String(255))
    description = Column(Text)
    created_at = Column(TIMESTAMP, server_default=func.current_timestamp())

    # Relationships
    category = relationship("Category", back_populates="products")
    order_details = relationship("OrderDetail", back_populates="product")
    reviews = relationship("Review", back_populates="product")

