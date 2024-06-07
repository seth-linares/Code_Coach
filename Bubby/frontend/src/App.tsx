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
import HistoryPage from "./pages/7_HistoryPage";
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


        */}

        <Router>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path={"/login"} element={<LoginPage />} />
                <Route path={"/register"} element={<RegistrationPage />} />
                <Route path={"/profile"} element={<ProfilePage />} />
                <Route path={"/problems"} element={<ProblemsPage />} />
                <Route path={"/problems/:id"} element={<CodingPage />} />
                <Route path={"/setup-2fa"} element={<MfaPage />} />
                <Route path={"/history"} element={<HistoryPage />} />
                <Route path={"/help"} element={<HelpPage />} />
                <Route path={"/resources"} element={<ResourcesPage />} />
                <Route path={"/settings"} element={<SettingsPage />} />
            </Routes>
        </Router>


      <h1 className="text-3xl font-bold underline text-orange-300">
      Hello world!
      </h1>
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
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
  );
}

export default App;
