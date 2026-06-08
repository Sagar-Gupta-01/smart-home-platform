import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  withCredentials: true // ✅ always send cookies
});


// ✅ Add interceptor
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // ✅ If 401 (token expired)
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // ✅ Call refresh API
        await axios.post(
          `${import.meta.env.VITE_API_BASE_URL}/api/Auth/refresh`,
          {},
          { withCredentials: true }
        );

        // ✅ Retry original request after refresh
        return api(originalRequest);
      } catch (err) {
        // ❌ Refresh failed → redirect to login
        window.location.href = "/login";
      }
    }

    return Promise.reject(error);
  }
);

export default api;
