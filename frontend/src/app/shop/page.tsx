import { MainLayout } from "@/widgets/layout";
import { ShopClient } from "@/features/shop";
import { fetchCategories, fetchProducts } from "@/entities/product";
import type { ProductCardModel, ProductCategory } from "@/entities/product";

export default async function ShopPage() {
  const { products, categories, error } = await getShopData();

  return (
    <MainLayout>
      <ShopClient products={products} categories={categories} error={error} />
    </MainLayout>
  );
}

async function getShopData(): Promise<{
  products: ProductCardModel[];
  categories: ProductCategory[];
  error: string | null;
}> {
  try {
    const [products, categories] = await Promise.all([
      fetchProducts(),
      fetchCategories(),
    ]);

    return { products, categories, error: null };
  } catch (loadError) {
    return {
      products: [],
      categories: [],
      error: loadError instanceof Error ? loadError.message : "Không thể tải dữ liệu sản phẩm",
    };
  }
}
