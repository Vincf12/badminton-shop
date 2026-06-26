# Badminton Shop Frontend

Frontend cho FlyShot Badminton Shop, xây dựng bằng Next.js App Router, React, TypeScript và Tailwind CSS.

## Scripts

```bash
npm run dev
npm run build
npm run start
npm run lint
```

Mặc định ứng dụng chạy tại `http://localhost:3000` khi dùng `npm run dev`.

## Cấu trúc

```text
frontend/
├── public/
│   └── assets/images/        # Hình ảnh tĩnh
├── src/
│   ├── app/                  # Next.js App Router
│   │   ├── about/page.tsx
│   │   ├── cart/page.tsx
│   │   ├── contact/page.tsx
│   │   ├── dashboard/page.tsx
│   │   ├── login/page.tsx
│   │   ├── product/[id]/page.tsx
│   │   ├── register/page.tsx
│   │   ├── shop/page.tsx
│   │   ├── globals.css
│   │   ├── layout.tsx
│   │   └── page.tsx
│   ├── components/           # Component dùng chung
│   │   ├── auth/
│   │   ├── home/
│   │   ├── layout/
│   │   └── product/
│   ├── contexts/             # React context
│   ├── services/             # API service layer
│   ├── types/                # TypeScript types
│   ├── features/             # Feature modules, dùng khi cần tách logic lớn
│   ├── hooks/                # Custom hooks
│   └── utils/                # Hàm tiện ích
├── next.config.ts
├── package.json
└── tsconfig.json
```

## Quy ước

- Route, layout và page đặt trong `src/app`.
- Component tái sử dụng đặt trong `src/components`, import bằng alias `@/components/...`.
- API call đặt trong `src/services`.
- Type dùng chung đặt trong `src/types`.
- Chỉ thêm `"use client"` cho component hoặc page thật sự cần state, effect, browser API hoặc event handler.

## Biến môi trường

Tạo file `.env` hoặc `.env.local` từ `.env.example` nếu cần cấu hình API:

```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:5211/api
```
