import type { Metadata } from "next";
import React from "react";

export const metadata: Metadata = {
  title: "Profile",
  description: "Profile for user on CodeCoach",
};

export default function ProfileLayout({
 children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <div className={"profile-layout"}>
      {children}
    </div>
  );
}
