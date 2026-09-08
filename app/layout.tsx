import type { Metadata } from "next";
import { headers } from "next/headers";
import "./globals.css";

export async function generateMetadata(): Promise<Metadata> {
  const requestHeaders = await headers();
  const host = requestHeaders.get("x-forwarded-host") ?? requestHeaders.get("host") ?? "localhost:3000";
  const protocol = requestHeaders.get("x-forwarded-proto") ?? (host.startsWith("localhost") ? "http" : "https");
  const origin = `${protocol}://${host}`;
  return {
    title: "Prism AI — Event Photo Booth Studio",
    description: "Capture multi-shot photo strips, enhance portraits, manage events, and share live QR galleries.",
    icons: { icon: "/favicon.svg", shortcut: "/favicon.svg" },
    openGraph: {
      title: "Prism AI — Capture. Create. Share.",
      description: "A complete AI-powered photo booth studio for modern events.",
      images: [{ url: `${origin}/og.png`, width: 1536, height: 907, alt: "Prism AI photo booth studio" }],
    },
    twitter: { card: "summary_large_image", title: "Prism AI", description: "Capture. Create. Share.", images: [`${origin}/og.png`] },
  };
}

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return <html lang="en"><body>{children}</body></html>;
}
