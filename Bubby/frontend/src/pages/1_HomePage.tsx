import React from "react";
import {Link} from "react-router-dom"; // named export, so we use {}

function HomePage() {

        return (
            <div>
                <p>Home Page</p>

                <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
                    <Link className="navbar-brand" to="/login">Login</Link>
                </nav>
            </div>

        )
}


// this says that when you do "import [obj] from [file];"
// the obj it will import is what we specify at the end of the file as the "default"
export default HomePage; // here obj = HomePage which is a function
// so to use we will do "import HomePage from ./pages/HomePage;"
