# Frontend Architecture

Frontend dùng kết hợp Feature-Sliced Design (FSD) và Atomic Design:

- `app/`: route của Next.js, chỉ compose page và gọi public API từ layer bên dưới.
- `widgets/`: khối UI lớn dùng để dựng trang, ví dụ layout, home sections, dashboard data.
- `features/`: flow nghiệp vụ có hành vi người dùng, ví dụ auth, shop filtering, product detail actions.
- `entities/`: domain nghiệp vụ độc lập, ví dụ product, cart, address.
- `shared/`: nền tảng dùng chung thật sự ổn định: UI atoms/molecules, api config/session, utility thuần.

## Dependency Direction

Import chỉ đi từ layer cao xuống layer thấp:

`app -> widgets -> features -> entities -> shared`

Quy ước thực tế:

- Feature không import trực tiếp feature khác. Nếu cần dùng chung, kéo phần đó xuống `entities` hoặc `shared` khi nó thật sự generic.
- Entity không import từ `features`; entity chỉ phụ thuộc `shared` hoặc type nội bộ.
- `shared/` không chứa business logic và không trở thành nơi gom file chưa biết để đâu.
- Mỗi slice expose qua `index.ts`; code bên ngoài slice import từ public API, không chọc sâu vào file nội bộ.

## Atomic UI

Atomic Design nằm trong `shared/ui`:

- `atoms/`: control nhỏ, không biết domain, ví dụ `Button`, `Badge`, `TextInput`.
- `molecules/`: tổ hợp UI nhỏ còn generic, ví dụ `Card`.

Khi component bắt đầu biết product/cart/auth/dashboard, đặt nó vào `entities`, `features`, hoặc `widgets` thay vì `shared/ui`.

## Adding Code

- Thêm model/API/UI thuộc một domain: tạo trong `entities/<domain>`.
- Thêm flow người dùng: tạo trong `features/<feature>`.
- Thêm section/layout ghép nhiều domain: tạo trong `widgets/<widget>`.
- Chỉ thêm abstraction khi đã có lặp lại hoặc ranh giới nghiệp vụ thật sự rõ.
