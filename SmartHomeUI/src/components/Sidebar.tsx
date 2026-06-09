import { useNavigate, useLocation } from "react-router-dom";

function Sidebar() {
  const navigate = useNavigate();
  const location = useLocation();

  const menuItems = [
    { name: "Dashboard", path: "/dashboard" },
    { name: "Devices", path: "/devices" },
    { name: "Settings", path: "/settings" },
  ];

  return (
    <div className="w-64 bg-gray-900 text-white flex flex-col p-6 shadow-lg">

      <h2 className="text-2xl font-bold mb-10 tracking-wide">
        SmartHome
      </h2>

      <nav className="flex flex-col gap-2">
        {menuItems.map((item) => {
          const isActive = location.pathname === item.path;

          return (
            <button
              key={item.name}
              onClick={() => navigate(item.path)}
              className={`text-left px-4 py-2 rounded-lg transition ${
                isActive
                  ? "bg-gray-800"
                  : "hover:bg-gray-700"
              }`}
            >
              {item.name}
            </button>
          );
        })}
      </nav>

    </div>
  );
}

export default Sidebar;