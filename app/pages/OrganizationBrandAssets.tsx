"use client";

import { FormEvent, useRef, useState } from "react";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { brandingApi, type BrandAsset } from "../features/organizations/brandingApi";
import "../brand-assets.css";

const assetTypes = ["Logo", "Watermark", "Background", "Frame", "Icon", "Pattern", "Decoration"];
const formatSize = (bytes: number) =>
  bytes >= 1048576 ? `${(bytes / 1048576).toFixed(1)} MB` : `${(bytes / 1024).toFixed(1)} KB`;

export default function OrganizationBrandAssets() {
  const { id = "" } = useParams();
  const queryClient = useQueryClient();
  const formRef = useRef<HTMLFormElement>(null);
  const [error, setError] = useState("");
  const [busyId, setBusyId] = useState("");
  const [fileName, setFileName] = useState("");
  const assets = useQuery({
    queryKey: ["brand-assets", id],
    queryFn: () => brandingApi.assets(id),
    enabled: !!id,
  });

  const refresh = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: ["brand-assets", id] }),
      queryClient.invalidateQueries({ queryKey: ["storage-dashboard", id] }),
    ]);

  async function upload(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    const form = new FormData(event.currentTarget);
    const file = form.get("file");
    if (!(file instanceof File) || !file.size) {
      setError("Choose an image to upload.");
      return;
    }
    try {
      await brandingApi.uploadAsset(
        id,
        String(form.get("name")),
        String(form.get("type")),
        String(form.get("tags")),
        file,
      );
      formRef.current?.reset();
      setFileName("");
      await refresh();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Upload failed.");
    }
  }

  async function moveToTrash(asset: BrandAsset) {
    if (!window.confirm(`Move “${asset.name}” to trash?`)) return;
    setBusyId(asset.id);
    setError("");
    try {
      await brandingApi.deleteAsset(id, asset.id);
      queryClient.setQueryData<BrandAsset[]>(
        ["brand-assets", id],
        (current = []) => current.filter((item) => item.id !== asset.id),
      );
      await refresh();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Could not move the asset to trash.");
    } finally {
      setBusyId("");
    }
  }

  return (
    <AppShell title="Brand assets" eyebrow="Reusable media library">
      <div className="brand-assets-head">
        <div>
          <p>Keep logos, watermarks, backgrounds, frames, icons and reusable graphics together.</p>
          <span>{assets.data?.length ?? 0} active assets</span>
        </div>
        <Link to={`/organizations/${id}/branding`}>← Back to Brand Kit</Link>
      </div>

      {error && <div className="form-error" role="alert">{error}</div>}

      <form ref={formRef} className="brand-asset-upload" onSubmit={upload}>
        <div className="brand-upload-copy">
          <span>ADD TO LIBRARY</span>
          <h2>Upload a brand asset</h2>
          <p>PNG, JPG, WEBP or SVG. Use descriptive names so your team can find assets quickly.</p>
        </div>
        <label>
          Asset name
          <input name="name" placeholder="e.g. Primary event logo" required />
        </label>
        <label>
          Asset type
          <select name="type">{assetTypes.map((type) => <option key={type}>{type}</option>)}</select>
        </label>
        <label>
          Tags
          <input name="tags" placeholder="event, graduation, dark" />
        </label>
        <label className="brand-file-picker">
          Image file
          <span>
            <b>{fileName || "Choose an image"}</b>
            <small>{fileName ? "Ready to upload" : "PNG, JPG, WEBP or SVG"}</small>
          </span>
          <input
            name="file"
            type="file"
            accept=".png,.jpg,.jpeg,.webp,.svg"
            required
            onChange={(event) => setFileName(event.target.files?.[0]?.name ?? "")}
          />
        </label>
        <button className="brand-upload-button">Upload asset</button>
      </form>

      <section className="brand-library">
        <div className="brand-library-title">
          <div>
            <span>YOUR LIBRARY</span>
            <h2>Active assets</h2>
          </div>
          <p>Assets moved to trash disappear from Brand Kit and Storage metrics.</p>
        </div>
        <div className="brand-asset-grid">
          {assets.isLoading && <p className="brand-assets-state">Loading assets…</p>}
          {assets.data?.map((asset) => (
            <article key={asset.id}>
              <div className="brand-asset-preview">
                <img src={asset.fileUrl} alt={asset.name} />
                <span>{asset.type}</span>
              </div>
              <div className="brand-asset-details">
                <strong>{asset.name}</strong>
                <small>{formatSize(asset.fileSize)} · Version {asset.version}</small>
                {asset.tags && <p>{asset.tags}</p>}
              </div>
              <div className="brand-asset-actions">
                <button type="button" onClick={() => void brandingApi.downloadAsset(id, asset)}>
                  Download
                </button>
                <button
                  type="button"
                  className="danger"
                  disabled={busyId === asset.id}
                  onClick={() => void moveToTrash(asset)}
                >
                  {busyId === asset.id ? "Moving…" : "Move to trash"}
                </button>
              </div>
            </article>
          ))}
          {!assets.isLoading && !assets.data?.length && (
            <div className="brand-assets-state">
              <b>Your asset library is empty</b>
              <p>Upload your first logo or graphic using the form above.</p>
            </div>
          )}
        </div>
      </section>
    </AppShell>
  );
}
