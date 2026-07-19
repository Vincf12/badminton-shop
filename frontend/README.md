# FlyShot Badminton Shop Frontend

Frontend cho FlyShot Badminton Shop, xây dựng bằng Next.js App Router, React, TypeScript và Tailwind CSS.

## Công Nghệ

- Next.js 16 App Router
- React 19
- TypeScript
- Tailwind CSS 4
- ESLint
- Lucide React icons

## Chạy Dự Án

```bash
npm install
npm run dev
```

Mặc định ứng dụng chạy tại:

```text
http://localhost:3000
```

Các script chính:

```bash
npm run dev
npm run build
npm run start
npm run lint
```

Trên Windows PowerShell, nếu bị chặn `npm.ps1`, dùng:

```bash
npm.cmd run dev
npm.cmd run lint
npx.cmd tsc --noEmit
```

## Biến Môi Trường

Tạo file `.env.local` từ `.env.example` nếu cần cấu hình API:

```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:5211/api
```

## Kiến Trúc Thư Mục

Dự án đang dùng mô hình lai giữa Feature-Sliced Design và Atomic Design.

```text
frontend/
├── public/
│   └── assets/images/          # Hình ảnh tĩnh
├── src/
│   ├── app/                    # Next.js routing shell
│   ├── views/                  # Page-level composition, nằm giữa app và feature/widget
│   ├── widgets/                # Khối UI lớn, ghép feature/entity/shared
│   ├── features/               # Business actions theo use case
│   ├── entities/               # Domain model, type, API nền
│   └── shared/                 # UI, lib, API config dùng chung
├── next.config.ts
├── package.json
└── tsconfig.json
```

## Vai Trò Từng Layer

### `src/app`

Chỉ giữ chức năng routing của Next.js:

- `layout.tsx`
- `page.tsx`
- route params như `product/[id]/page.tsx`
- `loading.tsx`, `error.tsx`, `not-found.tsx` nếu cần

Không đặt business logic dài, form logic phức tạp hoặc component UI lớn trong `app`.

Ví dụ:

```tsx
import { MainLayout } from "@/widgets/layout";
import { ProductDetailPage } from "@/views/product-detail";
import { fetchProduct } from "@/entities/product";

export default async function ProductRoute({ params }: ProductRouteProps) {
  const { id } = await params;
  const product = await fetchProduct(Number(id));

  return (
    <MainLayout>
      <ProductDetailPage product={product} />
    </MainLayout>
  );
}
```

### `src/views`

Chứa composition ở cấp page. Layer này giúp `src/app` mỏng hơn nhưng vẫn không nhồi mọi thứ vào `features`.

Ví dụ hiện tại:

```text
src/views/product-detail/
├── index.ts
└── ui/ProductDetailPage.tsx
```

### `src/widgets`

Chứa các khối UI lớn xuất hiện ở nhiều route hoặc đại diện cho một vùng màn hình:

```text
src/widgets/layout/
src/widgets/home/
src/widgets/dashboard/
src/widgets/admin/
```

Widget được phép import từ:

- `features`
- `entities`
- `shared`

### `src/features`

Chứa hành vi nghiệp vụ theo use case.

Ví dụ:

```text
src/features/auth/
├── api/authService.ts
├── model/AuthContext.tsx
├── ui/AuthLeftColumn.tsx
└── index.ts

src/features/cart/
├── api/cartService.ts
├── ui/AddToCartButton.tsx
└── index.ts
```

`AddToCartButton` nằm trong `features/cart` vì nó không chỉ render UI, mà còn thực hiện hành động thêm sản phẩm vào giỏ hàng.

### `src/entities`

Chứa domain model, type, mapper và API nền cho một thực thể nghiệp vụ:

```text
src/entities/product/
├── api/productService.ts
├── model/types.ts
├── ui/ProductGrid.tsx
└── index.ts

src/entities/cart/
├── api/cartService.ts
├── model/types.ts
└── index.ts
```

Entity không nên phụ thuộc vào `features`, `widgets` hoặc `views`.

### `src/shared`

Chứa phần dùng chung, không phụ thuộc business domain cụ thể:

```text
src/shared/
├── api/
│   ├── config.ts
│   └── authSession.ts
├── lib/
│   └── cn.ts
└── ui/
    ├── atoms/
    └── molecules/
```

UI dùng chung theo Atomic Design:

```text
src/shared/ui/atoms/Button.tsx
src/shared/ui/atoms/Badge.tsx
src/shared/ui/atoms/TextInput.tsx
src/shared/ui/molecules/Card.tsx
src/shared/ui/molecules/FilterGroup.tsx
src/shared/ui/molecules/ProductCard.tsx
```

`shared/ui` nên là UI thuần: nhận props, render giao diện, không gọi API trực tiếp.

## Public API Với `index.ts`

Mỗi folder chính nên export ra public API qua `index.ts`.

Nên import:

```ts
import { AddToCartButton, cartService } from "@/features/cart";
import { ProductGrid, type ProductDetailModel } from "@/entities/product";
import { Button, ProductCard } from "@/shared/ui";
```

Tránh import sâu:

```ts
import { cartService } from "@/features/cart/api/cartService";
import ProductGrid from "@/entities/product/ui/ProductGrid";
import Button from "@/shared/ui/atoms/Button";
```

Mục tiêu là giữ ranh giới module rõ ràng, dễ refactor mà không làm lan thay đổi import khắp dự án.

## Quy Tắc Phụ Thuộc

```text
app      -> views, widgets, features, entities, shared
views    -> widgets, features, entities, shared
widgets  -> features, entities, shared
features -> entities, shared
entities -> shared
shared   -> không phụ thuộc layer phía trên
```

Không import ngược chiều. Ví dụ `entities/product` không được import từ `features/cart`.

## Quy Ước Code

- Chỉ thêm `"use client"` cho component cần state, effect, browser API hoặc event handler.
- API config chung đặt trong `src/shared/api`.
- Type domain đặt gần entity tương ứng, ví dụ `src/entities/product/model/types.ts`.
- Business action đặt trong feature, ví dụ thêm giỏ hàng nằm trong `src/features/cart`.
- UI thuần, tái sử dụng rộng đặt trong `src/shared/ui`.
- Route trong `src/app` nên mỏng: parse params, fetch data, render view.

## Kiểm Tra Trước Khi Commit

```bash
npm.cmd run lint
npx.cmd tsc --noEmit
```

Nếu dùng shell không bị chặn script, có thể chạy:

```bash
npm run lint
npx tsc --noEmit
```
