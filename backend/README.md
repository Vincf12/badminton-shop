# Badminton Shop Backend

FastAPI backend cho ứng dụng Badminton Shop e-commerce.

## Cài đặt và chạy

### 1. Cài đặt dependencies

```bash
pip install -r requirements.txt
```

### 2. Cấu hình môi trường

Tạo file `.env` trong thư mục backend với nội dung:

```env
# Database Configuration
DATABASE_URL=mysql+pymysql://app:app123@localhost:3306/badminton_shop
MYSQL_HOST=localhost
MYSQL_PORT=3306
MYSQL_USER=app
MYSQL_PASSWORD=app123
MYSQL_DATABASE=badminton_shop

# Security
SECRET_KEY=your-secret-key-here-change-in-production
ALGORITHM=HS256
ACCESS_TOKEN_EXPIRE_MINUTES=30

# Application
DEBUG=True
ENVIRONMENT=development
```

### 3. Chạy với Docker Compose

```bash
# Từ thư mục gốc của project
docker-compose up backend
```

### 4. Chạy local development

```bash
# Đảm bảo MySQL đang chạy
uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Đăng ký user mới
- `POST /api/auth/login` - Đăng nhập
- `GET /api/auth/me` - Lấy thông tin user hiện tại

### Health Check
- `GET /` - Welcome message
- `GET /health` - Health check

## Database Models

- **User**: Thông tin người dùng
- **Category**: Danh mục sản phẩm
- **Product**: Sản phẩm
- **Order**: Đơn hàng
- **OrderDetail**: Chi tiết đơn hàng
- **Payment**: Thanh toán
- **Shipment**: Giao hàng
- **Review**: Đánh giá sản phẩm

## Cấu trúc thư mục

```
backend/
├── app/
│   ├── config.py          # Cấu hình ứng dụng
│   ├── main.py            # FastAPI app chính
│   ├── db/
│   │   ├── database.py    # Database connection
│   │   └── __init__.py
│   ├── models/            # SQLAlchemy models
│   ├── routers/           # API routers
│   ├── schemas/           # Pydantic schemas
│   ├── services/          # Business logic
│   └── crud/              # Database operations
├── requirements.txt       # Python dependencies
├── Dockerfile            # Docker configuration
└── alembic.ini           # Database migration config
```

## Database Migration

Sử dụng Alembic để quản lý database migrations:

```bash
# Tạo migration mới
alembic revision --autogenerate -m "Description"

# Chạy migrations
alembic upgrade head

# Rollback migration
alembic downgrade -1
```

## API Documentation

Khi server đang chạy, truy cập:
- Swagger UI: http://localhost:8000/docs
- ReDoc: http://localhost:8000/redoc
