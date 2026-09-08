"use client";
import{Navigate}from"react-router-dom";import{useAppStore}from"../store/useAppStore";
export default function StructureRedirect({unit}:{unit:"departments"|"branches"}){const id=useAppStore(x=>x.activeOrganizationId);return id?<Navigate to={`/organizations/${id}/${unit}`} replace/>:<Navigate to="/organizations" replace/>}
