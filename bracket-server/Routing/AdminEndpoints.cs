using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
using Microsoft.AspNetCore.Mvc;
using UserManagement.UserModels;

namespace bracket_server.Routing
{
    public class AdminEndpoints : EndpointManager
    {
        public static IResult CreateTournament([FromBody]ContestAuthToken cat, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                AuthToken token = cat.AuthToken;
                if (!token.Roles.Select(r => r.Name).Contains("admin"))
                {
                    return Results.Unauthorized();
                }
                UserID user_id = user_dal.UserIDFromToken(token);
                if(user_id.IsEmpty()) 
                {
                    return Results.Unauthorized(); // todo frontend: make this automatically log the user out.
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

        public override void AddRoutes()
        {
            AddPost("/createtournament", CreateTournament);
        }
    }
}
