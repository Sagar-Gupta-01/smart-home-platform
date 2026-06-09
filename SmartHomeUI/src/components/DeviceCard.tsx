import { useState } from "react";
import api from "../services/api";

interface Device {
  id: number;
  name: string;
  isOn: boolean;
}

interface Props {
  device: Device;
  onToggle: (id: number) => void;
}

function DeviceCard({ device, onToggle }: Props) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const toggle = async () => {
    setError("");
    try {
      setLoading(true);

      await api.post(`/api/Devices/${device.id}/toggle`);

      // ✅ update UI only after the server confirms the toggle
      onToggle(device.id);
    } catch {
      setError("Couldn't toggle this device. Try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white rounded-2xl shadow-md p-5 transition hover:shadow-xl border border-gray-100">

      <h4 className="font-semibold text-lg mb-2">
        {device.name}
      </h4>

      <div className="flex items-center justify-between mt-3">
        <span className="text-sm text-gray-500">Status</span>

        <span
          className={`text-sm font-semibold px-3 py-1 rounded-full ${
            device.isOn
              ? "bg-green-100 text-green-700"
              : "bg-red-100 text-red-600"
          }`}
        >
          {device.isOn ? "ON" : "OFF"}
        </span>
      </div>

      <button
        onClick={toggle}
        disabled={loading}
        className={`mt-5 w-full py-2 rounded-lg font-medium text-white transition ${
          loading
            ? "bg-gray-400 cursor-not-allowed"
            : device.isOn
              ? "bg-yellow-500 hover:bg-yellow-600"
              : "bg-blue-500 hover:bg-blue-600"
        }`}
      >
        {loading ? "Processing..." : "Toggle"}
      </button>

      {error && (
        <p className="mt-3 text-sm text-red-600" role="alert">
          {error}
        </p>
      )}
    </div>
  );
}

export default DeviceCard;