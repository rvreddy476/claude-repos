import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Chat System POC",
  description: "Real-time chat system using SignalR, Next.js, and React",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
