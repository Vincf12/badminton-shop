"use client";

import React, { useEffect, useMemo, useState } from "react";
import MainLayout from "@/app/components/layout/MainLayout";
import Image from "next/image";
import { useParams } from "next/navigation";
import { ShoppingCart, Heart, Star, Minus, Plus } from "lucide-react";
import { fetchProduct } from "@/services/productService";
import type { ProductDetailModel } from "@/types/product";

export default function ProductPage() {
  const params = useParams<{ id: string }>();
  const productId = Number(params?.id);

  const [product, setProduct] = useState<ProductDetailModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [selectedImage, setSelectedImage] = useState(0);
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState("");
  const [reviews, setReviews] = useState<
    { rating: number; comment: string; date: string }[]
  >([]);

  useEffect(() => {
    let isActive = true;

    const loadProduct = async () => {
      if (!Number.isFinite(productId)) {
        setError("Mã sản phẩm không hợp lệ.");
        setLoading(false);
        return;
      }

      try {
        const data = await fetchProduct(productId);
        if (isActive) {
          setProduct(data);
        }
      } catch (loadError) {
        if (isActive) {
          setError(loadError instanceof Error ? loadError.message : "Không thể tải sản phẩm");
        }
      } finally {
        if (isActive) {
          setLoading(false);
        }
      }
    };

    loadProduct();

    return () => {
      isActive = false;
    };
  }, [productId]);

  useEffect(() => {
    setSelectedImage(0);
  }, [product?.id]);

  const images = useMemo(() => {
    if (!product) {
      return [];
    }

    const image = product.image || "/assets/images/banner-netro.png";
    return [image, image, image];
  }, [product]);

  const formatPrice = (price: number) =>
    new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(price);

  const handleQuantityChange = (type: "increase" | "decrease") => {
    if (type === "increase") {
      setQuantity((prev) => prev + 1);
      return;
    }

    if (quantity > 1) {
      setQuantity((prev) => prev - 1);
    }
  };

  const handleSubmit = () => {
    if (rating === 0 || comment.trim() === "") {
      alert("Vui lòng chọn số sao và nhập nội dung đánh giá!");
      return;
    }

    const newReview = {
      rating,
      comment,
      date: new Date().toLocaleString("vi-VN"),
    };

    setReviews([newReview, ...reviews]);
    setRating(0);
    setComment("");
    alert("Cảm ơn bạn đã gửi đánh giá!");
  };

  return (
    <MainLayout>
      <div className="product-detail-page text-black">
        {loading ? (
          <div className="rounded-2xl bg-white p-8 shadow-md text-gray-600">Đang tải chi tiết sản phẩm...</div>
        ) : error ? (
          <div className="rounded-2xl bg-red-50 p-8 text-red-700 border border-red-200">{error}</div>
        ) : product ? (
          <>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 mb-16">
              <div className="space-y-4">
                <div className="relative aspect-square rounded-2xl overflow-hidden bg-white shadow-lg">
                  <Image
                    src={images[selectedImage]}
                    alt={product.name}
                    fill
                    className="object-contain p-8"
                  />
                </div>

                <div className="grid grid-cols-4 gap-4">
                  {images.map((image, index) => (
                    <button
                      key={`${image}-${index}`}
                      onClick={() => setSelectedImage(index)}
                      className={`relative aspect-square rounded-lg overflow-hidden border-2 transition-all ${
                        selectedImage === index
                          ? "border-emerald-600 scale-105"
                          : "border-gray-200 hover:border-gray-300"
                      }`}
                    >
                      <Image src={image} alt={`${product.name} ${index + 1}`} fill className="object-cover" />
                    </button>
                  ))}
                </div>
              </div>

              <div className="space-y-6">
                <div>
                  <h1 className="text-4xl font-bold text-gray-800 mb-4">{product.name}</h1>
                  <div className="flex flex-wrap items-center gap-3 mb-4 text-sm text-gray-600">
                    <span className="rounded-full bg-emerald-100 px-3 py-1 text-emerald-700 font-medium">
                      {product.category}
                    </span>
                    {product.brand ? <span>Thương hiệu: {product.brand}</span> : null}
                    <span>{product.stock > 0 ? `Còn ${product.stock} sản phẩm` : "Hết hàng"}</span>
                  </div>
                </div>

                <div className="flex items-baseline gap-4">
                  <span className="text-4xl font-bold text-emerald-600">{formatPrice(product.price)}</span>
                  <span className="bg-red-500 text-white px-3 py-1 rounded-full text-sm font-semibold">
                    {product.stock > 0 ? "Sẵn sàng giao" : "Tạm hết hàng"}
                  </span>
                </div>

                <div className="border-t border-b py-6 space-y-4">
                  <h3 className="text-xl font-bold text-gray-800">Thông tin sản phẩm</h3>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    {[
                      ["Danh mục", product.category],
                      ["Thương hiệu", product.brand || "Chưa cập nhật"],
                      ["Tồn kho", `${product.stock}`],
                      [
                        "Cập nhật",
                        product.updatedAt
                          ? new Date(product.updatedAt).toLocaleDateString("vi-VN")
                          : new Date(product.createdAt).toLocaleDateString("vi-VN"),
                      ],
                    ].map(([label, value]) => (
                      <div key={label} className="flex justify-between gap-4 border-b border-dashed border-gray-200 pb-2">
                        <span className="text-gray-600">{label}:</span>
                        <span className="font-semibold text-right">{value}</span>
                      </div>
                    ))}
                  </div>
                </div>

                <div className="space-y-4">
                  <div className="flex items-center gap-4">
                    <span className="text-gray-700 font-semibold">Số lượng:</span>
                    <div className="flex items-center gap-3">
                      <button
                        onClick={() => handleQuantityChange("decrease")}
                        className="w-10 h-10 rounded-full bg-gray-100 hover:bg-gray-200 flex items-center justify-center transition-colors"
                      >
                        <Minus className="w-5 h-5" />
                      </button>
                      <span className="w-16 text-center text-xl font-bold">{quantity}</span>
                      <button
                        onClick={() => handleQuantityChange("increase")}
                        className="w-10 h-10 rounded-full bg-gray-100 hover:bg-gray-200 flex items-center justify-center transition-colors"
                      >
                        <Plus className="w-5 h-5" />
                      </button>
                    </div>
                  </div>

                  <div className="flex gap-4">
                    <button className="flex-1 bg-orange-600 hover:bg-orange-700 text-white py-4 rounded-xl font-bold text-lg transition-all duration-300 flex items-center justify-center gap-2 shadow-lg hover:shadow-xl">
                      <ShoppingCart className="w-6 h-6" />
                      Mua ngay
                    </button>

                    <button className="flex-1 bg-emerald-600 hover:bg-emerald-700 text-white py-4 rounded-xl font-bold text-lg transition-all duration-300 flex items-center justify-center gap-2 shadow-lg hover:shadow-xl">
                      <ShoppingCart className="w-6 h-6" />
                      Thêm vào giỏ hàng
                    </button>

                    <button className="w-14 h-14 bg-gray-100 hover:bg-gray-200 rounded-xl flex items-center justify-center transition-colors">
                      <Heart className="w-6 h-6 text-gray-600" />
                    </button>
                  </div>

                  <div className={`flex items-center gap-2 ${product.stock > 0 ? "text-emerald-600" : "text-red-600"}`}>
                    <div className={`w-2 h-2 rounded-full ${product.stock > 0 ? "bg-emerald-600" : "bg-red-600"}`} />
                    <span className="font-semibold">{product.stock > 0 ? "Còn hàng" : "Hết hàng"}</span>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-white rounded-2xl shadow-md p-8">
              <h2 className="text-3xl font-bold text-gray-800 mb-6">Mô tả sản phẩm</h2>
              <p className="text-gray-700 leading-relaxed text-lg">
                {product.description || "Chưa có mô tả cho sản phẩm này."}
              </p>
            </div>

            <div className="bg-white rounded-2xl shadow-md p-8 mt-12">
              <h1 className="text-2xl font-bold text-gray-800 mb-6">Đánh giá và nhận xét sản phẩm {product.name}</h1>

              <div className="mb-4">
                <p className="font-semibold text-gray-700 mb-2">Đánh giá của bạn:</p>
                <div className="flex items-center gap-1 mb-4">
                  {[1, 2, 3, 4, 5].map((star) => (
                    <Star
                      key={star}
                      className={`w-8 h-8 cursor-pointer transition-transform ${
                        rating >= star
                          ? "fill-yellow-400 text-yellow-400 scale-110"
                          : "text-gray-300"
                      }`}
                      onClick={() => setRating(star)}
                    />
                  ))}
                </div>

                <textarea
                  value={comment}
                  onChange={(event) => setComment(event.target.value)}
                  placeholder="Nhập nhận xét của bạn..."
                  className="w-full rounded-xl border border-gray-300 p-4 outline-none focus:border-emerald-500 min-h-32"
                />

                <button
                  onClick={handleSubmit}
                  className="mt-4 bg-emerald-600 hover:bg-emerald-700 text-white px-6 py-3 rounded-xl font-semibold transition-colors"
                >
                  Gửi đánh giá
                </button>
              </div>

              <div className="space-y-4">
                {reviews.length === 0 ? (
                  <p className="text-gray-500">Chưa có đánh giá nào.</p>
                ) : (
                  reviews.map((item, index) => (
                    <div key={`${item.date}-${index}`} className="rounded-xl border border-gray-200 p-4">
                      <div className="flex items-center gap-2 mb-2 text-yellow-500">
                        {Array.from({ length: item.rating }).map((_, starIndex) => (
                          <Star key={starIndex} className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                        ))}
                      </div>
                      <p className="text-gray-700">{item.comment}</p>
                      <p className="mt-2 text-xs text-gray-400">{item.date}</p>
                    </div>
                  ))
                )}
              </div>
            </div>
          </>
        ) : null}
      </div>
    </MainLayout>
  );
}
