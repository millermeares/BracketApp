using bracket_server.Brackets;
using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
namespace bracket_server.Routing
{
    public class TournamentEndpoints : EndpointManager
    {
        public static IResult FakeTournament()
        {
            Tournament tournament = FakeTournamentData.MakeFakeNCAATournament();
            return GoodResult(tournament); ;
        }
        public static IResult FakeTournamentSkeleton()
        {

            Tournament tournament = FakeTournamentData.MakeFakeNCAATournamentSkeleton();
            return GoodResult(tournament);
        }

        public static IResult GetCompetitors(GenericID tournament_id, ITournamentDAL tournament_dal)
        {
            try
            {
                List<TournamentCompetitor> competitors = tournament_dal.TournamentCompetitorsForTournament(tournament_id.ID);
                return GoodResult(competitors);
            }
            catch (Exception ex)
            {
                return ResultFromException(tournament_dal.GetDataAccess(), ex);
            }
        }

        public static IResult GetSeedData(ITournamentDAL tournament_dal)
        {
            string tournamentType = "NCAA64Basketball"; // this would probably be a url argument
            try
            {
                return GoodResult(tournament_dal.GetSeedDataForTournamentType(tournamentType));
            }
            catch (Exception ex)
            {
                return ResultFromException(tournament_dal.GetDataAccess(), ex);
            }
        }

        public static IResult NewOrLatestBracket(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID id = user_dal.UserIDFromToken(token);
                if (id.IsEmpty())
                {
                    return Results.Unauthorized();
                }
                string tournament_id = tournament_dal.GetActiveBracketingTournamentID();
                if(string.IsNullOrEmpty(tournament_id))
                {
                    return ErrorResult("tell miller to assign an active tournament");
                }
                Bracket bracket = Bracket.GetForNewOrLatestBracketForUser(id, tournament_id, tournament_dal);
                if (bracket.IsEmpty())
                {
                    return ErrorResult("an error occurred while creating the bracket");
                }
                return GoodResult(bracket);
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult FinalizeContestEntry(IDAuthToken id_token, ITournamentDAL tournament_dal, IUserDAL user_dal)
        {
            try
            {
                UserID user_id = user_dal.UserIDFromToken(id_token.Token);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Bracket bracket = tournament_dal.GetBracket(id_token.ID);
                if(!bracket.SaveableState())
                {
                    return ErrorResult("fill out entire bracket before saving");
                }
                tournament_dal.FinishBracket(bracket);
                return NewOrLatestBracket(id_token.Token, user_dal, tournament_dal); // i like this.
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult SavePick(PickAuthToken pick_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = user_dal.UserIDFromToken(pick_token.Token);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Bracket b = tournament_dal.GetBracket(pick_token.Pick.BracketID);
                List<PickChange> pick_changes = b.GetPickChanges(pick_token.Pick);
                if(pick_changes.Count == 0) // already same
                {
                    return EmptyValidResult();
                }
                bool success = tournament_dal.SavePickChanges(pick_changes);
                return EmptyValidResult();
            }catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public override void AddRoutes()
        {
            AddGet("/faketournament", FakeTournament);
            AddGet("/faketournamentskeleton", FakeTournamentSkeleton);
            AddPost("/getcompetitors", GetCompetitors);
            AddGet("/getseeddata", GetSeedData);
            AddPost("/neworlatestbracket", NewOrLatestBracket);
            AddPost("/finalizecontestentry", FinalizeContestEntry);
            AddPost("/savepick", SavePick);
        }

        

        

       
    }
}
