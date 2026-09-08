"use client";import{Navigate}from"react-router-dom";import{useAppStore}from"../store/useAppStore";
export default function TeamRedirect(){const id=useAppStore(s=>s.activeOrganizationId);return <Navigate to={id?`/organizations/${id}/team`:"/organizations"} replace/>}
