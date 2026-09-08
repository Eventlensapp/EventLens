# EventLens AI Photo Processing

## Architecture

The frontend uses a JSON document made of ordered layers. `CanvasRenderer`
renders the same document for interactive preview and PNG/JPEG output:

1. document background
2. photo and shape layers
3. frame layers
4. text layers
5. sticker layers
6. logo and watermark layers

Every layer carries position, dimensions, rotation, opacity, visibility, and
type-specific properties. The editor store adds selection, undo history,
duplication, deletion, and ordering without coupling those actions to React
components.

Large editor code is lazy-loaded. Source images are decoded once through an
image promise cache. High-resolution exports render to a separate canvas so the
interactive canvas can remain scaled for mobile hardware.

## Backend boundaries

- `IStorageService` isolates local storage from future S3, Azure Blob, or R2
  implementations.
- `IImageProcessingService` isolates ImageSharp from application workflows.
- `PhotoProcessingService` enforces event/organization membership before
  reading, processing, exporting, or reporting status.
- Templates may only be changed by organization owners, managers, or editors.
- Local storage normalizes paths and rejects traversal and unsupported output
  extensions.

The browser owns rich layer composition. The server provides deterministic
resize, crop, compression, preset export, persistence state, and storage.

## API

```text
GET    /api/templates
POST   /api/templates
PUT    /api/templates/{id}
DELETE /api/templates/{id}

GET  /api/photos/{id}
POST /api/photos
POST /api/photos/process
POST /api/photos/export

POST /api/photo-processing/render
GET  /api/photo-processing/status/{id}
```

All endpoints use the existing JWT authentication and `ApiResponse<T>` format.
Image payloads are not accepted implicitly; the photo is first registered with
an explicit storage key.

## Example template

```json
{
  "width": 1200,
  "height": 1800,
  "background": "#F8F1E8",
  "layout": "Polaroid",
  "padding": 54,
  "spacing": 24,
  "borderRadius": 20,
  "borderColor": "#7B4C3A",
  "shadow": true,
  "layers": [
    {
      "id": "photo-1",
      "type": "photo",
      "order": 10,
      "x": 90,
      "y": 90,
      "width": 1020,
      "height": 1280,
      "rotation": 0,
      "opacity": 1,
      "properties": { "fit": "cover", "sourcePhotoIndex": 0 }
    },
    {
      "id": "title",
      "type": "text",
      "order": 30,
      "x": 150,
      "y": 1450,
      "width": 900,
      "height": 100,
      "rotation": 0,
      "opacity": 1,
      "properties": {
        "text": "Rahul & Priya",
        "fontFamily": "Georgia",
        "fontSize": 64,
        "align": "center",
        "color": "#7B4C3A",
        "shadow": false,
        "stroke": "#FFFFFF",
        "strokeWidth": 0
      }
    },
    {
      "id": "event-logo",
      "type": "logo",
      "order": 60,
      "x": 500,
      "y": 1660,
      "width": 200,
      "height": 80,
      "rotation": 0,
      "opacity": 0.9,
      "properties": { "source": "/storage/branding/event-logo.png" }
    }
  ]
}
```

The `PhotoProcessingAndTemplates` EF migration upgrades photos, adds template
ownership, and creates the associated indexes and relationships.
