namespace bracket_server.Tournaments.KenPom
{
    public class CompetitorKenPomData
    {

        public TournamentCompetitor Competitor { get; set; }
        public KenPomData Data { get; set; }
        public CompetitorKenPomData(TournamentCompetitor competitor, KenPomData data)
        {
            Competitor = competitor;
            Data = data;
        }

    }
}
