import Image from "next/image";
import Link from "next/link";
import type { ReactNode } from "react";
import { ShoppingBag } from "lucide-react";
import Badge from "../atoms/Badge";
import Card from "./Card";

export interface ProductCardProps {
  id: number | string;
  name: string;
  image: string;
  price: number;
  stock: number;
  href?: string;
  brand?: string;
  category?: string;
  badgeText?: string;
  showBadge?: boolean;
  action?: ReactNode;
}

function formatPrice(price: number): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(price);
}

export default function ProductCard({
  id,
  name,
  image,
  price,
  stock,
  href = `/product/${id}`,
  brand,
  category,
  badgeText,
  showBadge = true,
  action,
}: ProductCardProps) {
  const inStock = stock > 0;
  const label = badgeText ?? category;

  return (
    <Card
      variant="elevated"
      className="group flex h-full min-h-[400px] flex-col overflow-hidden rounded-3xl border border-slate-200 bg-white transition-all duration-300 hover:-translate-y-1 hover:border-emerald-200 hover:shadow-xl"
    >
      <Link href={href} className="block">
        <div className="px-4 pt-4">
          {showBadge && label && (
            <Badge
              variant="primary"
              className="mb-3 inline-flex rounded-full px-3 py-1 text-xs font-medium"
            >
              {label}
            </Badge>
          )}

          <div className="relative aspect-[4/5] overflow-hidden rounded-2xl bg-slate-50">
            <Image
              src={image}
              alt={name}
              fill
              sizes="(max-width:768px) 50vw, 25vw"
              className="object-contain p-0 transition-transform duration-300 group-hover:scale-105"
            />
          </div>
        </div>
      </Link>

      <div className="flex flex-1 flex-col px-4 pb-4">
        <Link href={href} className="min-h-[52px]">
          <h3 className="line-clamp-2 text-base font-semibold leading-6 text-slate-950 transition hover:text-emerald-700">
            {name}
          </h3>

          {brand ? (
            <p className="mt-1 text-sm text-slate-500">{brand}</p>
          ) : null}
        </Link>

        <div className="mt-auto flex items-end justify-between gap-3 pt-4">
          <div>
            <p className="text-[18px] font-bold tracking-tight text-emerald-700">
              {formatPrice(price)}
            </p>
            <p
              className={`mt-1 text-sm ${
                inStock ? "text-slate-500" : "text-red-600"
              }`}
            >
              {inStock ? `Còn ${stock} sản phẩm` : "Hết hàng"}
            </p>
          </div>

          {action ?? (
            <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full border border-slate-200 bg-white text-slate-700 shadow-sm transition-all duration-300 hover:-translate-y-0.5 hover:scale-105 hover:border-emerald-500 hover:bg-emerald-500 hover:text-white active:scale-95">
              <ShoppingBag className="h-5 w-5" strokeWidth={2} />
            </span>
          )}
        </div>
      </div>
    </Card>
  );
}
