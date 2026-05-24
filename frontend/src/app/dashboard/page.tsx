"use client";

import React from "react";
import MainLayout from "@/app/components/layout/MainLayout";
import { User, ShoppingBag, Heart, Settings } from "lucide-react";

export default function DashboardPage() {
  const stats = [
    { label: "Đơn hàng", value: "12", icon: ShoppingBag },
    { label: "Yêu thích", value: "8", icon: Heart },
    { label: "Đánh giá", value: "24", icon: User },
  ];

  const recentOrders = [
    {
      id: "#ORD001",
      date: "2024-01-15",
      status: "Đã giao",
      total: "4.500.000đ",
    },
    {
      id: "#ORD002", 
      date: "2024-01-12",
      status: "Đang giao",
      total: "2.200.000đ",
    },
    {
      id: "#ORD003",
      date: "2024-01-10", 
      status: "Đã hủy",
      total: "1.800.000đ",
    },
  ];

  return (
    <MainLayout>
      <div className="dashboard-page">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-800 mb-4">Bảng điều khiển</h1>
          <p className="text-gray-600">Quản lý tài khoản và đơn hàng của bạn</p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          {stats.map((stat, index) => {
            const Icon = stat.icon;
            return (
              <div key={index} className="bg-white rounded-2xl p-6 shadow-md">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-gray-600 text-sm">{stat.label}</p>
                    <p className="text-3xl font-bold text-gray-800">{stat.value}</p>
                  </div>
                  <div className="w-12 h-12 bg-emerald-100 rounded-lg flex items-center justify-center">
                    <Icon className="w-6 h-6 text-emerald-600" />
                  </div>
                </div>
              </div>
            );
          })}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Profile Card */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-2xl p-6 shadow-md">
              <div className="text-center">
                <div className="w-24 h-24 bg-emerald-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <User className="w-12 h-12 text-emerald-600" />
                </div>
                <h3 className="text-xl font-bold text-gray-800 mb-2">Nguyễn Văn A</h3>
                <p className="text-gray-600 mb-6">nguyenvana@email.com</p>
                <button className="w-full bg-emerald-600 hover:bg-emerald-700 text-white py-3 rounded-lg font-semibold transition-colors duration-300 flex items-center justify-center gap-2">
                  <Settings className="w-4 h-4" />
                  Chỉnh sửa hồ sơ
                </button>
              </div>
            </div>
          </div>

          {/* Recent Orders */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-2xl p-6 shadow-md">
              <h3 className="text-xl font-bold text-gray-800 mb-6">Đơn hàng gần đây</h3>
              <div className="space-y-4">
                {recentOrders.map((order, index) => (
                  <div key={index} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                    <div>
                      <p className="font-semibold text-gray-800">{order.id}</p>
                      <p className="text-sm text-gray-600">{order.date}</p>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-gray-800">{order.total}</p>
                      <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                        order.status === 'Đã giao' ? 'bg-green-100 text-green-800' :
                        order.status === 'Đang giao' ? 'bg-blue-100 text-blue-800' :
                        'bg-red-100 text-red-800'
                      }`}>
                        {order.status}
                      </span>
                    </div>
                  </div>
                ))}
              </div>
              <button className="w-full mt-6 text-emerald-600 hover:text-emerald-700 font-semibold py-3 border border-emerald-600 rounded-lg transition-colors duration-300">
                Xem tất cả đơn hàng
              </button>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
