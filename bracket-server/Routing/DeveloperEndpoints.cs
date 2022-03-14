using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
using bracket_server.Tournaments.KenPom;

namespace bracket_server.Routing
{
    public class DeveloperEndpoints : EndpointManager
    {
        public IResult MillerTest(ContestAuthToken cat, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(cat.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Tournament tournament = tournament_dal.CreateTournament(user_id, cat.TournamentName);
                if(tournament.IsEmpty())
                {
                    return ErrorResult($"tournament name already used {cat.TournamentName}");
                }
                List<NewTournamentCompetitor> fake_competitors = GetFakeCompetitors();
                foreach(NewTournamentCompetitor competitor in fake_competitors)
                {
                    TournamentCompetitor comp = tournament_dal.CreateTournamentCompetitor(tournament.ID, competitor);
                    if(comp.IsEmpty())
                    {
                        throw new Exception("failed to create tournament competitor");
                    }
                }
                TournamentIDAuthToken tournament_token = new TournamentIDAuthToken()
                {
                    Token = cat.Token,
                    TournamentID = tournament.ID
                };
                IResult result = AdminEndpoints.FinalizeTournament(tournament_token, user_dal, tournament_dal);
                tournament_dal.InsertActiveBracketingTournamentID(tournament.ID);
                List<TournamentCompetitor> competitors = tournament_dal.TournamentCompetitorsForTournament(tournament.ID);
                EnterFakeKenPomRatings(tournament, competitors, tournament_dal);
                return EmptyValidResult();
            }
            catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        private static List<NewTournamentCompetitor> GetFakeCompetitors()
        {
            List<NewTournamentCompetitor> competitors = new List<NewTournamentCompetitor>();
            for(int i = 0; i < 64; i++)
            {
                NewTournamentCompetitor new_competitor = new NewTournamentCompetitor()
                {
                    Seed = GetSeed(i+1),
                    Division = GetDivision(i+1),
                    Name = $"Team {i+1}"
                };
                competitors.Add(new_competitor);
            }
            return competitors;
        }

        private static int GetSeed(int number)
        {
            while(number > 16)
            {
                number -= 16;
            }
            return number;
        }
        private static string GetDivision(int number)
        {
            if(number <= 16)
            {
                return "A";
            }
            if(number <= 32)
            {
                return "B";
            }
            if(number <= 48)
            {
                return "C";
            }
            return "D";
        }

        private static UserID ConfirmAuth(AuthToken token, IUserDAL dal)
        {
            return ConfirmDesiredAuth(token, dal, "developer");
        }
        public override void AddRoutes()
        {
            AddPost("/millertest", MillerTest);
        }

        private void EnterFakeKenPomRatings(Tournament tournament, List<TournamentCompetitor> competitors, ITournamentDAL dal) 
        {
            foreach(TournamentCompetitor competitor in competitors)
            {
                KenPomData data = GetFakeKenPomRating(competitor);
                dal.SaveKenpomData(new KenPomDataReference()
                {
                    Tempo=data.Tempo,
                    OffensiveEfficiency=data.OffensiveEfficiency,
                    OverallEfficiency=data.OverallEfficiency,
                    DefensiveEfficiency =data.DefensiveEfficiency,
                    CompetitorID=competitor.ID,
                    TournamentID=tournament.ID
                });
            }
        }

        private KenPomData GetFakeKenPomRating(TournamentCompetitor competitor)
        {
            double tempo = (Random.Shared.NextDouble() * 20) + 60;
            double overall_efficiency = EfficiencyFromSeed(competitor.Seed);
            double off_efficiency = EfficiencyFromSeed(competitor.Seed);
            double def_efficiency = EfficiencyFromSeed(competitor.Seed);
            return new KenPomData(off_efficiency, def_efficiency, tempo, overall_efficiency);
        }

        private double EfficiencyFromSeed(int seed)
        {
            int floor = (105 - (seed * 2));
            return floor + (Random.Shared.NextDouble() * 15);
        }
    }
}
