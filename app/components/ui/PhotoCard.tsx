type PhotoCardProps = { image: string; title?: string; tag?: string; tall?: boolean };

export function PhotoCard({ image, title, tag, tall }: PhotoCardProps) {
  return (
    <article className={`photo-card ${tall ? "tall" : ""}`}>
      <img src={image} alt={title ?? "AI generated portrait"} />
      {(title || tag) && <div className="photo-meta"><div><strong>{title}</strong><span>{tag}</span></div><button aria-label="More options">•••</button></div>}
    </article>
  );
}
