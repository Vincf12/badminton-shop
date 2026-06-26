import Link from "next/link";
import { ArrowRight } from "lucide-react";

type SectionTitleProps = {
  title?: string;
  description?: string;
};

export default function SectionTitle({ title = "Sản phẩm mới", description }: SectionTitleProps) {
  return (
    <div className="mb-7 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h2 className="text-3xl font-black tracking-[-0.04em] text-slate-950 sm:text-4xl">{title}</h2>
        {description ? <p className="mt-2 max-w-2xl text-sm leading-6 text-slate-600">{description}</p> : null}
      </div>
      <Link
        href="/shop"
        className="inline-flex items-center gap-2 text-sm font-bold text-emerald-700 transition hover:text-emerald-800"
      >
        Xem tất cả
        <ArrowRight className="h-4 w-4" strokeWidth={1.8} />
      </Link>
    </div>
  );
}
