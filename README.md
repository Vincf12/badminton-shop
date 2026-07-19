RUN PROJECT
Project structure hiện tại:
```
MyAPI
│
├── Application
│   │
│   ├── Common
│   │   ├── Constants
│   │   ├── Exceptions
│   │   ├── Helpers
│   │   ├── Models
│   │   │   ├── ApiResponse.cs
│   │   │   ├── ServiceResult.cs
│   │   │   └── PagedResult.cs
│   │   └── Mapping
│   │
│   ├── DTOs
│   │
│   ├── Interfaces
│   │   ├── Admin
│   │   ├── Auth
│   │   ├── Catalog
│   │   ├── Customer
│   │   ├── Order
│   │   └── Store
│   │
│   ├── Services
│   │   ├── Admin
│   │   ├── Auth
│   │   ├── Catalog
│   │   ├── Customer
│   │   ├── Order
│   │   └── Store
│   │
│   ├── Validators
│   │
│   └── Mappings
│       ├── ProductProfile.cs
│       ├── OrderProfile.cs
│       ├── UserProfile.cs
│       └── StoreProfile.cs
│
├── Controllers
│
├── Domain
│   ├── Entities
│   ├── Enums
│   ├── Constants
│   ├── Exceptions
│   ├── Events
│   ├── ValueObjects
│   └── Interfaces
│
├── Infrastructure
│   ├── Persistence
│   │   ├── Configurations
│   │   ├── DbContext
│   │   ├── Seed
│   │   └── Repositories
│   │
│   ├── Identity
│   │
│   ├── Authentication
│   │
│   ├── Authorization
│   │
│   ├── Storage
│   │
│   ├── Email
│   │
│   ├── Payments
│   │   ├── PayOS
│   │   └── VNPay
│   │
│   └── DependencyInjection.cs
│
├── Extensions
│
├── Middleware
│
├── Migrations
│
├── Properties
│
├── wwwroot
│
├── Program.cs
└── appsettings.json
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

    mở http://localhost:5173