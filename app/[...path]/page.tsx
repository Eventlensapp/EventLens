"use client";

import dynamic from "next/dynamic";

// Serve the same React application shell for every browser-entered URL.
// React Router then resolves /events, /organizations, and the other client routes.
const App = dynamic(() => import("../App"), { ssr: false });

export default function ClientRoute() {
  return <App />;
}
