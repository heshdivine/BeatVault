import axios, { type AxiosResponse } from 'axios';
import type { Beat } from '../models/beat';
import type { User } from '../models/user';

// 1. Base Configuration
axios.defaults.baseURL = 'https://localhost:7144/api'; // Your .NET API URL
axios.defaults.withCredentials = true; // For CORS

// 2. Helper to extract data from the response
const responseBody = <T>(response: AxiosResponse<T>) => response.data;

// 3. Interceptor: Attach Token automatically
axios.interceptors.request.use(config => {
    const token = localStorage.getItem('jwt'); // We will store token here
    if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// 4. Methods for each Entity
const requests = {
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    // put, del, etc...
};

const Beats = {
    list: () => requests.get<Beat[]>('/beats'),
    create: (beat: any) => requests.post<Beat>('/beats', beat),
};

const Account = {
    login: (values: any) => requests.post<User>('/account/login', values),
    register: (values: any) => requests.post<User>('/account/register', values),
};

const agent = {
    Beats,
    Account
};

export default agent;