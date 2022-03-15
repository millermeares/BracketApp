import {useAuth} from '../../Entry/Auth'
import {useState, useEffect} from 'react';
import ExposureReportTable from './ExposureReportTable'
import api from '../../Services/api';
function UserExposureReport() {
    let [report, setReport] = useState(null);
    let auth = useAuth();

    useEffect(() => {
        if(!report) {
            api.post("/exposureforuser", auth.token).then(response => {
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
        <ExposureReportTable {...report} />
    )
}

export default UserExposureReport;