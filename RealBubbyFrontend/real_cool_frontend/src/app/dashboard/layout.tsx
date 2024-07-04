import type { Metadata } from "next";
import React from "react";

export const metadata: Metadata = {
  title: "Dashboard",
  description: "Dashboard for CodeCoach",
};

export default function DashboardLayout({
 children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <div className={"dashboard-layout"}>
      {children}
    </div>
  );
}
