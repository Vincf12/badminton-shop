import { MainLayout } from "@/widgets/layout";
import { ProductDetailPage } from "@/views/product-detail";
import { fetchProduct } from "@/entities/product";

interface ProductPageProps {
  params: Promise<{
    id: string;
  }>;
}

export default async function ProductPage({ params }: ProductPageProps) {
  const { id } = await params;
  const productId = Number(id);
  let product = null;
  let error: string | null = null;

  if (!Number.isFinite(productId)) {
    return (
      <MainLayout>
        <ProductError message="Mã sản phẩm không hợp lệ." />
      </MainLayout>
    );
  }

  try {
    product = await fetchProduct(productId);
  } catch (loadError) {
    error = loadError instanceof Error ? loadError.message : "Không thể tải sản phẩm";
  }

  if (error || !product) {
    return (
      <MainLayout>
        <ProductError message={error ?? "Không thể tải sản phẩm"} />
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <ProductDetailPage product={product} />
    </MainLayout>
  );
}

function ProductError({ message }: { message: string }) {
  return (
    <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6">
      <div className="rounded-2xl border border-red-200 bg-red-50 p-8 text-sm font-semibold text-red-700">
        {message}
      </div>
    </div>
  );
}
