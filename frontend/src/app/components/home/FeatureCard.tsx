interface Feature {
  icon: string;
  title: string;
  description: string;
}

interface FeatureCardProps {
  feature: Feature;
}

export default function FeatureCard({ feature }: FeatureCardProps) {
  return (
    <div className="bg-white rounded-2xl p-3.5 shadow-md hover:shadow-xl transition-all duration-300 hover:-translate-y-2 border border-gray-100 flex items-start gap-4">
      <div className="text-4xl leading-none shrink-0 mt-1">{feature.icon}</div>
      <div className="min-w-0">
        <h3 className="text-xl font-bold text-gray-800">{feature.title}</h3>
        <p className="text-gray-600">{feature.description}</p>
      </div>
    </div>
  );
}