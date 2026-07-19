"use client";

import { useState } from "react";
import { ShoppingBag } from "lucide-react";
import { Button } from "@/shared/ui";
import { cn } from "@/shared/lib";
import { cartService } from "../api/cartService";

interface AddToCartButtonProps {
  variantId: number | null;
  quantity?: number;
  disabled?: boolean;
  className?: string;
  onSuccess?: () => void;
  onError?: (message: string) => void;
}

export function AddToCartButton({
  variantId,
  quantity = 1,
  disabled,
  className,
  onSuccess,
  onError,
}: AddToCartButtonProps) {
  const [adding, setAdding] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const handleAddToCart = async () => {
    if (!variantId) {
      const errorMessage = "Sản phẩm chưa có biến thể để thêm vào giỏ hàng.";
      setMessage(errorMessage);
      onError?.(errorMessage);
      return;
    }

    setAdding(true);
    setMessage(null);

    try {
      await cartService.addItem(variantId, quantity);
      setMessage("Đã thêm sản phẩm vào giỏ hàng.");
      onSuccess?.();
    } catch (addError) {
      const errorMessage =
        addError instanceof Error
          ? addError.message
          : "Không thể thêm sản phẩm vào giỏ hàng.";
      setMessage(errorMessage);
      onError?.(errorMessage);
    } finally {
      setAdding(false);
    }
  };

  return (
    <div>
      <Button
        type="button"
        onClick={handleAddToCart}
        disabled={adding || disabled}
        className={cn(
          "h-12 rounded bg-[#ff6b6b] px-5 text-base font-bold uppercase text-white hover:bg-[#fa5252] disabled:bg-slate-300",
          className
        )}
      >
        <ShoppingBag className="h-5 w-5" strokeWidth={2} />
        {adding ? "Đang thêm..." : "Thêm vào giỏ hàng"}
      </Button>

      {message ? (
        <p
          className={cn(
            "mt-4 rounded border px-4 py-3 text-sm font-semibold",
            message.startsWith("Đã")
              ? "border-emerald-200 bg-emerald-50 text-emerald-700"
              : "border-red-200 bg-red-50 text-red-700"
          )}
        >
          {message}
        </p>
      ) : null}
    </div>
  );
}
