import type { Metadata } from "next";
import React from "react";

export const metadata: Metadata = {
  title: "Join CodeCoach!",
  description: "Registration page for CodeCoach",
};

export default function JoinLayout({
 children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
      <div className={"join-layout"}>
        {children}
      </div>
  );
}