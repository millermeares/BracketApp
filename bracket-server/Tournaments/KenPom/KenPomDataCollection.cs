namespace bracket_server.Tournaments.KenPom
{
    public class KenPomDataCollection
    {
        private Dictionary<string, KenPomData> _data = new Dictionary<string, KenPomData>();
        public KenPomDataCollection()
        {

        }
        public KenPomDataCollection(Dictionary<string, KenPomData> data)
        {
            _data = data;
        }
        public KenPomData GetKenPomData(string competitor)
        {
            return _data[competitor];
        }

        public bool BothPartiesHaveKenPom(Game game)
        {
            if (game.Competitor1 == null || game.Competitor2 == null) throw new ArgumentException("null competitors");
            return BothPartiesHaveKenPom(game.Competitor1, game.Competitor2);
        }
        public bool BothPartiesHaveKenPom(TournamentCompetitor comp1, TournamentCompetitor comp2)
        {
            KenPomData data_1 = GetKenPomData(comp1.ID);
            KenPomData data_2 = GetKenPomData(comp2.ID);
            return data_1.IsComplete() && data_2.IsComplete();
        }

        public double GetKenPomSpreadDiff(Game game)
        {
            return GetKenPomSpreadDiff(game.Competitor1, game.Competitor2);
        }

        public double GetKenPomSpreadDiff(TournamentCompetitor competitor1, TournamentCompetitor competitor2)
        {
            KenPomData data_1 = GetKenPomData(competitor1.ID);
            KenPomData data_2 = GetKenPomData(competitor2.ID);
            return data_1.GetSpreadDiff(data_2);
        }

        public double WinPercentageFromKenPomSpreadDiff(double team_1_spread_diff)
        {
            //1.85x + 50.2 ish. 
            // this is just a trendline from historical performance of gambling odds in ncaa tournament.
            return (1.85 * team_1_spread_diff + 50.2) / 100;
        }
    }
}
