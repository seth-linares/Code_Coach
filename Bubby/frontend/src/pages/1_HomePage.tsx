import React from "react";
import { Fragment } from "react";
import {Link} from "react-router-dom";

import {
    Disclosure,
    DisclosureButton,
    DisclosurePanel,
    Menu,
    MenuButton,
    MenuItem,
    MenuItems,
    Transition,
} from '@headlessui/react'

import { Bars3Icon, BellIcon, XMarkIcon } from '@heroicons/react/24/outline'

import logo from "../logo.svg"; // named export, so we use {}

function HomePage() {

        return (
            <div>

                <div className="bg-amber-600">
                    <Link className="text-red-800" to="/login">Login</Link>
                </div>


                <header className="App-header">
                    <img src={logo} className="App-logo" alt="logo"/>
                    <h3 className="text-3xl font-bold underline text-orange-300">
                        Hello world!
                    </h3>
                    <p>Home Page</p>
                    <p>
                        Edit <code>src/App.tsx</code> and save to reload.
                    </p>
                    <a
                        className="App-link"
                        href="https://reactjs.org"
                        target="_blank"
                        rel="noopener noreferrer"
                    >
                        Learn React
                    </a>
                </header>
            </div>

        )
}


// this says that when you do "import [obj] from [file];"
// the obj it will import is what we specify at the end of the file as the "default"
export default HomePage; // here obj = HomePage which is a function
// so to use we will do "import HomePage from ./pages/HomePage;"
