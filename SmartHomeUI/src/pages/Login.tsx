import { useState } from "react";
import axios from "axios";
import api from "../services/api";
import { useNavigate } from "react-router-dom";

function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleLogin = async () => {
        setError("");

        // Basic client-side validation before hitting the API
        if (!email.trim() || !password) {
            setError("Email and password are required.");
            return;
        }
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            setError("Please enter a valid email address.");
            return;
        }

        try {
            setLoading(true);
            await api.post("/api/Auth/login", {
                email,
                passwordHash: password,
            });
            navigate("/dashboard");
        } catch (err) {
            // 429 = rate limited by the backend after too many attempts
            if (axios.isAxiosError(err) && err.response?.status === 429) {
                setError("Too many attempts. Please wait a minute and try again.");
            } else {
                setError("Invalid email or password.");
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ padding: 20 }}>
            <h2>Login</h2>

            <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                onKeyDown={(e) => e.key === "Enter" && handleLogin()}
            />

            <br /><br />

            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                onKeyDown={(e) => e.key === "Enter" && handleLogin()}
            />

            <br /><br />

            <button onClick={handleLogin} disabled={loading}>
                {loading ? "Signing in..." : "Login"}
            </button>

            {error && (
                <p style={{ color: "crimson", marginTop: 12 }} role="alert">
                    {error}
                </p>
            )}
        </div>
    );
}

export default Login;
