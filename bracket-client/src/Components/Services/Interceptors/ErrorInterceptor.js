import axios from 'axios';

export function errorInterceptor() {
    axios.interceptors.response.use(null, (error) => {
        const { response } = error;
        if (!response) {
            // network error
            console.error(error);
            return;
        }
    
        const errorMessage = response.data?.message || response.statusText;
        console.error('ERROR:', errorMessage);
    });
}