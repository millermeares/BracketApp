using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
using Microsoft.AspNetCore.Mvc;
using UserManagement.UserModels;

namespace bracket_server.Routing
{
    public class AdminEndpoints : EndpointManager
    {
        public static IResult CreateTournament(ContestAuthToken cat, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(cat.Token, user_dal);
                if(user_id.IsEmpty())
                {
                    return Results.Unauthorized();
                }
                Tournament tournament = tournament_dal.CreateTournament(user_id, cat.TournamentName);
                if(tournament.IsEmpty())
                {
                    return ErrorResult("could not create tournament for some reason");
                }
                return GoodResult(tournament);
            }catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult AllTournaments(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                List<Tournament> tournaments = tournament_dal.AllTournaments();
                return GoodResult(tournaments);
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult DeleteTournament(TournamentAuthToken tournament, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(tournament.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                bool success = tournament_dal.DeleteTournament(tournament.TournamentID);
                if (!success) return ErrorResult("Failed to delete");
                return EmptyValidResult();
            }
            catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult CreateCompetitor(NewCompetitorAuthToken comp_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            UserID user_id = ConfirmAuth(comp_token.Token, user_dal);
            if (user_id.IsEmpty()) return Results.Unauthorized();
            TournamentCompetitor competitor = tournament_dal.CreateTournamentCompetitor(comp_token.TournamentID, comp_token.Competitor);
            if (competitor.IsEmpty()) return ErrorResult("error creating competitor");
            return GoodResult(competitor);
        }

        public static IResult DeleteCompetitor(CompetitorAuthToken competitor, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            UserID user_id = ConfirmAuth(competitor.Token, user_dal);
            if (user_id.IsEmpty()) return Results.Unauthorized();
            bool success = tournament_dal.DeleteCompetitor(competitor.Competitor);
            return success ? EmptyValidResult() : ErrorResult("error deleting competitor");
        }

        private static UserID ConfirmAuth(AuthToken token, IUserDAL user_dal)
        {
            if (!token.Roles.Select(r => r.Name).Contains("admin")) // todo: this isn't secure - i should look it up first.
            {
                return UserID.MakeEmpty();
            }
            return user_dal.UserIDFromToken(token);
        }

        public override void AddRoutes()
        {
            AddPost("/createtournament", CreateTournament);
            AddPost("/alltournaments", AllTournaments);
            AddPost("/deletetournament", DeleteTournament);
            AddPost("/createcompetitor", CreateCompetitor);
            AddPost("/deletecompetitor", DeleteCompetitor);
        }
    }
}
