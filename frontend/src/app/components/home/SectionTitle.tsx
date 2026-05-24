import React from 'react'
import Link from 'next/link'
import ProductGrid from './ProductGrid'

type ProductsListProps = {
  title?: string
}

function ProductsList({ title = 'Sản phẩm mới' }: ProductsListProps) {
  return (
    <div className="flex justify-between items-center mb-8">
      <h2 className="text-3xl font-bold text-gray-800">{title}</h2>
      <Link
        href="/shop"
        className="text-emerald-600 hover:text-emerald-700 font-semibold flex items-center gap-2 group"
      >
        Xem tất cả
        <span className="transform group-hover:translate-x-1 transition-transform">→</span>
      </Link>
    </div>
  )
}

export default ProductsList
