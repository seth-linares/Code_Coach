import React from "react";
import {Link} from "react-router-dom";
import App from "../App";

function LoginPage() {
    return(
        <div>
            <header>
                <h1>
                    <Link to="/">Home</Link>
                </h1>
            </header>

            <h1>
                Login Page
            </h1>

        </div>
    )
}

export default LoginPage;