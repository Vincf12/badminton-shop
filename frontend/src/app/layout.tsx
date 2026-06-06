import type { Metadata } from "next";
import "./globals.css";
import { AuthProvider } from "@/contexts/AuthContext";

export const metadata: Metadata = {
  title: "FlyShot - Premium Badminton Shop",
  description: "FlyShot - Nâng tầm trải nghiệm cầu lông của bạn. Cung cấp vợt, giày và phụ kiện chính hãng, cho mọi trình độ.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="antialiased">
        <AuthProvider>
          {children}
        </AuthProvider>
      </body>
    </html>
  );
}
