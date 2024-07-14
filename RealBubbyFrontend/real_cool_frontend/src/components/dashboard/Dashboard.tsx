import Link from "next/link";

export function Dashboard() {
    return (
        <div className="flex-grow bg-base-300 flex flex-col items-center justify-start py-20 pl-10">
            <h1 className="text-5xl font-bold mb-4 py-10">Choose a Topic to Get Started</h1>
            <div className="flex flex-row items-center">
                <div className="grid grid-rows-1 md:grid-rows-2 lg:grid-rows-3 gap-4 px-2">
                    <div className="flex flex-col items-center p-10 rounded-lg">
                        <h2 className="text-4xl text-primary font-bold mb-2">Warmup</h2>
                        <p className="text-2xl mb-4 text-balance">Simple warmup problems to get started</p>
                    </div>

                    <div className="flex flex-col items-center p-10 rounded-lg">
                        <h2 className="text-4xl text-primary font-bold mb-2"> Strings</h2>
                        <p className="text-2xl mb-4">Basic string problems, no loops</p>
                    </div>

                    <div className="flex flex-col items-center p-10 rounded-lg">
                        <h2 className="text-4xl text-primary font-bold mb-2">Logic</h2>
                        <p className="text-2xl mb-4">Basic boolean logic puzzles -- if else and or not</p>
                    </div>

                    <div className="flex flex-col items-center p-10 rounded-lg">
                        <h2 className="text-4xl text-primary font-bold mb-2">Lists</h2>
                        <p className="text-2xl mb-4">Difficult List Problems</p>
                    </div>
                </div>

                    <div className="grid grid-rows-1 md:grid-rows-2 lg:grid-rows-3 gap-4 px-2">
                        <div className="flex flex-col items-center bg-base-100 p-4 rounded-lg shadow-md">
                            <h2 className="text-4xl text-primary font-bold mb-2">Difficulty: <span
                                className="text-green-500">
                            Easy
                            </span>
                            </h2>
                            <p className="text-2xl mb-4">Total Problems: 21</p>
                            <button className="btn">Get Started</button>
                        </div>

                        <div className="flex flex-col items-center bg-base-100 p-4 rounded-lg shadow-md">
                            <h2 className="text-4xl text-primary font-bold mb-2">Difficulty: <span
                                className="text-yellow-500">
                            Medium
                            </span>
                            </h2>
                            <p className="text-2xl mb-4">Total Problems: 11</p>
                            <button className="btn">Get Started</button>
                        </div>

                        <div className="flex flex-col items-center bg-base-100 p-4 rounded-lg shadow-md">
                            <h2 className="text-4xl text-primary font-bold mb-2">Difficulty: <span
                                className="text-yellow-500">
                            Medium
                            </span>
                            </h2>
                            <p className="text-2xl mb-4">Total Problems: 11</p>
                            <button className="btn">Get Started</button>
                        </div>

                        <div className="flex flex-col items-center bg-base-100 p-4 rounded-lg shadow-md">
                            <h2 className="text-4xl text-primary font-bold mb-2">Difficulty: <span
                                className="text-red-500">
                            Hard
                            </span>
                            </h2>
                            <p className="text-2xl mb-4">Total Problems: 12</p>
                            <Link className={"btn"} href={"/problems"}>Get Started</Link>
                            {/*<button className="btn">Get Started</button>*/}
                        </div>
                    </div>
                </div>
            </div>
            );
            }