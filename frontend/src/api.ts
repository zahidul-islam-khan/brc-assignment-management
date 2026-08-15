export const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api";

export class ApiError extends Error {
  constructor(public status: number, public data: any) {
    super(data?.message || "An error occurred");
    this.name = "ApiError";
  }
}

async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const url = `${API_URL}${endpoint}`;
  
  const headers = new Headers(options.headers);
  
  // Attach token
  const token = localStorage.getItem("token");
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  // Handle JSON
  if (options.body && typeof options.body !== "string" && !(options.body instanceof FormData)) {
    options.body = JSON.stringify(options.body);
    headers.set("Content-Type", "application/json");
  }

  const response = await fetch(url, { ...options, headers });

  if (response.status === 204) {
    return {} as T;
  }

  const data = await response.json().catch(() => ({}));

  if (!response.ok) {
    throw new ApiError(response.status, data);
  }

  // The backend uses ApiResponse<T> where data is wrapped in `data` property.
  // We'll extract `data` if it exists.
  if (data && typeof data === 'object' && 'success' in data) {
      if (!data.success) {
          throw new ApiError(response.status, data);
      }
      return data.data as T;
  }

  return data as T;
}

export const api = {
  get: <T>(endpoint: string, options?: RequestInit) => request<T>(endpoint, { ...options, method: "GET" }),
  post: <T>(endpoint: string, body: any, options?: RequestInit) => request<T>(endpoint, { ...options, body, method: "POST" }),
  put: <T>(endpoint: string, body: any, options?: RequestInit) => request<T>(endpoint, { ...options, body, method: "PUT" }),
  patch: <T>(endpoint: string, body?: any, options?: RequestInit) => request<T>(endpoint, { ...options, body, method: "PATCH" }),
  delete: <T>(endpoint: string, options?: RequestInit) => request<T>(endpoint, { ...options, method: "DELETE" }),
};
