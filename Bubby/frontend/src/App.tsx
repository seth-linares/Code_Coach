import React from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import logo from './logo.svg';
import './App.css';
import HomePage from './pages/1_HomePage';
import LoginPage from "./pages/2_LoginPage";
import RegistrationPage from "./pages/3_RegistrationPage";
import ProfilePage from "./pages/4_ProfilePage";
import ProblemsPage from "./pages/5_ProblemsPage";
import CodingPage from "./pages/6_CodingPage";
import SolutionsPage from "./pages/7_SolutionsPage";
import MfaPage from "./pages/8_MfaPage";
import HelpPage from "./pages/9_HelpPage";
import ResourcesPage from "./pages/10_ResourcesPage";
import SettingsPage from "./pages/11_SettingsPage";




function App() {
  return (

    <div className="App">

        {/*

        So far we know we need dynamic id's for the problems (/problems/:id)

        I believe we can use JWT tokens to handle loading which profile needs to be loaded to a user
        JWT tokens should store the user info so that would suffice

        We MIGHT need to use id's for solutions (/solutions/:id) but it depends on how loading that info works
        Code Wars just does this:
            - Finished solutions: https://www.codewars.com/users/cat-coding/completed_solutions
            - Unfinished solutions: https://www.codewars.com/users/cat-coding/unfinished_solutions
            - Obsolete solutions: https://www.codewars.com/users/cat-coding/obsolete_solutions

        Seems like the approach will be using 1 Solutions page and then use tabs to traverse rather than 3 links

        */}

        <Router>
            <Routes>
                <Route path={"/"} element={<HomePage />} />
                <Route path={"/login"} element={<LoginPage />} />
                <Route path={"/register"} element={<RegistrationPage />} />
                <Route path={"/profile"} element={<ProfilePage />} />
                <Route path={"/problems"} element={<ProblemsPage />} />
                <Route path={"/problems/:id"} element={<CodingPage />} />
                <Route path={"/setup-2fa"} element={<MfaPage />} />
                <Route path={"/solutions"} element={<SolutionsPage />} />
                <Route path={"/help"} element={<HelpPage />} />
                <Route path={"/resources"} element={<ResourcesPage />} />
                <Route path={"/settings"} element={<SettingsPage />} />
            </Routes>
        </Router>
    </div>
  );
}

export default App;
