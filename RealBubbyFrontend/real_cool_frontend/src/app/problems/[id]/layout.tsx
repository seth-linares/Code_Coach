import type { Metadata } from "next";
import React from "react";

export const metadata: Metadata = {
    title: "Problems",
    description: "Problems for CodeCoach",
};

export default function ProblemsLayout({
                                            children,
                                        }: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <div className={"problems-layout"}>
            {children}
        </div>
    );
}
