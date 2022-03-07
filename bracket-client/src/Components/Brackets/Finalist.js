function Finalist({name, id, className, nameClass}) {
    return (
        <ul className={className}>
            <li className='spacer'>&nbsp;</li>
            <li className={nameClass}>{name}</li>
            <li className='spacer'>&nbsp;</li>
        </ul>
    )
}

export default Finalist