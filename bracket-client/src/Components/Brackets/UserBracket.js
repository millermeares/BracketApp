import NCAABracket from "./NCAABracket";
function UserBracket({tournament, id, name, owner, completed, champTotalPoints, creationTime}) {

    //todo: functions like onPickMade or something
    return (
        <NCAABracket {...tournament} />
    )
}

export default UserBracket;