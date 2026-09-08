"use client";

import { Navigate, NavLink } from "react-router-dom";
import { Brand } from "../components/layout/AppShell";
import { SparkleIcon } from "../components/ui/Icons";
import { useAppStore } from "../store/useAppStore";

const portraits = [
  "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?auto=format&fit=crop&w=700&q=85",
  "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=700&q=85",
  "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=700&q=85",
];

export default function Landing() {
  const authenticated = useAppStore((state) => state.authenticated);
  if (authenticated) return <Navigate to="/dashboard" replace />;

  return (
    <div className="landing dark">
      <nav className="landing-nav"><Brand /><div className="landing-links"><a href="#features">Features</a><NavLink to="/pricing">Pricing</NavLink><NavLink to="/gallery">Gallery</NavLink></div><div><NavLink to="/dashboard" className="text-btn">Log in</NavLink><NavLink to="/camera" className="nav-cta">Create for free <span>→</span></NavLink></div></nav>
      <main className="hero">
        <div className="aurora a1" /><div className="aurora a2" />
        <div className="hero-copy"><div className="pill"><SparkleIcon /> AI photography, reimagined</div><h1>Turn any moment into a <em>masterpiece.</em></h1><p>Your personal AI photo studio. Capture, edit, and transform ordinary shots into extraordinary portraits in seconds.</p><div className="hero-actions"><NavLink to="/camera" className="hero-cta">Open photo booth <span>→</span></NavLink><NavLink to="/gallery" className="hero-secondary">Explore gallery</NavLink></div><div className="trusted"><div className="mini-avatars">{portraits.map((x,i)=><img key={x} src={x} alt="" style={{zIndex: 4-i}} />)}</div><div><span>★★★★★</span><p>Loved by 12,000+ creators</p></div></div></div>
        <div className="hero-visual"><div className="visual-glow" /><div className="portrait-stack"><div className="portrait-side left"><img src={portraits[0]} alt="Professional AI portrait" /></div><div className="portrait-main"><div className="live-badge"><i /> AI GENERATED</div><img src={portraits[1]} alt="Colorful AI portrait" /><div className="scan-line" /></div><div className="portrait-side right"><img src={portraits[2]} alt="Cinematic AI portrait" /></div></div><div className="floating-chip chip-one"><span>✦</span><div><strong>Studio quality</strong><small>In under 10 seconds</small></div></div><div className="floating-chip chip-two"><span>✓</span><div><strong>Portrait ready</strong><small>4K export</small></div></div></div>
      </main>
      <section id="features" className="feature-strip"><span>POWERED BY</span><strong>Neural capture</strong><i /> <strong>Smart relighting</strong><i /> <strong>Generative styles</strong><i /> <strong>4K enhancement</strong></section>
    </div>
  );
}
