from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.config_local import settings  # Sử dụng config local
from app.db.database import engine
from app.models import *  # Import all models to register them

# Create database tables
# Base.metadata.create_all(bind=engine)

app = FastAPI(
    title="Badminton Shop API",
    description="API for Badminton Shop e-commerce platform",
    version="1.0.0",
    debug=settings.debug
)

# CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5173", "http://localhost:3000"],  # Frontend URLs
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.get("/")
async def root():
    return {"message": "Welcome to Badminton Shop API"}


@app.get("/health")
async def health_check():
    return {"status": "healthy", "environment": settings.environment}


# Include routers
from app.routers import auth
app.include_router(auth.router, prefix="/api/auth", tags=["authentication"])

# Include other routers here when you create them
# app.include_router(products_router, prefix="/api/products", tags=["products"])
# app.include_router(orders_router, prefix="/api/orders", tags=["orders"])


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
