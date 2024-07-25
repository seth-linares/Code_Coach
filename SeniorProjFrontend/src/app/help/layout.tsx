import type { Metadata } from "next";
import React from "react";

export const metadata: Metadata = {
  title: "Help",
  description: "Help/FAQ Page for CodeCoach",
};

export default function HelpLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <div className={"help-layout"}>
      {children}
    </div>
  );
}
