import '../../Styling/BracketCSS.css'
function Team({name, id, seed, renderSeed, handleTeamClicked}) {

    return (
        <div className="team" onClick={() => handleTeamClicked(id)}>
            <span>{renderSeed ? seed : null}</span>
            <span>{name}</span>
        </div>
    )
}

export default Team;