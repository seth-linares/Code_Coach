import type { Metadata } from "next";
import React from "react";


export const metadata: Metadata = {
  title: "Login",
  description: "Login page for CodeCoach",
};

export default function LoginLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <div className={"login-layout"}>
      {children}
    </div>
  );
}
