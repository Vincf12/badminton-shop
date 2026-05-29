RUN PROJECT
Project structure hiện tại:
```
d:\badminton-shop
├─ api/
│  └─ MyAPI/
│     └─ MyAPI/
│        ├─ Controllers/
│        ├─ Program.cs
│        ├─ MyAPI.csproj
│        └─ Properties/
├─ database/
├─ frontend/
└─ docker-compose.yml
```
1. Mở database
    Nếu bạn dùng MySQL local trên máy: chỉ cần đảm bảo service MySQL đang chạy.
    Nếu bạn dùng Docker DB thì chạy:
    docker compose up -d db phpmyadmin
2. Chạy backend
    Mở PowerShell trong thư mục backend C#:
    cd D:\badminton-shop\api\MyAPI\MyAPI
    Chạy server:
    dotnet run
3. Chạy frontend
    Mở terminal mới trong thư mục frontend:
    cd D:\badminton-shop\frontend
    Chạy:
    npm run dev
4. Mở trình duyệt
    Frontend: http://localhost:3000
    Backend: http://localhost:5211
    OpenAPI: https://localhost:7115/openapi/v1.json
    Hoặc HTTP: http://localhost:5211/openapi/v1.json
    Swagger UI: http://localhost:5211/swagger
    Phpmyadmin: http://localhost:8080
    Nếu bạn muốn chạy đúng cổng 5173 cho frontend:

    npm run dev -- --port 5173
    mở http://localhost:5173