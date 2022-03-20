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
            if (game.PredictedCompetitor1 == null || game.PredictedCompetitor2 == null) throw new ArgumentException("null competitors");
            return BothPartiesHaveKenPom(game.PredictedCompetitor1, game.PredictedCompetitor2);
        }
        public bool BothPartiesHaveKenPom(TournamentCompetitor comp1, TournamentCompetitor comp2)
        {
            KenPomData data_1 = GetKenPomData(comp1.ID);
            KenPomData data_2 = GetKenPomData(comp2.ID);
            return data_1.IsComplete() && data_2.IsComplete();
        }

        public double GetKenPomSpreadDiff(Game game)
        {
            return GetKenPomSpreadDiff(game.PredictedCompetitor1, game.PredictedCompetitor2);
        }

        public double GetKenPomSpreadDiff(TournamentCompetitor competitor1, TournamentCompetitor competitor2)
        {
            KenPomData data_1 = GetKenPomData(competitor1.ID);
            KenPomData data_2 = GetKenPomData(competitor2.ID);
            return data_1.GetSpreadDiff(data_2);
        }

        public double WinPercentageFromKenPomSpreadDiff(double team_1_spread_diff)
        {
            return 1 - CumulativeDensityFunction(team_1_spread_diff);
        }

        private double CumulativeDensityFunction(double spread)
        {
            int x = 0;
            int sigma = 11;
            return 0.5 * (1 + ERF((x - spread) / (sigma * Math.Sqrt(2))));
        }

        private double ERF(double x)
        {
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }
    }
}
