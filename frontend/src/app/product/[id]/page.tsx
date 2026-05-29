"use client";

import React, { useState } from "react";
import MainLayout from "@/app/components/layout/MainLayout";
import Image from "next/image";
import { ShoppingCart, Heart, Star, Minus, Plus } from "lucide-react";

// 1. Cập nhật lại kiểu dữ liệu của params thành một Promise
interface ProductPageProps {
  params: Promise<{
    id: string;
  }>;
}

export default function ProductPage({ params }: ProductPageProps) {
  // 2. Sử dụng React.use() để giải nén params một cách đồng bộ trong Client Component
  const unwrappedParams = React.use(params);
  const productId = unwrappedParams.id;

  const [quantity, setQuantity] = useState(1);
  const [selectedImage, setSelectedImage] = useState(0);

  // ⭐ State cho phần đánh giá
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState("");
  const [reviews, setReviews] = useState<
    { rating: number; comment: string; date: string }[]
  >([]);

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

  // Mock data - sau này sẽ fetch từ API dựa vào params.id
  const product = {
    id: productId,
    name: "Vợt Cầu Lông Yonex Astrox 99",
    price: 4500000,
    originalPrice: 5500000,
    description:
      "Vợt cầu lông Yonex Astrox 99 là dòng vợt cao cấp dành cho người chơi chuyên nghiệp. Thiết kế đột phá với công nghệ Rotational Generator System giúp tăng cường sức mạnh đập cầu.",
    images: [
      "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
      "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
      "https://cdn.shopvnb.com/uploads/san_pham/vot-cau-long-vnb-v200-xanh-2.webp",
    ],
    specs: {
      weight: "83g",
      balance: "Head Heavy",
      flex: "Stiff",
      material: "Carbon Nanotube",
    },
    inStock: true,
    rating: 4.8,
    reviews: 128,
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };

  const handleQuantityChange = (type: "increase" | "decrease") => {
    if (type === "increase") {
      setQuantity((prev) => prev + 1);
    } else if (quantity > 1) {
      setQuantity((prev) => prev - 1);
    }
  };

  return (
    <MainLayout>
      <div className="product-detail-page text-black">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 mb-16">
          {/* Product Images */}
          <div className="space-y-4">
            <div className="relative aspect-square rounded-2xl overflow-hidden bg-white shadow-lg">
              <Image
                src={product.images[selectedImage]}
                alt={product.name}
                fill
                className="object-contain p-8"
              />
            </div>
            <div className="grid grid-cols-4 gap-4">
              {product.images.map((image, index) => (
                <button
                  key={index}
                  onClick={() => setSelectedImage(index)}
                  className={`relative aspect-square rounded-lg overflow-hidden border-2 transition-all ${
                    selectedImage === index
                      ? "border-emerald-600 scale-105"
                      : "border-gray-200 hover:border-gray-300"
                  }`}
                >
                  <Image
                    src={image}
                    alt={`${product.name} ${index + 1}`}
                    fill
                    className="object-cover"
                  />
                </button>
              ))}
            </div>
          </div>

          {/* Product Info */}
          <div className="space-y-6">
            <div>
              <h1 className="text-4xl font-bold text-gray-800 mb-4">
                {product.name}
              </h1>
              <div className="flex items-center gap-4 mb-4">
                <div className="flex items-center gap-1">
                  {[...Array(5)].map((_, i) => (
                    <Star
                      key={i}
                      className={`w-5 h-5 ${
                        i < Math.floor(product.rating)
                          ? "fill-yellow-400 text-yellow-400"
                          : "text-gray-300"
                      }`}
                    />
                  ))}
                </div>
                <span className="text-gray-600">
                  {product.rating} ({product.reviews} đánh giá)
                </span>
              </div>
            </div>

            <div className="flex items-baseline gap-4">
              <span className="text-4xl font-bold text-emerald-600">
                {formatPrice(product.price)}
              </span>
              <span className="text-2xl text-gray-400 line-through">
                {formatPrice(product.originalPrice)}
              </span>
              <span className="bg-red-500 text-white px-3 py-1 rounded-full text-sm font-semibold">
                -
                {Math.round(
                  ((product.originalPrice - product.price) /
                    product.originalPrice) *
                    100
                )}
                %
              </span>
            </div>

            <div className="border-t border-b py-6 space-y-4">
              <h3 className="text-xl font-bold text-gray-800">
                Thông số kỹ thuật
              </h3>
              <div className="grid grid-cols-2 gap-4">
                {Object.entries(product.specs).map(([key, value]) => (
                  <div key={key} className="flex justify-between">
                    <span className="text-gray-600 capitalize">{key}:</span>
                    <span className="font-semibold">{value}</span>
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
                  <span className="w-16 text-center text-xl font-bold">
                    {quantity}
                  </span>
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

              {product.inStock ? (
                <div className="flex items-center gap-2 text-emerald-600">
                  <div className="w-2 h-2 bg-emerald-600 rounded-full"></div>
                  <span className="font-semibold">Còn hàng</span>
                </div>
              ) : (
                <div className="flex items-center gap-2 text-red-600">
                  <div className="w-2 h-2 bg-red-600 rounded-full"></div>
                  <span className="font-semibold">Hết hàng</span>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Product Description */}
        <div className="bg-white rounded-2xl shadow-md p-8">
          <h2 className="text-3xl font-bold text-gray-800 mb-6">
            Mô tả sản phẩm
          </h2>
          <p className="text-gray-700 leading-relaxed text-lg">
            {product.description}
          </p>
        </div>

        {/* ⭐ Review Section */}
        <div className="bg-white rounded-2xl shadow-md p-8 mt-12">
          <h1 className="text-2xl font-bold text-gray-800 mb-6">
            Đánh giá và nhận xét sản phẩm {product.name}
          </h1>

          {/* Phần chọn sao */}
          <div className="mb-4">
            <p className="font-semibold text-gray-700 mb-2">
              Đánh giá của bạn:
            </p>
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
              className="w-full border border-gray-300 rounded-md p-3 mt-2 focus:ring-2 focus:ring-emerald-400 focus:outline-none"
              rows={4}
              placeholder="Nhập đánh giá của bạn..."
              value={comment}
              onChange={(e) => setComment(e.target.value)}
            ></textarea>
            <button
              className="mt-3 bg-emerald-600 hover:bg-emerald-700 text-white py-2 px-5 rounded-lg transition-colors"
              onClick={handleSubmit}
            >
              Gửi đánh giá
            </button>
          </div>

          {/* Danh sách đánh giá */}
          {reviews.length > 0 ? (
            <div className="mt-8">
              <h2 className="text-xl font-semibold mb-4 text-gray-800">
                Các đánh giá gần đây:
              </h2>
              <div className="space-y-6">
                {reviews.map((r, index) => (
                  <div key={index} className="border-b pb-4">
                    <div className="flex items-center gap-2 mb-1">
                      {[1, 2, 3, 4, 5].map((star) => (
                        <Star
                          key={star}
                          className={`w-5 h-5 ${
                            r.rating >= star
                              ? "fill-yellow-400 text-yellow-400"
                              : "text-gray-300"
                          }`}
                        />
                      ))}
                      <span className="text-gray-500 text-sm ml-2">
                        {r.date}
                      </span>
                    </div>
                    <p className="text-gray-700">{r.comment}</p>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <p className="text-gray-500 mt-6">
              Chưa có đánh giá nào cho sản phẩm này.
            </p>
          )}
        </div>
      </div>
    </MainLayout>
  );
}
