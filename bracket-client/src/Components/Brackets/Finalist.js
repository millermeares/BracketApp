function Finalist({ name, id, seed, className, nameClass, handleSetWinner }) {
    return (
        <ul className={className}>
            <li className='spacer'>&nbsp;</li>
            <li className={nameClass}>
                <div onClick={() => (handleSetWinner) ? handleSetWinner(id) : {}}>
                    <span>{name}</span>
                </div>
                
            </li>
            <li className='spacer'>&nbsp;</li>
        </ul>
    )
}

export default Finalist