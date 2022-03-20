import '../../Styling/BracketCSS.css'
function Team({name, id, seed, renderSeed, handleTeamClicked, isLeft, asWrong}) {
    let name_span = <span>{name}</span>
    let div_class_name = "team" + (asWrong ? " wrong-pick" : "");
    return (
        <div className={div_class_name} onClick={() => handleTeamClicked(id)}>
            {!isLeft ? name_span : null}
            <span className="seedNumber">{renderSeed ? seed : null}</span>
            {isLeft ? name_span : null}
        </div>
    )
}

export default Team;