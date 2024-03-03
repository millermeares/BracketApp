using bracket_server.Brackets;
using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
using bracket_server.Tournaments.KenPom;
using Microsoft.Extensions.Primitives;

namespace bracket_server.Routing
{
    public class DeveloperEndpoints : EndpointManager
    {
        public IResult MillerTest(ContestAuthToken cat, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                for(int i = 0; i < 10000; i++)
                {
                    SimulateAndSubmitBracket(cat.Token, user_dal, tournament_dal);
                }
                return GoodResult("hi");
            }
            catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public IResult SimulateAndSubmitBracket(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            UserID id = user_dal.UserIDFromToken(token);
            if (id.IsEmpty())
            {
                return Results.Unauthorized();
            }
            string tournament_id = tournament_dal.GetActiveBracketingTournamentID();
            if (string.IsNullOrEmpty(tournament_id))
            {
                return ErrorResult("tell miller to assign an active tournament");
            }
            Bracket bracket = Bracket.GetForNewOrLatestBracketForUser(id, tournament_id, tournament_dal);
            if (bracket.IsEmpty())
            {
                return ErrorResult("an error occurred while creating the bracket");
            }
            bracket.SmartFill(tournament_dal);
            IDAuthToken idAuthToken = new IDAuthToken
            {
                Token = token,
                ID = bracket.ID
            };
            return TournamentEndpoints.FinalizeContestEntry(idAuthToken, tournament_dal, user_dal);
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
