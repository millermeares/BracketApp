﻿using bracket_server.Tournaments;
using bracket_server.Tournaments.KenPom;
using bracket_server.Tournaments.SmartFill;

namespace bracket_server.Brackets
{
    public class SmartFillArgs
    {
        public ITournamentDAL DAL { get; set; }
        public SeedDataCollection SeedData { get; private set; } = new SeedDataCollection();
        public bool SavePicks { get; set; }
        public KenPomDataCollection KenPom { get; private set; } = new KenPomDataCollection();
        public int AutoWinSpread { get; private set; } = 10;
        public int MaxUnderdogRuns { get; private set; } = 2;
        public int BigUnderdogThreshold { get; private set; } = 7;
        private double MinimumThresholdForPreviousOccurrences = 1.5;

        private bool _filled = false;
        public SmartFillArgs(ITournamentDAL dal)
        {
            DAL = dal;
        }

        public void SetSeedData(SeedDataCollection seeds)
        {
            SeedData = seeds;
        }

        public void SetKenPomData(KenPomDataCollection data)
        {
            KenPom = data;
        }

        public void FillAllDbArgs(Tournament t)
        {
            if (!_filled)
            {
                SetSeedData(GetSeedDataCollection(DAL, t));
                SetKenPomData(GetKenPomCollection(DAL, t));
            }
        }

        protected static SeedDataCollection GetSeedDataCollection(ITournamentDAL dal, Tournament t)
        {
            List<SeedData> seed_data = dal.GetSeedDataForTournamentType(t.TournamentType);
            return new SeedDataCollection(seed_data);
        }

        protected static KenPomDataCollection GetKenPomCollection(ITournamentDAL dal, Tournament t)
        {
            Dictionary<string, KenPomData> data = dal.KenPomDataForTournament(t.ID);
            return new KenPomDataCollection(data);
        }

        // hmm.
        public virtual Pick MakePick(Game game, string tournamentID)
        {
            return new SmartEvalPick(tournamentID, game.ID, game.Winner.ID, game.Round);
        }

        public bool OneOfTeamsNotAllowedToWin(Game game)
        {
            return !TeamAllowedToWin(game.Competitor1, game.Round) || !TeamAllowedToWin(game.Competitor2, game.Round);

        }

        public bool TeamAllowedToWin(TournamentCompetitor competitor, int round)
        {
            if (round < 4) return true;
            SeedData data = SeedData.GetSeedData(competitor.Seed);
            if (round == 4)
            {
                return data.EliteEightOdds > MinimumThresholdForPreviousOccurrences;
            }
            if(round == 5)
            {
                return data.FinalFourOdds > MinimumThresholdForPreviousOccurrences;
            }
            return true;
        }
    }
}
