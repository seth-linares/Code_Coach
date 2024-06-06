import React from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import logo from './logo.svg';
import './App.css';
import HomePage from './pages/HomePage';
import LoginPage from "./pages/LoginPage";
import HelpPage from "./pages/HelpPage";
import ResourcesPage from "./pages/ResourcesPage";

function App() {
  return (


    <div className="App">
        <Router>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path={"/help"} element={<HelpPage />} />
                <Route path={"/resources"} element={<ResourcesPage />} />
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
