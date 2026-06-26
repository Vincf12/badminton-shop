import type { ButtonHTMLAttributes, ReactNode } from "react";
import { cn } from "@/shared/lib";

type ButtonVariant = "primary" | "secondary" | "outline" | "soft" | "danger" | "ghost";
type ButtonSize = "sm" | "md" | "lg" | "icon";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  icon?: ReactNode;
}

const variants: Record<ButtonVariant, string> = {
  primary: "border border-emerald-600 bg-emerald-600 text-white shadow-[0_14px_30px_rgba(5,150,105,0.22)] hover:bg-emerald-700 disabled:border-slate-300 disabled:bg-slate-300",
  secondary: "border border-slate-950 bg-slate-950 text-white hover:bg-slate-800 disabled:border-slate-300 disabled:bg-slate-300",
  outline: "border border-slate-200 bg-white text-slate-800 hover:border-emerald-500 hover:text-emerald-700",
  soft: "border border-emerald-100 bg-emerald-50 text-emerald-800 hover:bg-emerald-100",
  danger: "border border-red-200 bg-red-50 text-red-700 hover:bg-red-100",
  ghost: "border border-transparent bg-transparent text-slate-700 hover:bg-slate-100",
};

const sizes: Record<ButtonSize, string> = {
  sm: "h-9 px-3 text-sm",
  md: "h-10 px-4 text-sm",
  lg: "h-14 px-6 text-base",
  icon: "h-10 w-10 p-0",
};

export default function Button({
  type = "button",
  variant = "primary",
  size = "md",
  icon,
  className,
  children,
  ...props
}: ButtonProps) {
  return (
    <button
      type={type}
      className={cn(
        "inline-flex shrink-0 items-center justify-center gap-2 rounded-full font-semibold transition-all active:translate-y-px disabled:cursor-not-allowed",
        variants[variant],
        sizes[size],
        className
      )}
      {...props}
    >
      {icon}
      {children}
    </button>
  );
}
