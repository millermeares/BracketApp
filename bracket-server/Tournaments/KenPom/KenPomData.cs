namespace bracket_server.Tournaments.KenPom
{
    public class KenPomData
    {
        public double OffensiveEfficiency { get; set; } = double.MinValue;
        public double DefensiveEfficiency { get; set; } = 0;
        public double Tempo { get; set; } = double.MinValue;
        public double OverallEfficiency { get; set; } = 0;
        public KenPomData()
        {

        }
        public KenPomData(double offensive, double defensive, double tempo, double overall)
        {
            OffensiveEfficiency = offensive;
            DefensiveEfficiency = defensive;
            Tempo = tempo;
            OverallEfficiency= overall;
        }

        public bool IsComplete()
        {
            return Tempo > 0 && OverallEfficiency > -100 && OverallEfficiency < 200; 
        }

        public double GetSpreadDiff(KenPomData other_team)
        {
            double efficiency_diff = OverallEfficiency- other_team.OverallEfficiency;
            double tempo_sum = Tempo + other_team.Tempo;
            return (tempo_sum * efficiency_diff) / 200;
        }

    }
}
