import './App.css'
import { Routes, Route } from "react-router-dom";
import Dashboard from "./pages/Dashboard";
import Login from "./pages/Login";
import { useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";


function App() {
  
const navigate = useNavigate();

  useEffect(() => {
    const refresh = async () => {
      try {
        await axios.post(
          `${import.meta.env.VITE_API_BASE_URL}/api/Auth/refresh`,
          {},
          { withCredentials: true }
        );
      } catch {
        navigate("/login");
      }
    };

    refresh();
  }, []);
  
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/login" element={<Login />} />
      <Route path="/dashboard" element={<Dashboard />} />
    </Routes>
  );
}

export default App;