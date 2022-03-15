import GenericExposureReportView from "./GenericExposureReportView";

function SmartFillExposureReport() {
    let title = "Exposure report for smart fill after 1000 iterations.";
    return <GenericExposureReportView title={title} api_url="/testsmartfill" />
}

export default SmartFillExposureReport;