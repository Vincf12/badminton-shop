"use client";

import React, { useState } from "react";

interface FilterGroupProps {
  title: string;
  children: React.ReactNode;
  defaultOpen?: boolean; // Cho phép cấu hình mặc định đóng hay mở
}

export default function FilterGroup({ title, children, defaultOpen = false }: FilterGroupProps) {
  // Trạng thái đóng/mở
  const [isOpen, setIsOpen] = useState(defaultOpen);

  return (
    <div className="border-b border-gray-200 py-3 mb-3 last:border-none last:mb-0">
      {/* Nút bấm tiêu đề (Trigger) - Đã thêm w-full để kéo giãn bằng kích thước các nút khác */}
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        aria-expanded={isOpen}
        className="flex w-full items-center justify-between text-sm font-medium text-gray-800 hover:text-emerald-600 transition-colors"
      >
        <span className="text-left leading-6">{title}</span>
        <span className="flex-none">
          <svg
            className={`h-4 w-4 transform text-gray-400 transition-transform duration-200 ${
              isOpen ? "rotate-180" : ""
            }`}
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth={2}
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <path d="M6 9l6 6 6-6" />
          </svg>
        </span>
      </button>

      {/* Phần nội dung bên dưới (Content) với animation chiều cao */}
      <div
        className={`overflow-hidden transition-all duration-200 ${
          isOpen ? "max-h-96 mt-2" : "max-h-0"
        }`}
      >
        <div className="space-y-2 py-1 text-sm text-gray-700">{children}</div>
      </div>
    </div>
  );
}