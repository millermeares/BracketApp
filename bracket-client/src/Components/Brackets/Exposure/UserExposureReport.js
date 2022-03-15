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
        <div>
            <div>Below is a list of the percentage of brackets in which you selected each team to win in each respective round.</div>
            <ExposureReportTable {...report} />
        </div>
        
    )
}

export default UserExposureReport;