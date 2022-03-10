import api from './api';
let post = (route, params, param_name_in_object, auth) => {
    let obj = {
        Token: auth.token
    }
    obj[param_name_in_object] = params;
    console.log(obj);
    return api.post(route, obj);
}

export default {post};
