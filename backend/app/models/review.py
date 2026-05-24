from sqlalchemy import Column, Integer, Text, TIMESTAMP, ForeignKey, CheckConstraint
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from app.db.database import Base


class Review(Base):
    __tablename__ = "reviews"

    review_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    user_id = Column(Integer, ForeignKey("users.user_id"))
    product_id = Column(Integer, ForeignKey("products.product_id"))
    rating = Column(Integer, CheckConstraint('rating BETWEEN 1 AND 5'))
    comment = Column(Text)
    created_at = Column(TIMESTAMP, server_default=func.current_timestamp())

    # Relationships
    user = relationship("User", back_populates="reviews")
    product = relationship("Product", back_populates="reviews")


