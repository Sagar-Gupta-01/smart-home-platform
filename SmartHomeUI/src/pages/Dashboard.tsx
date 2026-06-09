import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import DeviceCard from "../components/DeviceCard"; // make sure this is imported
import Layout from "../components/Layout";

interface Device {
  id: number;
  name: string;
  isOn: boolean;
}

interface Room {
  id: number;
  name: string;
  devices: Device[];
}

interface Location {
  id: number;
  name: string;
}

function Dashboard() {
  const navigate = useNavigate();

  const [locations, setLocations] = useState<Location[]>([]);
  const [selectedLocation, setSelectedLocation] = useState<number | null>(null);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");



  // ✅ Load locations
  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get("/api/Locations");
        setLocations(res.data);

        if (res.data.length > 0) {
          setSelectedLocation(res.data[0].id);
        }
      } catch {
        navigate("/login");
      }
    };

    load();
  }, []);

  // ✅ Load rooms & devices
  useEffect(() => {
    const load = async () => {
      if (!selectedLocation) return;

      setLoading(true);
      setError("");
      try {
        const res = await api.get(`/api/Locations/full?locationId=${selectedLocation}`);
        setRooms(res.data.rooms || []);
      } catch {
        setError("Failed to load this location's data. Please try again.");
        setRooms([]);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [selectedLocation]);   

  return (
  <Layout title="Dashboard">

    {/* Content */}
    <div className="space-y-8">

      {/* Location Selector */}
      
      {locations.length === 0 ? 
        (<div className="text-gray-500">
          No locations available
        </div>) :
        (<div className="bg-white p-6 rounded-xl shadow-sm border w-fit">
        <label className="block text-sm mb-2 font-medium text-gray-600">
          Select Location
        </label>

        <select
          value={selectedLocation ?? ""}
          onChange={(e) => setSelectedLocation(Number(e.target.value))}
          className="border p-2 rounded-lg w-64"
        >
          {locations.map((loc) => (
            <option key={loc.id} value={loc.id}>
              {loc.name}
            </option>
          ))}
        </select>
      </div>)
      }      

      {/* Error */}
      {error && (
        <div className="bg-red-50 text-red-700 border border-red-200 px-4 py-3 rounded-lg" role="alert">
          {error}
        </div>
      )}

      {/* Loading */}
      {loading && (
        <div className="text-center text-gray-500">
          Loading...
        </div>
      )}

      {/* Empty */}
      {!loading && rooms.length === 0 && (
        <div className="text-gray-500">
          No rooms found
        </div>
      )}

      {/* Rooms */}
      {!loading &&
        rooms.map((room) => (
          <div key={room.id} className="space-y-4">

            <h3 className="text-xl font-semibold">
              {room.name}
            </h3>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
              {room.devices.map((device) => (
                <DeviceCard
                  key={device.id}
                  device={device}
                  onToggle={(deviceId) => {
                    setRooms((prev) =>
                      prev.map((room) => ({
                        ...room,
                        devices: room.devices.map((d) =>
                          d.id === deviceId
                            ? { ...d, isOn: !d.isOn }
                            : d
                        ),
                      }))
                    );
                  }}
                />
              ))}
            </div>
          </div>
        ))}

    </div>

  </Layout>
);
}

export default Dashboard;
