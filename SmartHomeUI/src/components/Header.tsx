import api from "../services/api";
import { useNavigate } from "react-router-dom";
import { useState } from "react";

interface Props {
  title?: string;
}

function Header({ title = "Dashboard" }: Props) {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const logout = async () => {
    try {
      setLoading(true);

      await api.post("/api/Auth/logout");

      navigate("/login");
    } catch {
      alert("Logout failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white h-16 shadow-sm flex items-center justify-between px-8 border-b">

      <h2 className="text-xl font-semibold tracking-wide">
        {title}
      </h2>

      <button
        onClick={logout}
        disabled={loading}
        className={`px-4 py-2 rounded-lg shadow text-white transition ${
          loading
            ? "bg-gray-400 cursor-not-allowed"
            : "bg-red-500 hover:bg-red-600"
        }`}
      >
        {loading ? "Logging out..." : "Logout"}
      </button>

    </div>
  );
}

export default Header;