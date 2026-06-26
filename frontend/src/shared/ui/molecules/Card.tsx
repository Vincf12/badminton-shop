import type { HTMLAttributes } from "react";
import { cn } from "@/shared/lib";

type CardVariant = "default" | "elevated" | "soft" | "danger";

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  variant?: CardVariant;
}

const variants: Record<CardVariant, string> = {
  default: "border border-slate-200/80 bg-white shadow-[0_1px_2px_rgba(15,23,42,0.04)]",
  elevated: "border border-slate-200/70 bg-white shadow-[0_18px_50px_rgba(15,23,42,0.08)]",
  soft: "border border-emerald-100 bg-emerald-50/70",
  danger: "border border-red-200 bg-red-50 text-red-700",
};

export default function Card({
  variant = "default",
  className,
  children,
  ...props
}: CardProps) {
  return (
    <div className={cn("rounded-2xl", variants[variant], className)} {...props}>
      {children}
    </div>
  );
}
