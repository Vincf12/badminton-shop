import type { Metadata } from "next";
import "./globals.css";
import { AuthProvider } from "@/features/auth";

export const metadata: Metadata = {
  title: "FlyShot - Premium Badminton Shop",
  description: "FlyShot nâng tầm trải nghiệm cầu lông với vợt, giày và phụ kiện chính hãng cho mọi trình độ.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="vi">
      <body className="antialiased">
        <AuthProvider>{children}</AuthProvider>
      </body>
    </html>
  );
}
