import axios from 'axios';
export function requestInterceptor() {
    axios.interceptors.request.use(function (successfulReq) {
        // modify code
        // return 
        console.log(successfulReq);
        return successfulReq;
    }, function (error) {
        const { response } = error;
        if (!response) {
            // network error
            console.error(error);
            return;
        }
        const errorMessage = response.data?.message || response.statusText;
        console.error('ERROR:', errorMessage);
    }
    )
}