import type { Metadata } from "next";
import React from "react";


export const metadata: Metadata = {
  title: "Resources",
  description: "Resources page for CodeCoach",
};

export default function ResourcesLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
      <div className={"resources-layout"}>
        {children}
      </div>
  );
}
