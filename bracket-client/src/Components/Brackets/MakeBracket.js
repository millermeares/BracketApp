import {Bracket, RoundProps} from 'react-brackets';
import { Outlet } from 'react-router-dom';
// sample data: 
const rounds = [
    {
      title: 'Round one',
      seeds: [
        {
          id: 1,
          date: new Date().toDateString(),
          teams: [{ name: 'Team A' }, { name: 'Team B' }],
        },
        {
          id: 2,
          date: new Date().toDateString(),
          teams: [{ name: 'Team C' }, { name: 'Team D' }],
        },
      ],
    },
    {
      title: 'Round one',
      seeds: [
        {
          id: 3,
          date: new Date().toDateString(),
          teams: [{ name: 'Team A' }, { name: 'Team C' }],
        },
      ],
    },
  ];


function MakeBracket() {
  return (
    <div>
      <Bracket rounds={rounds} />
    </div>
    )
}

export default MakeBracket;