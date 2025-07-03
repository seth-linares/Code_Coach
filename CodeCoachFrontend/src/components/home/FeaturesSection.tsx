// src/components/home/FeaturesSection.tsx
import { FaCode, FaBookOpen, FaRobot } from 'react-icons/fa';

export const FeaturesSection = () => {
    return (
        <div className="py-16 bg-gray-300">
            <div className="container mx-auto px-4">
                <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
                    <div className="p-8 rounded-lg shadow-lg bg-base-100 transition duration-300 ease-in-out hover:shadow-2xl hover:scale-105">
                        <FaCode className="text-4xl text-primary mb-3 mx-auto" />
                        <h3 className="text-2xl font-bold mb-3">Challenges</h3>
                        <p className="text-neutral-content">
                            Quickly find and solve coding challenges to improve your algorithmic thinking and coding proficiency.
                        </p>
                    </div>
                    <div className="p-8 rounded-lg shadow-lg bg-base-100 transition duration-300 ease-in-out hover:shadow-2xl hover:scale-105">
                        <FaBookOpen className="text-4xl text-primary mb-3 mx-auto" />
                        <h3 className="text-2xl font-bold mb-3">Learning</h3>
                        <p className="text-neutral-content">
                            Enhance your skills with detailed explanations and real-time feedback as you solve each problem.
                        </p>
                    </div>
                    <div className="p-8 rounded-lg shadow-lg bg-base-100 transition duration-300 ease-in-out hover:shadow-2xl hover:scale-105">
                        <FaRobot className="text-4xl text-primary mb-3 mx-auto" />
                        <h3 className="text-2xl font-bold mb-3">Interactive Learning</h3>
                        <p className="text-neutral-content">
                            Engage with AI-driven coaching to receive personalized advice and insights tailored to your learning curve.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
};
