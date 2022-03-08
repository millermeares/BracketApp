import axios from 'axios';
let is_dev = process.env.NODE_ENV == "development";
export default is_dev ? axios.create() : axios.create({
    baseURL: process.env.REACT_APP_BASE_URL
});