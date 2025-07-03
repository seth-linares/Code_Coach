// src/components/home/HowItWorksSection.tsx
"use client"

import { motion } from 'framer-motion';
import React from "react";

export const HowItWorksSection = () => {

    const stepVariants = {
        offscreen: {
            y: 50,
            opacity: 0
        },
        onscreen: (i: number) => ({
            y: 0,
            opacity: 1,
            transition: {
                type: "spring",
                bounce: 0.4,
                duration: 0.8,
                delay: i * 0.3  // Sequential delay for each item
            }
        })
    };

    return (
        <div className="py-16 bg-base-100">
            <div className="container mx-auto px-4">
                <h2 className="text-3xl font-bold text-center text-primary mb-12">How It Works</h2>
                <div className="flex flex-col md:flex-row justify-center items-center gap-8">
                    {['Register & Login', 'Choose a Challenge', 'Solve & Improve'].map((step, index) => (
                        <motion.div
                            className="flex-1 text-center p-4 shadow-lg rounded-lg bg-base-200 hover:bg-base-300 transition-colors duration-300"
                            initial="offscreen"
                            whileInView="onscreen"
                            viewport={{ once: true, amount: 0.8 }}
                            variants={stepVariants}
                            custom={index}
                            key={index}
                        >
                            <h3 className="text-xl font-bold text-primary">{step}</h3>
                            <p className="text-neutral-content">{stepDescriptions[index]}</p>
                        </motion.div>
                    ))}
                </div>
            </div>
        </div>
    );
};

const stepDescriptions = [
    'Start by creating an account and logging in to access personalized features.',
    'Select from a variety of challenges that match your skill level and interests.',
    'Use the platform tools to solve challenges and track your progress over time.'
];
