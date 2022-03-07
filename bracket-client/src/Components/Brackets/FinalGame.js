import '../../Styling/BracketCSS.css'
import Team from './Team';
function FinalGame({competitor1, competitor2, id, leftGame, rightGame, winner}) {
    let spacer_li = <li className='spacer'>&nbsp;</li>

    return (
        <div className="champ-wrapper">
            <ul className='round semi-final'>
                {spacer_li}
                <li className='game-left game-top'>
                    <Team {...competitor1} />
                </li>
                {spacer_li}
            </ul>
            <ul className='round finals'>
                {spacer_li}
                <li className='game final'>
                    <Team {...winner} />
                </li>
                {spacer_li}
            </ul>
            <ul className='round semi-final'>
                {spacer_li}
                <li className='game-right game-top'>
                    <Team {...competitor2} />
                </li>
                {spacer_li}
            </ul>
        </div>
    )
}

export default FinalGame;