import type { HTMLAttributes } from "react";
import { cn } from "@/shared/lib";

type BadgeVariant = "primary" | "success" | "danger" | "neutral";

interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  variant?: BadgeVariant;
}

const variants: Record<BadgeVariant, string> = {
  primary: "border border-emerald-100 bg-emerald-50 text-emerald-800",
  success: "bg-emerald-600 text-white",
  danger: "bg-red-500 text-white",
  neutral: "border border-slate-200 bg-white text-slate-700",
};

export default function Badge({
  variant = "primary",
  className,
  children,
  ...props
}: BadgeProps) {
  return (
    <span
      className={cn("inline-flex items-center rounded-full px-3 py-1 text-sm font-semibold", variants[variant], className)}
      {...props}
    >
      {children}
    </span>
  );
}
