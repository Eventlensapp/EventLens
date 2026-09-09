"use client";

import { BrowserRouter, Navigate, Route, Routes, useLocation } from "react-router-dom";
import { lazy, Suspense, type ReactNode } from "react";
import "./dark-theme.css";
import Landing from "./pages/Landing";
import Dashboard from "./pages/Dashboard";
import Camera from "./pages/Camera";
import PhotoEditor from "./pages/PhotoEditor";
import Gallery from "./pages/Gallery";
import Pricing from "./pages/Pricing";
import Login from "./pages/Login";
import Register from "./pages/Register";
import AccountSettings from "./pages/AccountSettings";
import {ForgotPassword,ResetPassword,VerifyEmail} from "./pages/AccountRecovery";
import SessionBridge from "./components/SessionBridge";
import Events from "./pages/Events";
import EventCreate from "./pages/EventCreate";
import EventDashboard from "./pages/EventDashboard";
import EventSettings from "./pages/EventSettings";
import EventTeam from "./pages/EventTeam";
import EventTypes from "./pages/EventTypes";
import EventTemplates from "./pages/EventTemplates";
import EventTemplateEditor from "./pages/EventTemplateEditor";
import EventExperienceWorkspace from "./pages/EventExperienceWorkspace";
import EventOperations from "./pages/EventOperations";
import EventOperationsManagement from "./pages/EventOperationsManagement";
import EventPublicAccessAdmin from "./pages/EventPublicAccessAdmin";
import PublicEvent from "./pages/PublicEvent";
import Organizations from "./pages/Organizations";
import OrganizationCreate from "./pages/OrganizationCreate";
import OrganizationSettings from "./pages/OrganizationSettings";
import OrganizationTeam from "./pages/OrganizationTeam";
import InvitationAccept from "./pages/InvitationAccept";
import TeamRedirect from "./pages/TeamRedirect";
import OrganizationSecurity from "./pages/OrganizationSecurity";
import SecurityRedirect from "./pages/SecurityRedirect";
import RolesDashboard from "./pages/RolesDashboard";
import OrganizationDepartments from "./pages/OrganizationDepartments";
import OrganizationBranches from "./pages/OrganizationBranches";
import StructureRedirect from "./pages/StructureRedirect";
import OrganizationBranding from "./pages/OrganizationBranding";
import OrganizationBrandAssets from "./pages/OrganizationBrandAssets";
import OrganizationBrandThemes from "./pages/OrganizationBrandThemes";
import {StorageDashboardPage,StorageFilesPage,StorageFoldersPage,StorageUploadsPage,StorageTrashPage} from "./pages/StoragePages";
import {AdminPlans,AdminPlanCreate,AdminPlanDetails,OrganizationSubscriptionPage,OrganizationUsagePage} from "./pages/EntitlementPages";
import BoothLanding from "./pages/BoothLanding";
import BoothSession from "./pages/BoothSession";
import BoothPreview from "./pages/BoothPreview";
import BoothResult from "./pages/BoothResult";
import BoothFoundation from "./pages/BoothFoundation";
import BoothCamera from "./pages/BoothCamera";
import BoothSessionEngine from "./pages/BoothSessionEngine";
import BoothCapture from "./pages/BoothCapture";
import BoothCameraControls from "./pages/BoothCameraControls";
import BoothCaptureModes from "./pages/BoothCaptureModes";
import ProfessionalCameras from "./pages/ProfessionalCameras";
import PhotoTemplates from "./pages/PhotoTemplates";
import GuestBoothExperience from "./pages/GuestBoothExperience";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
const queryClient = new QueryClient({ defaultOptions: { queries: { staleTime: 30_000, refetchOnWindowFocus: false } } });
const PhotoProcessingEditor = lazy(() => import("./features/photo-processing/editor/PhotoProcessingEditor"));
const AIStudio = lazy(() => import("./features/ai-studio/AIStudio"));
const CRM = lazy(() => import("./features/crm/CRM"));
const AnalyticsDashboard = lazy(() => import("./features/analytics/AnalyticsDashboard"));
const BillingPortal = lazy(() => import("./features/billing/BillingPortal"));
import { useAppStore } from "./store/useAppStore";
import { canAccessPath } from "./lib/access";
import AccessDenied from "./pages/AccessDenied";
import ChangeTemporaryPassword from "./pages/ChangeTemporaryPassword";
const emptyRoles: string[] = [];

function Protected({ children }: { children: ReactNode }) {
  const authenticated = useAppStore((state) => state.authenticated);
  const roles = useAppStore((state) => state.user?.roles ?? emptyRoles);
  const mustChangePassword = useAppStore((state) => state.user?.mustChangePassword ?? false);
  const location = useLocation();
  if (!authenticated) return <Navigate to="/login" replace />;
  if (mustChangePassword && location.pathname !== "/change-password") return <Navigate to="/change-password" replace />;
  return canAccessPath(location.pathname, roles) ? children : <Navigate to="/forbidden" replace />;
}
function BrandingRedirect(){const id=useAppStore(x=>x.activeOrganizationId);return id?<Navigate to={`/organizations/${id}/branding`} replace/>:<Navigate to="/organizations" replace/>}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}><BrowserRouter><SessionBridge />
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password" element={<ResetPassword />} />
        <Route path="/verify-email" element={<VerifyEmail />} />
        <Route path="/change-password" element={<Protected><ChangeTemporaryPassword /></Protected>} />
        <Route path="/account" element={<Protected><AccountSettings /></Protected>} />
        <Route path="/dashboard" element={<Protected><Dashboard /></Protected>} />
        <Route path="/forbidden" element={<Protected><AccessDenied /></Protected>} />
        <Route path="/events" element={<Protected><Events /></Protected>} />
        <Route path="/events/create" element={<Protected><EventCreate /></Protected>} />
        <Route path="/events/:id" element={<Protected><EventDashboard /></Protected>} />
        <Route path="/events/:id/settings" element={<Protected><EventSettings /></Protected>} />
        <Route path="/events/:id/team" element={<Protected><EventTeam /></Protected>} />
        <Route path="/event-types" element={<Protected><EventTypes /></Protected>} />
        <Route path="/event-templates" element={<Protected><EventTemplates /></Protected>} />
        <Route path="/templates" element={<Protected><PhotoTemplates /></Protected>} />
        <Route path="/event-templates/create" element={<Protected><EventTemplateEditor /></Protected>} />
        <Route path="/event-templates/:id" element={<Protected><EventTemplateEditor /></Protected>} />
        <Route path="/events/:id/branding" element={<Protected><EventExperienceWorkspace view="branding" /></Protected>} />
        <Route path="/events/:id/experience" element={<Protected><EventExperienceWorkspace view="experience" /></Protected>} />
        <Route path="/events/:id/assets" element={<Protected><EventExperienceWorkspace view="assets" /></Protected>} />
        <Route path="/events/:id/sponsors" element={<Protected><EventExperienceWorkspace view="sponsors" /></Protected>} />
        <Route path="/events/:id/venue" element={<Protected><EventOperations view="venue" /></Protected>} />
        <Route path="/events/:id/schedule" element={<Protected><EventOperations view="schedule" /></Protected>} />
        <Route path="/events/:id/operations" element={<Protected><EventOperationsManagement view="operations" /></Protected>} />
        <Route path="/events/:id/checklist" element={<Protected><EventOperationsManagement view="checklist" /></Protected>} />
        <Route path="/events/:id/staff" element={<Protected><EventOperationsManagement view="staff" /></Protected>} />
        <Route path="/events/:id/placements" element={<Protected><EventOperationsManagement view="placements" /></Protected>} />
        <Route path="/events/:id/qr" element={<Protected><EventPublicAccessAdmin view="qr" /></Protected>} />
        <Route path="/events/:id/access" element={<Protected><EventPublicAccessAdmin view="access" /></Protected>} />
        <Route path="/e/:token" element={<PublicEvent />} />
        <Route path="/organizations" element={<Protected><Organizations /></Protected>} />
        <Route path="/organizations/create" element={<Protected><OrganizationCreate /></Protected>} />
        <Route path="/organizations/:id/settings" element={<Protected><OrganizationSettings /></Protected>} />
        <Route path="/organizations/:id/team" element={<Protected><OrganizationTeam /></Protected>} />
        <Route path="/invitations/:token" element={<InvitationAccept />} />
        <Route path="/team" element={<Protected><TeamRedirect /></Protected>} />
        <Route path="/organizations/:id/security" element={<Protected><OrganizationSecurity /></Protected>} />
        <Route path="/organizations/:id/departments" element={<Protected><OrganizationDepartments /></Protected>} />
        <Route path="/organizations/:id/branches" element={<Protected><OrganizationBranches /></Protected>} />
        <Route path="/departments" element={<Protected><StructureRedirect unit="departments" /></Protected>} />
        <Route path="/branches" element={<Protected><StructureRedirect unit="branches" /></Protected>} />
        <Route path="/organizations/:id/branding" element={<Protected><OrganizationBranding /></Protected>} />
        <Route path="/organizations/:id/branding/assets" element={<Protected><OrganizationBrandAssets /></Protected>} />
        <Route path="/organizations/:id/branding/themes" element={<Protected><OrganizationBrandThemes /></Protected>} />
        <Route path="/branding" element={<Protected><BrandingRedirect /></Protected>} />
        <Route path="/storage" element={<Protected><StorageDashboardPage /></Protected>} />
        <Route path="/storage/files" element={<Protected><StorageFilesPage /></Protected>} />
        <Route path="/storage/folders" element={<Protected><StorageFoldersPage /></Protected>} />
        <Route path="/storage/uploads" element={<Protected><StorageUploadsPage /></Protected>} />
        <Route path="/storage/trash" element={<Protected><StorageTrashPage /></Protected>} />
        <Route path="/admin/plans" element={<Protected><AdminPlans /></Protected>} />
        <Route path="/admin/plans/create" element={<Protected><AdminPlanCreate /></Protected>} />
        <Route path="/admin/plans/:id" element={<Protected><AdminPlanDetails /></Protected>} />
        <Route path="/organization/subscription" element={<Protected><OrganizationSubscriptionPage /></Protected>} />
        <Route path="/organization/usage" element={<Protected><OrganizationUsagePage /></Protected>} />
        <Route path="/access" element={<Protected><SecurityRedirect /></Protected>} />
        <Route path="/roles" element={<Protected><RolesDashboard /></Protected>} />
        <Route path="/crm" element={<Protected><Suspense fallback={<main className="editor-loading">Loading CRM…</main>}><CRM /></Suspense></Protected>} />
        <Route path="/analytics" element={<Protected><Suspense fallback={<main className="editor-loading">Loading analytics…</main>}><AnalyticsDashboard /></Suspense></Protected>} />
        <Route path="/subscription" element={<Protected><Suspense fallback={<main className="editor-loading">Loading billing…</main>}><BillingPortal view="subscription" /></Suspense></Protected>} />
        <Route path="/billing/history" element={<Protected><Suspense fallback={<main className="editor-loading">Loading billing…</main>}><BillingPortal view="history" /></Suspense></Protected>} />
        <Route path="/billing/invoices" element={<Protected><Suspense fallback={<main className="editor-loading">Loading billing…</main>}><BillingPortal view="invoices" /></Suspense></Protected>} />
        <Route path="/billing/usage" element={<Protected><Suspense fallback={<main className="editor-loading">Loading billing…</main>}><BillingPortal view="usage" /></Suspense></Protected>} />
        <Route path="/billing/payment-methods" element={<Protected><Suspense fallback={<main className="editor-loading">Loading billing…</main>}><BillingPortal view="methods" /></Suspense></Protected>} />
        <Route path="/billing/upgrade" element={<Protected><Suspense fallback={<main className="editor-loading">Loading billing…</main>}><BillingPortal view="upgrade" /></Suspense></Protected>} />
        <Route path="/camera" element={<Protected><Camera /></Protected>} />
        <Route path="/editor" element={<Protected><PhotoEditor /></Protected>} />
        <Route path="/gallery" element={<Protected><Gallery /></Protected>} />
        <Route path="/pricing" element={<Pricing />} />
        <Route path="/booth/:eventSlug" element={<BoothLanding />} />
        <Route path="/booth" element={<Protected><BoothFoundation view="overview" /></Protected>} />
        <Route path="/booth/setup" element={<Protected><BoothFoundation view="setup" /></Protected>} />
        <Route path="/booth/settings" element={<Protected><BoothFoundation view="settings" /></Protected>} />
        <Route path="/booth/diagnostics" element={<Protected><BoothFoundation view="diagnostics" /></Protected>} />
        <Route path="/booth/camera" element={<Protected><BoothCamera /></Protected>} />
        <Route path="/booth/session" element={<Protected><BoothSessionEngine /></Protected>} />
        <Route path="/booth/capture" element={<Protected><BoothCapture /></Protected>} />
        <Route path="/booth/camera-controls" element={<Protected><BoothCameraControls /></Protected>} />
        <Route path="/booth/capture-modes" element={<Protected><BoothCaptureModes /></Protected>} />
        <Route path="/booth/experience" element={<GuestBoothExperience />} />
        <Route path="/booth/professional-cameras" element={<Protected><ProfessionalCameras /></Protected>} />
        <Route path="/session" element={<BoothSession />} />
        <Route path="/preview" element={<BoothPreview />} />
        <Route path="/result" element={<BoothResult />} />
        <Route path="/photo-processing/editor" element={<Protected><Suspense fallback={<main className="editor-loading">Loading editor…</main>}><PhotoProcessingEditor /></Suspense></Protected>} />
        <Route path="/ai" element={<Protected><Suspense fallback={<main className="editor-loading">Loading AI Studio…</main>}><AIStudio view="dashboard"/></Suspense></Protected>} />
        <Route path="/ai/editor" element={<Protected><Suspense fallback={<main className="editor-loading">Loading AI Studio…</main>}><AIStudio view="editor"/></Suspense></Protected>} />
        <Route path="/ai/backgrounds" element={<Protected><Suspense fallback={<main className="editor-loading">Loading AI Studio…</main>}><AIStudio view="backgrounds"/></Suspense></Protected>} />
        <Route path="/ai/styles" element={<Protected><Suspense fallback={<main className="editor-loading">Loading AI Studio…</main>}><AIStudio view="styles"/></Suspense></Protected>} />
        <Route path="/ai/jobs" element={<Protected><Suspense fallback={<main className="editor-loading">Loading AI Studio…</main>}><AIStudio view="jobs"/></Suspense></Protected>} />
        <Route path="/ai/history" element={<Protected><Suspense fallback={<main className="editor-loading">Loading AI Studio…</main>}><AIStudio view="history"/></Suspense></Protected>} />
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter></QueryClientProvider>
  );
}
