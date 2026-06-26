"use client";

import { useEffect, useMemo, useState } from "react";
import Image from "next/image";
import {
  Check,
  CheckCircle2,
  Gift,
  Minus,
  Plus,
  ShoppingBag,
  Star,
} from "lucide-react";
import { Button } from "@/shared/ui";
import { cartService } from "@/entities/cart";
import { cn } from "@/shared/lib";
import type { ProductDetailModel, ProductVariantModel } from "@/entities/product";

interface ProductDetailClientProps {
  product: ProductDetailModel;
}

type Review = {
  rating: number;
  comment: string;
  date: string;
};

const STORE_LOCATIONS = [
  "VNB PREMIUM Quận 1",
  "VNB PREMIUM Ninh Kiều Cần Thơ",
  "VNB Quận 3",
  "Tổng Kho VNB",
  "VNB Đống Đa",
  "VNB Tây Hồ",
  "VNB Thanh Trì",
  "VNB Cẩm Lệ Đà Nẵng",
  "VNB Cái Răng Cần Thơ",
  "VNB Thủ Dầu Một",
  "VNB TP Bến Cát",
  "VNB Trảng Bom",
  "VNB Đức Trọng",
  "VNB Quy Nhơn",
  "VNB Rạch Giá",
  "VNB TP Phú Quốc",
];

const CITIES = ["Tất cả tỉnh thành", "Hồ Chí Minh", "Hà Nội", "Đà Nẵng", "Cần Thơ", "Bình Dương"];

export default function ProductDetailClient({ product }: ProductDetailClientProps) {
  const [quantity, setQuantity] = useState(1);
  const [selectedImage, setSelectedImage] = useState(0);
  const [selectedVariantId, setSelectedVariantId] = useState<number | null>(null);
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState("");
  const [cartMessage, setCartMessage] = useState<string | null>(null);
  const [addingToCart, setAddingToCart] = useState(false);
  const [reviews, setReviews] = useState<Review[]>([]);

  const inStockVariants = useMemo(
    () => product.variants.filter((variant) => variant.stock > 0),
    [product.variants]
  );

  const selectedVariant = useMemo(() => {
    if (selectedVariantId !== null) {
      return product.variants.find((variant) => variant.id === selectedVariantId) ?? null;
    }

    return inStockVariants[0] ?? product.variants[0] ?? null;
  }, [inStockVariants, product.variants, selectedVariantId]);

  const images = useMemo(() => {
    const variantImages = product.variants
      .map((variant) => variant.image)
      .filter((image): image is string => Boolean(image));
    const uniqueImages = Array.from(new Set([product.image, ...variantImages]));
    return uniqueImages.length > 0 ? uniqueImages : ["/assets/images/banner-flyshot.png"];
  }, [product]);

  const currentPrice = selectedVariant?.price ?? product.price;
  const currentStock = selectedVariant?.stock ?? product.stock;
  const canAddToCart = Boolean(selectedVariant) && currentStock > 0;
  const productCode = `VNB${String(product.id).padStart(6, "0")}`;
  const listPrice = Math.ceil((currentPrice * 1.2) / 1000) * 1000;

  useEffect(() => {
    setSelectedImage(0);
    setSelectedVariantId(inStockVariants[0]?.id ?? product.variants[0]?.id ?? null);
    setQuantity(1);
    setCartMessage(null);
  }, [inStockVariants, product.id, product.variants]);

  const formatPrice = (price: number) =>
    new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(price);

  const handleQuantityChange = (type: "increase" | "decrease") => {
    setQuantity((previous) => {
      if (type === "decrease") {
        return Math.max(1, previous - 1);
      }

      return Math.min(Math.max(currentStock, 1), previous + 1);
    });
  };

  const handleSubmitReview = () => {
    if (rating === 0 || comment.trim() === "") {
      setCartMessage("Vui lòng chọn số sao và nhập nội dung đánh giá.");
      return;
    }

    setReviews([
      {
        rating,
        comment,
        date: new Date().toLocaleString("vi-VN"),
      },
      ...reviews,
    ]);
    setRating(0);
    setComment("");
  };

  const handleAddToCart = async () => {
    if (!selectedVariant) {
      setCartMessage("Sản phẩm chưa có biến thể để thêm vào giỏ hàng.");
      return;
    }

    setCartMessage(null);
    setAddingToCart(true);

    try {
      await cartService.addItem(selectedVariant.id, quantity);
      setCartMessage("Đã thêm sản phẩm vào giỏ hàng.");
    } catch (addError) {
      setCartMessage(addError instanceof Error ? addError.message : "Không thể thêm sản phẩm vào giỏ hàng.");
    } finally {
      setAddingToCart(false);
    }
  };

  return (
    <div className="bg-white">
      <div className="mx-auto max-w-7xl px-4 py-7 sm:px-6 lg:px-8">
        <div className="grid gap-7 lg:grid-cols-[380px_minmax(0,1fr)_300px] xl:grid-cols-[380px_minmax(0,540px)_300px]">
          <ProductGallery
            images={images}
            productName={product.name}
            selectedImage={selectedImage}
            onSelect={setSelectedImage}
          />

          <main>
            <h1 className="text-[26px] font-semibold leading-tight text-slate-800 sm:text-[28px]">{product.name}</h1>

            <div className="mt-3 flex flex-wrap items-center gap-x-4 gap-y-2 text-sm text-slate-600">
              <span>
                Mã: <span className="font-medium text-orange-600">{productCode}</span>
              </span>
              <span>
                Thương hiệu: <span className="font-medium text-orange-600">{product.brand || "Đang cập nhật"}</span>
              </span>
              <span>
                Tình trạng:{" "}
                <span className={cn("font-medium", currentStock > 0 ? "text-orange-600" : "text-red-600")}>
                  {currentStock > 0 ? "Còn hàng" : "Hết hàng"}
                </span>
              </span>
            </div>

            <div className="mt-4 flex flex-wrap items-end gap-2 border-b border-slate-200 pb-4">
              <span className="text-[23px] font-bold leading-none text-rose-600">{formatPrice(currentPrice)}</span>
              <span className="text-sm text-slate-400">
                Giá niêm yết: <span className="line-through">{formatPrice(listPrice)}</span>
              </span>
            </div>

            <PromotionBox />

            <VariantPicker
              variants={product.variants}
              selectedVariantId={selectedVariant?.id ?? null}
              onSelect={(variant) => {
                setSelectedVariantId(variant.id);
                setQuantity(1);
                if (variant.image) {
                  const imageIndex = images.findIndex((image) => image === variant.image);
                  if (imageIndex >= 0) {
                    setSelectedImage(imageIndex);
                  }
                }
              }}
              formatPrice={formatPrice}
            />

            <div className="mt-5 flex flex-wrap items-center gap-3">
              <div className="flex items-center">
                <button
                  type="button"
                  onClick={() => handleQuantityChange("decrease")}
                  disabled={quantity <= 1}
                  className="grid h-7 w-7 place-items-center rounded-full bg-orange-600 text-white transition hover:bg-orange-700 disabled:cursor-not-allowed disabled:bg-orange-200"
                  aria-label="Giảm số lượng"
                >
                  <Minus className="h-3.5 w-3.5" strokeWidth={2.4} />
                </button>
                <span className="mx-1 grid h-8 w-24 place-items-center rounded border border-orange-500 bg-white text-sm text-orange-600">
                  {quantity}
                </span>
                <button
                  type="button"
                  onClick={() => handleQuantityChange("increase")}
                  disabled={!canAddToCart || quantity >= currentStock}
                  className="grid h-7 w-7 place-items-center rounded-full bg-orange-600 text-white transition hover:bg-orange-700 disabled:cursor-not-allowed disabled:bg-orange-200"
                  aria-label="Tăng số lượng"
                >
                  <Plus className="h-3.5 w-3.5" strokeWidth={2.4} />
                </button>
              </div>
            </div>

            <div className="mt-4 grid gap-3 sm:grid-cols-[168px_250px]">
              <button
                type="button"
                disabled={!canAddToCart}
                className="h-12 rounded bg-amber-400 px-5 text-base font-bold uppercase text-white transition hover:bg-amber-500 disabled:cursor-not-allowed disabled:bg-slate-300"
              >
                Mua ngay
              </button>
              <button
                type="button"
                onClick={handleAddToCart}
                disabled={addingToCart || !canAddToCart}
                className="inline-flex h-12 items-center justify-center gap-2 rounded bg-orange-600 px-5 text-base font-bold uppercase text-white transition hover:bg-orange-700 disabled:cursor-not-allowed disabled:bg-slate-300"
              >
                <ShoppingBag className="h-5 w-5" strokeWidth={2} />
                {addingToCart ? "Đang thêm..." : "Thêm vào giỏ hàng"}
              </button>
            </div>

            {cartMessage ? (
              <p
                className={cn(
                  "mt-4 rounded border px-4 py-3 text-sm font-semibold",
                  cartMessage.startsWith("Đã")
                    ? "border-emerald-200 bg-emerald-50 text-emerald-700"
                    : "border-red-200 bg-red-50 text-red-700"
                )}
              >
                {cartMessage}
              </p>
            ) : null}
          </main>

          <StockPanel />
        </div>

        <section className="mt-10 grid gap-6 lg:grid-cols-[1fr_340px]">
          <div className="rounded border border-slate-200 bg-white p-6">
            <h2 className="text-xl font-bold text-slate-900">Mô tả sản phẩm</h2>
            <p className="mt-4 max-w-3xl text-base leading-8 text-slate-600">
              {product.description ||
                "Sản phẩm này chưa có mô tả chi tiết. Bạn có thể liên hệ FlyShot để được tư vấn cấu hình phù hợp."}
            </p>
          </div>

          <div className="rounded border border-slate-200 bg-white p-6">
            <h2 className="text-xl font-bold text-slate-900">Thông tin nhanh</h2>
            <div className="mt-5 space-y-3">
              {[
                ["Danh mục", product.category],
                ["Thương hiệu", product.brand || "Đang cập nhật"],
                ["Tồn kho", `${product.stock}`],
                [
                  "Cập nhật",
                  product.updatedAt
                    ? new Date(product.updatedAt).toLocaleDateString("vi-VN")
                    : new Date(product.createdAt).toLocaleDateString("vi-VN"),
                ],
              ].map(([label, value]) => (
                <div key={label} className="flex items-center justify-between gap-4 border-t border-slate-100 pt-3 text-sm">
                  <span className="text-slate-500">{label}</span>
                  <span className="text-right font-bold text-slate-950">{value}</span>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="mt-6 rounded border border-slate-200 bg-white p-6">
          <div className="grid gap-8 lg:grid-cols-[340px_1fr]">
            <div>
              <h2 className="text-xl font-bold text-slate-900">Đánh giá sản phẩm</h2>
              <p className="mt-2 text-sm leading-6 text-slate-600">
                Chia sẻ cảm nhận sau khi dùng sản phẩm để người chơi khác chọn dễ hơn.
              </p>

              <div className="mt-5 flex items-center gap-1">
                {[1, 2, 3, 4, 5].map((star) => (
                  <button key={star} type="button" onClick={() => setRating(star)} aria-label={`Chọn ${star} sao`}>
                    <Star
                      className={cn(
                        "h-7 w-7 transition",
                        rating >= star ? "fill-amber-400 text-amber-400" : "text-slate-300 hover:text-amber-300"
                      )}
                      strokeWidth={1.6}
                    />
                  </button>
                ))}
              </div>

              <textarea
                value={comment}
                onChange={(event) => setComment(event.target.value)}
                placeholder="Nhập nhận xét của bạn"
                className="mt-4 min-h-32 w-full rounded border border-slate-200 bg-white p-4 text-sm text-slate-800 outline-none transition focus:border-orange-500 focus:ring-4 focus:ring-orange-100"
              />

              <Button onClick={handleSubmitReview} className="mt-4 bg-orange-600 hover:bg-orange-700">
                Gửi đánh giá
              </Button>
            </div>

            <div className="space-y-3">
              {reviews.length === 0 ? (
                <div className="flex min-h-48 items-center justify-center rounded border border-dashed border-slate-300 bg-slate-50 p-6 text-center">
                  <div>
                    <CheckCircle2 className="mx-auto h-8 w-8 text-emerald-600" strokeWidth={1.8} />
                    <p className="mt-3 text-sm font-bold text-slate-950">Chưa có đánh giá nào</p>
                    <p className="mt-1 text-sm text-slate-500">Hãy là người đầu tiên chia sẻ trải nghiệm.</p>
                  </div>
                </div>
              ) : (
                reviews.map((item, index) => (
                  <div key={`${item.date}-${index}`} className="rounded border border-slate-200 bg-white p-4">
                    <div className="mb-2 flex items-center gap-1 text-amber-400">
                      {Array.from({ length: item.rating }).map((_, starIndex) => (
                        <Star key={starIndex} className="h-4 w-4 fill-current" strokeWidth={1.6} />
                      ))}
                    </div>
                    <p className="text-sm leading-6 text-slate-700">{item.comment}</p>
                    <p className="mt-2 text-xs font-semibold text-slate-400">{item.date}</p>
                  </div>
                ))
              )}
            </div>
          </div>
        </section>
      </div>
    </div>
  );
}

function ProductGallery({
  images,
  productName,
  selectedImage,
  onSelect,
}: {
  images: string[];
  productName: string;
  selectedImage: number;
  onSelect: (index: number) => void;
}) {
  return (
    <section className="grid gap-5 sm:grid-cols-[72px_1fr] lg:grid-cols-1">
      <div className="relative min-h-[360px] bg-white sm:order-2 lg:order-none lg:min-h-[430px]">
        <Image
          src={images[selectedImage]}
          alt={productName}
          fill
          sizes="(max-width: 1024px) 100vw, 380px"
          className="object-contain p-4"
          priority
        />
      </div>

      <div className="flex gap-3 overflow-x-auto sm:order-1 sm:flex-col lg:order-none lg:flex-row">
        {images.map((image, index) => (
          <button
            key={`${image}-${index}`}
            type="button"
            onClick={() => onSelect(index)}
            className={cn(
              "relative h-[88px] w-[68px] shrink-0 border bg-white transition",
              selectedImage === index ? "border-orange-500" : "border-slate-200 hover:border-orange-300"
            )}
            aria-label={`Xem ảnh sản phẩm ${index + 1}`}
          >
            <Image src={image} alt={`${productName} ${index + 1}`} fill className="object-contain p-2" />
          </button>
        ))}
      </div>
    </section>
  );
}

function PromotionBox() {
  return (
    <section className="relative mt-6 rounded border border-dotted border-orange-500 px-4 pb-4 pt-7">
      <div className="absolute -top-[15px] left-3 inline-flex h-8 items-center gap-2 rounded border border-orange-500 bg-white px-3 text-sm font-bold uppercase text-orange-600">
        <Gift className="h-4 w-4 fill-orange-500 text-orange-500" strokeWidth={2} />
        Ưu đãi
      </div>

      <ul className="space-y-3 text-sm leading-6 text-slate-600">
        <OfferItem>
          Tặng 2 Quấn cán vợt Cầu Lông: <span className="text-orange-600">VNB 001, VS002</span> hoặc{" "}
          <span className="text-orange-600">Joto 001</span>
        </OfferItem>
        <OfferItem>Sản phẩm cam kết chính hãng</OfferItem>
        <OfferItem>Một số sản phẩm sẽ được tặng bao đơn hoặc bao nhung bảo vệ vợt</OfferItem>
        <OfferItem>Thanh toán sau khi kiểm tra và nhận hàng (Giao khung vợt)</OfferItem>
        <OfferItem>Bảo hành chính hãng theo nhà sản xuất (Trừ hàng nội địa, xách tay)</OfferItem>
      </ul>

      <div className="mt-8">
        <h3 className="text-base font-bold text-slate-700">
          🎁 Ưu đãi thêm khi mua sản phẩm tại <span className="text-orange-600">VNB Premium</span>
        </h3>
        <ul className="mt-3 space-y-2 text-sm leading-6 text-slate-600">
          {[
            "Sơn logo mặt vợt miễn phí",
            "Bảo hành lưới đan trong 72 giờ",
            "Thay gen vợt miễn phí trọn đời",
            "Tích lũy điểm thành viên Premium",
            "Voucher giảm giá cho lần mua hàng tiếp theo",
          ].map((item) => (
            <li key={item} className="flex gap-2">
              <span className="mt-1 grid h-4 w-4 shrink-0 place-items-center rounded-sm bg-emerald-400 text-white">
                <Check className="h-3 w-3" strokeWidth={3} />
              </span>
              <span>
                <span className="text-orange-600">{item.split(" ")[0]} {item.split(" ")[1]}</span>
                {item.split(" ").slice(2).length > 0 ? ` ${item.split(" ").slice(2).join(" ")}` : ""}
              </span>
            </li>
          ))}
        </ul>
      </div>
    </section>
  );
}

function OfferItem({ children }: { children: React.ReactNode }) {
  return (
    <li className="flex gap-2">
      <Check className="mt-1 h-4 w-4 shrink-0 text-indigo-600" strokeWidth={3} />
      <span>{children}</span>
    </li>
  );
}

function StockPanel() {
  return (
    <aside className="relative rounded border border-dotted border-orange-500 p-3 pt-8 lg:sticky lg:top-24">
      <div className="absolute -top-[15px] left-3 rounded border border-orange-500 bg-white px-3 py-1 text-sm font-bold uppercase text-orange-600">
        Đang có hàng tại
      </div>

      <label className="sr-only" htmlFor="store-city">
        Chọn tỉnh thành
      </label>
      <select
        id="store-city"
        defaultValue={CITIES[0]}
        className="h-9 w-full rounded border border-orange-200 bg-white px-3 text-sm text-slate-700 outline-none focus:border-orange-500 focus:ring-2 focus:ring-orange-100"
      >
        {CITIES.map((city) => (
          <option key={city}>{city}</option>
        ))}
      </select>

      <div className="mt-3 max-h-[570px] overflow-y-auto">
        {STORE_LOCATIONS.map((store) => (
          <div
            key={store}
            className="border-b border-orange-200 bg-[#ef7148] px-3 py-2.5 text-sm font-bold text-white last:border-b-0"
          >
            {store}
          </div>
        ))}
      </div>
    </aside>
  );
}

function VariantPicker({
  variants,
  selectedVariantId,
  onSelect,
  formatPrice,
}: {
  variants: ProductVariantModel[];
  selectedVariantId: number | null;
  onSelect: (variant: ProductVariantModel) => void;
  formatPrice: (price: number) => string;
}) {
  if (variants.length === 0) {
    return null;
  }

  return (
    <div className="mt-5">
      <p className="mb-2 text-sm font-bold text-slate-800">Biến thể</p>
      <div className="grid gap-2 sm:grid-cols-2">
        {variants.map((variant) => {
          const label = [variant.weight, variant.gripSize, variant.color].filter(Boolean).join(" / ") || variant.sku;
          const selected = selectedVariantId === variant.id;

          return (
            <button
              key={variant.id}
              type="button"
              onClick={() => onSelect(variant)}
              disabled={variant.stock <= 0}
              className={cn(
                "min-h-14 rounded border px-3 py-2 text-left text-sm transition",
                selected
                  ? "border-orange-500 bg-orange-50 text-orange-700"
                  : "border-slate-200 bg-white text-slate-700 hover:border-orange-300",
                variant.stock <= 0 && "cursor-not-allowed opacity-55"
              )}
            >
              <span className="block font-bold">{label}</span>
              <span className="mt-1 block text-xs">
                {formatPrice(variant.price)} · còn {variant.stock}
              </span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
