# EventLens AI Studio

## Provider-independent architecture

Application code depends only on `IAIProvider`. Provider routing is handled by
`IAIProviderSelection`, and provider lookup/capability checks are handled by
`IAIProviderResolver`. No application service references a provider SDK.

`HttpAIProvider` is the example adapter. It sends the normalized
`AIProviderRequest` contract to a configured HTTP endpoint and maps the response
to `AIProviderResult`. It can front OpenAI, Stability AI, Gemini, Azure OpenAI,
ComfyUI, a local Stable Diffusion gateway, or a future provider without changing
the queue or application service.

```json
{
  "AI": {
    "DefaultProvider": "ProductionGateway",
    "Routing": {
      "BackgroundRemoval": "FastBackgroundProvider",
      "StyleTransfer": "ProductionGateway"
    },
    "Providers": {
      "ProductionGateway": {
        "Enabled": true,
        "Endpoint": "https://ai-gateway.example/v1/process",
        "ApiKey": "supplied-by-secret-manager",
        "TimeoutSeconds": 120,
        "SupportedJobTypes": [
          "BackgroundRemoval",
          "StyleTransfer",
          "FaceEnhancement"
        ]
      }
    }
  }
}
```

Secrets must be supplied through environment configuration or a secret manager.
The checked-in example provider is disabled.

## Pipeline

1. The API validates the request and event ownership.
2. Subscription quota is checked for the organization.
3. An `AIJob` is persisted and placed on the bounded channel queue.
4. `AIJobWorker` resolves a capable configured provider.
5. SignalR publishes started and progress events to the authorized event group.
6. The provider returns a storage URL and optional metadata.
7. The job and related photo are updated in one unit of work.
8. Completion or failure is broadcast. Transient failures use exponential
   backoff for up to three attempts.

Prompt definitions are stored in `ai_prompt_definitions`, keeping prompt changes
out of provider and application code.

## API and realtime

```text
POST /api/ai/background
POST /api/ai/style
POST /api/ai/enhance
POST /api/ai/upscale
POST /api/ai/props
POST /api/ai/magic-erase
POST /api/ai/generative-fill
POST /api/ai/template
POST /api/ai/smart-selection
POST /api/ai/memory-book
GET  /api/ai/backgrounds
POST /api/ai/backgrounds
GET  /api/ai/jobs
GET  /api/ai/jobs/{id}
POST /api/ai/jobs/{id}/retry
```

SignalR is available at `/hubs/ai-jobs` and emits `JobStarted`, `JobProgress`,
`JobCompleted`, and `JobFailed`. Clients subscribe to an event-specific group.

## Frontend

The lazy-loaded AI Studio routes are:

```text
/ai
/ai/editor
/ai/backgrounds
/ai/styles
/ai/jobs
/ai/history
```

Zustand owns editor choices and undo/redo state. React Query owns remote job
state. SignalR invalidates relevant job queries when progress changes.

The `AIStudio` migration adds jobs, editable prompts, organization backgrounds,
indexes, and relationships. AI features remain inactive until at least one
provider is enabled through configuration.
