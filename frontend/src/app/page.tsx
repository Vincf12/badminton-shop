import { Headphones, RotateCcw, ShieldCheck, Truck } from "lucide-react";
import { MainLayout } from "@/widgets/layout";
import { HeroBanner, FeatureCard, Newsletter, SectionTitle } from "@/widgets/home";
import { ProductGrid } from "@/entities/product";
import { fetchProducts } from "@/entities/product";
import type { ProductCardModel } from "@/entities/product";

const features = [
  {
    icon: ShieldCheck,
    title: "Hàng chính hãng",
    description: "Sản phẩm được chọn lọc từ các thương hiệu cầu lông uy tín.",
  },
  {
    icon: Truck,
    title: "Giao nhanh",
    description: "Đóng gói chắc chắn, giao hàng linh hoạt cho lịch tập của bạn.",
  },
  {
    icon: RotateCcw,
    title: "Đổi trả rõ ràng",
    description: "Hỗ trợ đổi trả trong 30 ngày khi sản phẩm phát sinh lỗi.",
  },
  {
    icon: Headphones,
    title: "Tư vấn đúng nhu cầu",
    description: "Gợi ý vợt, giày và phụ kiện theo trình độ và lối chơi.",
  },
];

export default async function HomePage() {
  const { products, error } = await getHomeProducts();

  const getSectionProducts = (categoryName: string) => {
    const filtered = products.filter((product) => product.category === categoryName);
    return filtered.length > 0 ? filtered : products.slice(0, 5);
  };

  return (
    <MainLayout>
      <HeroBanner />

      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:py-16">
        <section className="grid gap-x-8 gap-y-2 md:grid-cols-2 lg:grid-cols-4">
          {features.map((feature) => (
            <FeatureCard key={feature.title} feature={feature} />
          ))}
        </section>

        <section className="mt-16 space-y-14">
          <div>
            <SectionTitle
              title="Sản phẩm mới"
              description="Những lựa chọn đang được người chơi FlyShot quan tâm trong tuần này."
            />
            {error ? <p className="mb-6 rounded-2xl bg-red-50 px-4 py-3 text-sm font-semibold text-red-700">{error}</p> : null}
            <ProductGrid products={products.slice(0, 5)} />
          </div>

          <div>
            <SectionTitle title="Vợt cầu lông" />
            <ProductGrid products={getSectionProducts("Vợt cầu lông")} />
          </div>

          <div>
            <SectionTitle title="Giày cầu lông" />
            <ProductGrid products={getSectionProducts("Giày cầu lông")} />
          </div>

          <div>
            <SectionTitle title="Phụ kiện" />
            <ProductGrid products={getSectionProducts("Phụ kiện")} />
          </div>
        </section>

        <div className="mt-16">
          <Newsletter />
        </div>
      </div>
    </MainLayout>
  );
}

async function getHomeProducts(): Promise<{
  products: ProductCardModel[];
  error: string | null;
}> {
  try {
    return {
      products: await fetchProducts(),
      error: null,
    };
  } catch (loadError) {
    return {
      products: [],
      error: loadError instanceof Error ? loadError.message : "Không thể tải sản phẩm",
    };
  }
}
