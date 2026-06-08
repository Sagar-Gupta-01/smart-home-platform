import { useState } from "react";
import api from "../services/api";
import { useNavigate } from "react-router-dom";

function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const handleLogin = async () => {
        try {
            
            await api.post("/api/Auth/login", {
            email,
            passwordHash: password
            });
            alert("Login successful");
            navigate("/dashboard");
        } catch (error) {
            alert("Login failed");
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
            />

            <br /><br />

            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />

            <br /><br />

            <button onClick={handleLogin}>Login</button>
        </div>
    );
}

export default Login;
