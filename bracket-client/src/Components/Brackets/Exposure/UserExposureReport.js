import {useAuth} from '../../Entry/Auth'
import {useState, useEffect} from 'react';
import ExposureReportTable from './ExposureReportTable'
import api from '../../Services/api';
import GenericExposureReportView from './GenericExposureReportView';
function UserExposureReport() {
    let title = "Below is a list of the percentage of brackets in which you selected each team to win in each respective round.";
    return <GenericExposureReportView title={title} api_url="/exposureforuser"/>
}

export default UserExposureReport;