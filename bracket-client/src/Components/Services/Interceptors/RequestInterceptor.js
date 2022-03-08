import axios from 'axios';
export function requestInterceptor() {
    axios.interceptors.request.use(function (successfulReq) {
        // modify code
        // return 
        console.log(successfulReq);
        return successfulReq;
    }, function (error) {
        return Promise.reject(error);
    }
    )
}