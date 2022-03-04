import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';

import {errorInterceptor} from './Components/Services/Interceptors/ErrorInterceptor'
import { history } from './Components/Services/Helpers/History';
import { render } from '@testing-library/react';
errorInterceptor();

ReactDOM.render(
   <Router>
      <App />
    </Router>,
   document.getElementById('root')
 );
