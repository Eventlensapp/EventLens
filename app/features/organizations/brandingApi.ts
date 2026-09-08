import { apiDownload, apiOrigin, apiRequest } from "../../lib/api";

export type BrandKit = {
  id: string;
  organizationId: string;
  logos: Record<string, string | null>;
  colors: Record<string, string>;
  primaryFont: string;
  secondaryFont: string;
  headingFont: string;
  bodyFont: string;
  fontScale: number;
  watermark: { enabled: boolean; imageUrl?: string; opacity: number; position: string; scale: number; margin: number };
  qr: { primaryColor: string; backgroundColor: string; rounded: boolean; frame?: string; logoInCenter: boolean; errorCorrectionLevel: string; defaultSize: number };
  email: { companyName: string; logoUrl?: string; primaryColor: string; footer?: string; socialLinksJson?: string; signature?: string };
};

export type BrandAsset = {
  id: string;
  name: string;
  type: string;
  fileUrl: string;
  mimeType: string;
  fileSize: number;
  tags?: string;
  version: number;
  createdAt: string;
};

export type BrandTheme = {
  id: string;
  name: string;
  description?: string;
  kind: "Theme" | "Preset";
  configurationJson: string;
  isDefault: boolean;
  isActive: boolean;
  createdAt: string;
};

const form = (values: Record<string, string | Blob>) => {
  const payload = new FormData();
  Object.entries(values).forEach(([key, value]) => payload.append(key, value));
  return payload;
};

export const brandingAssetUrl = (url?: string | null) => {
  if (!url || /^(?:https?:|blob:|data:)/i.test(url)) return url ?? null;
  return `${apiOrigin()}${url.startsWith("/") ? url : `/${url}`}`;
};

const normalizeKit = (kit: BrandKit): BrandKit => ({
  ...kit,
  logos: Object.fromEntries(
    Object.entries(kit.logos).map(([key, value]) => [key, brandingAssetUrl(value)]),
  ),
  watermark: {
    ...kit.watermark,
    opacity: kit.watermark.opacity >= 0 && kit.watermark.opacity <= 1
      ? kit.watermark.opacity
      : 0.3,
    scale: kit.watermark.scale > 0 && kit.watermark.scale <= 1
      ? kit.watermark.scale
      : 0.2,
    margin: kit.watermark.margin >= 0 ? kit.watermark.margin : 16,
    imageUrl: brandingAssetUrl(kit.watermark.imageUrl) ?? undefined,
  },
  qr: {
    ...kit.qr,
    defaultSize: kit.qr.defaultSize >= 128 && kit.qr.defaultSize <= 4096
      ? kit.qr.defaultSize
      : 512,
    errorCorrectionLevel: kit.qr.errorCorrectionLevel || "M",
  },
  email: {
    ...kit.email,
    logoUrl: brandingAssetUrl(kit.email.logoUrl) ?? undefined,
  },
});

const normalizeAsset = (asset: BrandAsset): BrandAsset => ({
  ...asset,
  fileUrl: brandingAssetUrl(asset.fileUrl) ?? asset.fileUrl,
});

export const brandingApi = {
  kit: async (organizationId: string) =>
    normalizeKit(await apiRequest<BrandKit>(`/api/organizations/${organizationId}/branding`)),
  save: async (organizationId: string, body: unknown) =>
    normalizeKit(await apiRequest<BrandKit>(`/api/organizations/${organizationId}/branding`, {
      method: "PUT",
      body: JSON.stringify(body),
    })),
  logo: async (organizationId: string, type: string, file: File) =>
    normalizeKit(await apiRequest<BrandKit>(`/api/organizations/${organizationId}/branding/logos/${type}`, {
      method: "POST",
      body: form({ file }),
    })),
  removeLogo: (organizationId: string, type: string) =>
    apiRequest(`/api/organizations/${organizationId}/branding/logos/${type}`, { method: "DELETE" }),
  assets: async (organizationId: string) =>
    (await apiRequest<BrandAsset[]>(`/api/organizations/${organizationId}/branding/assets`)).map(normalizeAsset),
  uploadAsset: async (organizationId: string, name: string, type: string, tags: string, file: File) =>
    normalizeAsset(await apiRequest<BrandAsset>(`/api/organizations/${organizationId}/branding/assets`, {
      method: "POST",
      body: form({ name, type, tags, file }),
    })),
  deleteAsset: (organizationId: string, assetId: string) =>
    apiRequest(`/api/organizations/${organizationId}/branding/assets/${assetId}`, { method: "DELETE" }),
  downloadAsset: (organizationId: string, asset: BrandAsset) =>
    apiDownload(
      `/api/organizations/${organizationId}/branding/assets/${asset.id}/download`,
      {},
      asset.name,
    ),
  themes: (organizationId: string) =>
    apiRequest<BrandTheme[]>(`/api/organizations/${organizationId}/branding/themes`),
  createTheme: (organizationId: string, body: unknown) =>
    apiRequest<BrandTheme>(`/api/organizations/${organizationId}/branding/themes`, { method: "POST", body: JSON.stringify(body) }),
  updateTheme: (organizationId: string, themeId: string, body: unknown) =>
    apiRequest<BrandTheme>(`/api/organizations/${organizationId}/branding/themes/${themeId}`, { method: "PUT", body: JSON.stringify(body) }),
  duplicateTheme: (organizationId: string, themeId: string, name: string) =>
    apiRequest<BrandTheme>(`/api/organizations/${organizationId}/branding/themes/${themeId}/duplicate`, { method: "POST", body: JSON.stringify({ name }) }),
  activateTheme: (organizationId: string, themeId: string) =>
    apiRequest(`/api/organizations/${organizationId}/branding/themes/${themeId}/activate`, { method: "POST" }),
  archiveTheme: (organizationId: string, themeId: string) =>
    apiRequest(`/api/organizations/${organizationId}/branding/themes/${themeId}`, { method: "DELETE" }),
};
