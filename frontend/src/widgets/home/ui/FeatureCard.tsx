import type { LucideIcon } from "lucide-react";

interface Feature {
  icon: LucideIcon;
  title: string;
  description: string;
}

interface FeatureCardProps {
  feature: Feature;
}

export default function FeatureCard({ feature }: FeatureCardProps) {
  const Icon = feature.icon;

  return (
    <div className="group border-t border-slate-200 py-5 transition-colors hover:border-emerald-300">
      <div className="flex items-start gap-4">
        <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-2xl bg-emerald-50 text-emerald-700 ring-1 ring-emerald-100 transition group-hover:bg-emerald-600 group-hover:text-white">
          <Icon className="h-5 w-5" strokeWidth={1.8} />
        </div>
        <div>
          <h3 className="text-base font-black tracking-[-0.02em] text-slate-950">{feature.title}</h3>
          <p className="mt-1 text-sm leading-6 text-slate-600">{feature.description}</p>
        </div>
      </div>
    </div>
  );
}
