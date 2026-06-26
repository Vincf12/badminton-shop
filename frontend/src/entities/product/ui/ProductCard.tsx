import Image from "next/image";
import Link from "next/link";
import { ShoppingBag } from "lucide-react";
import { Badge, Card } from "@/shared/ui";
import type { ProductCardModel } from "@/entities/product";

interface ProductCardProps {
  product: ProductCardModel;
  showBadge?: boolean;
  badgeText?: string;
}

function formatPrice(price: number): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(price);
}

export default function ProductCard({
  product,
  showBadge = true,
  badgeText,
}: ProductCardProps) {
  const inStock = product.stock > 0;

  return (
    <Link href={`/product/${product.id}`} className="group block h-full">
      <Card
        variant="elevated"
        className="flex h-full min-h-[410px] flex-col overflow-hidden transition duration-300 hover:-translate-y-1 hover:border-emerald-200 hover:shadow-[0_24px_70px_rgba(15,23,42,0.12)]"
      >
        <div className="relative m-3 aspect-[4/5] overflow-hidden rounded-2xl bg-slate-100">
          <Image
            src={product.image}
            alt={product.name}
            fill
            sizes="(max-width: 768px) 50vw, 25vw"
            className="object-contain p-5 transition duration-500 group-hover:scale-[1.04]"
          />
          {showBadge ? (
            <Badge variant="primary" className="absolute left-3 top-3 max-w-[calc(100%-1.5rem)] truncate text-xs">
              {badgeText ?? product.category}
            </Badge>
          ) : null}
        </div>

        <div className="flex flex-1 flex-col px-4 pb-4">
          <div className="min-h-[52px]">
            <h3 className="line-clamp-2 text-sm font-black leading-6 tracking-[-0.01em] text-slate-950">
              {product.name}
            </h3>
            {product.brand ? <p className="mt-1 text-xs font-semibold text-slate-500">{product.brand}</p> : null}
          </div>

          <div className="mt-auto flex items-end justify-between gap-3 pt-5">
            <div>
              <p className="text-base font-black text-emerald-700">{formatPrice(product.price)}</p>
              <p className={`mt-1 text-xs font-semibold ${inStock ? "text-slate-500" : "text-red-600"}`}>
                {inStock ? `Còn ${product.stock} sản phẩm` : "Hết hàng"}
              </p>
            </div>
            <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-slate-950 text-white transition group-hover:bg-emerald-600">
              <ShoppingBag className="h-4 w-4" strokeWidth={1.8} />
            </span>
          </div>
        </div>
      </Card>
    </Link>
  );
}
