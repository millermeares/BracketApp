import {useAuth} from '../../Entry/Auth'
import {useState, useEffect} from 'react';
import ExposureReportTable from './ExposureReportTable'
import api from '../../Services/api';

function GenericExposureReportView({title, api_url}) {
    let [report, setReport] = useState(null);
    let auth = useAuth();
    useEffect(() => {
        if(!report) {
            api.post(api_url, auth.token).then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setReport(response.data.payload);
            }).catch(err => {
                console.log(err);
            });
        }
    })
    if(!report) {
        return <div>Loading Report...</div> 
    }
    return (
        <div>
            <div>{title}</div>
            <ExposureReportTable {...report} />
        </div>
        
    )
}

export default GenericExposureReportView;