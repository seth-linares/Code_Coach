import type { Metadata } from "next";
import React from "react";

export const metadata: Metadata = {
  title: "Confirming Email",
  description: "Page to handle email confirmations",
};

export default function ConfirmEmailLayout({
 children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
      <div className={"confirm-email-layout"}>
        {children}
      </div>
  );
}