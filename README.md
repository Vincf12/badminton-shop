RUN PROJECT
1. Mở database
    Nếu bạn dùng MySQL local trên máy: chỉ cần đảm bảo service MySQL đang chạy.
    Nếu bạn dùng Docker DB thì chạy:
    docker compose up -d db phpmyadmin
2. Chạy backend
    Mở PowerShell trong thư mục backend:
    cd D:\badminton-shop\backend
    Kích hoạt môi trường ảo:
    ..venv\Scripts\Activate.ps1
    Chạy server:
    uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
3. Chạy frontend
    Mở terminal mới trong thư mục frontend:
    cd D:\badminton-shop\frontend
    Chạy:
    npm run dev
4. Mở trình duyệt
    Frontend: http://localhost:3000
    Backend: http://localhost:8000
    Swagger: http://localhost:8000/docs
    Nếu bạn muốn chạy đúng cổng 5173 cho frontend:

    npm run dev -- --port 5173
    mở http://localhost:5173