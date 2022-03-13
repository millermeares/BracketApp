import '../../Styling/BracketCSS.css'
function Team({name, id, seed, renderSeed, handleTeamClicked, isLeft}) {
    let name_span = <span>{name}</span>
    return (
        <div className="team" onClick={() => handleTeamClicked(id)}>
            {!isLeft ? name_span : null}
            <span className="seedNumber">{renderSeed ? seed : null}</span>
            {isLeft ? name_span : null}
        </div>
    )
}

export default Team;