import React from "react";
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Spinner } from "../ui/Spinner";
import { useAuth } from "../../contexts/AuthContext";

export const ProtectedRoute: React.FC = () => {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return <div className="min-h-screen flex items-center justify-center bg-slate-100"><Spinner size="lg" /></div>;
  }

  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace state={{ from: location }} />;
};
