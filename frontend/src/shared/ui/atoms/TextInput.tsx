import type { InputHTMLAttributes, ReactNode } from "react";
import { cn } from "@/shared/lib";

interface TextInputProps extends InputHTMLAttributes<HTMLInputElement> {
  leftIcon?: ReactNode;
}

export default function TextInput({ leftIcon, className, ...props }: TextInputProps) {
  return (
    <label className="flex items-center gap-2 rounded-full border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-400 shadow-[0_1px_2px_rgba(15,23,42,0.04)] transition-colors focus-within:border-emerald-500 focus-within:ring-4 focus-within:ring-emerald-100">
      {leftIcon ? <span className="shrink-0">{leftIcon}</span> : null}
      <input
        className={cn("w-full bg-transparent text-sm text-slate-800 outline-none placeholder-slate-400", className)}
        {...props}
      />
    </label>
  );
}
