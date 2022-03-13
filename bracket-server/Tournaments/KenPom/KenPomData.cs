namespace bracket_server.Tournaments.KenPom
{
    public class KenPomData
    {
        public double OffensiveEfficiency { get; set; }
        public double DefensiveEfficiency { get; set; }
        public double Tempo { get; set; }
        public double OverallEfficiency { get; set; }
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
    }
}
