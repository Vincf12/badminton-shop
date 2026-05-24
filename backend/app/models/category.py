from sqlalchemy import Column, Integer, String, Text
from sqlalchemy.orm import relationship
from app.db.database import Base


class Category(Base):
    __tablename__ = "categories"

    category_id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    category_name = Column(String(100), nullable=False)
    description = Column(Text)

    # Relationships
    products = relationship("Product", back_populates="category")
