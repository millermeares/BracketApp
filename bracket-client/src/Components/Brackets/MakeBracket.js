import { Bracket, RoundProps, Seed, SeedItem, SeedTeam, RenderSeedProps } from 'react-brackets';
import { Outlet } from 'react-router-dom';
import React from 'react';
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

  const CustomSeed = ({seed, breakpoint, roundIndex, seedIndex}) => {
    // breakpoint passed to Bracket component
    // to check if mobile view is triggered or not
  
    // mobileBreakpoint is required to be passed down to a seed
    return (
      <Seed mobileBreakpoint={breakpoint} style={{ fontSize: 12 }}>
        <SeedItem>
          <div>
            <SeedTeam style={{ color: 'red' }}>{seed.teams[0]?.name || 'NO TEAM '}</SeedTeam>
            <SeedTeam>{seed.teams[1]?.name || 'NO TEAM '}</SeedTeam>
          </div>
        </SeedItem>
      </Seed>
    );
  };

function MakeBracket() {
  return (
    <div>
      <Bracket rounds={rounds} renderSeedComponent={CustomSeed} />
    </div>
    )
}

export default MakeBracket;