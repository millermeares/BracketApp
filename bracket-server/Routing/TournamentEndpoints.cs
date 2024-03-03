using bracket_server.Brackets;
using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
using bracket_server.Tournaments.Exposure;
using bracket_server.Tournaments.KenPom;

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

        //
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
                bool success = tournament_dal.FinishBracket(bracket);
                if(!success)
                {
                    return ErrorResult("error saving");
                }
                return NewOrLatestBracket(id_token.Token, user_dal, tournament_dal); // i like this.
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult SavePick(BracketPickAuthToken pick_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = user_dal.UserIDFromToken(pick_token.Token);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Bracket b = tournament_dal.GetBracket(pick_token.Pick.BracketID);
                List<BracketPickChange> pick_changes = b.GetPickChanges(pick_token.Pick);
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

        public static IResult SmartFillBracket(IDAuthToken id_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = user_dal.UserIDFromToken(id_token.Token);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Bracket bracket = tournament_dal.GetBracket(id_token.ID);
                // ok here is where we need to do the 'simulate' type thing
                bracket.SmartFill(tournament_dal);
                // here's the thing - are we considering this to be a pick thing? like are those gonna be finalized fr? probably right? 
                // yeah they are. hm.
                return GoodResult(bracket);
            }catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult BracketSummariesForUser(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = user_dal.UserIDFromToken(token);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                List<BracketSummary> summaries = tournament_dal.BracketSummariesForUser(user_id);
                string active_tournament_id = tournament_dal.GetActiveBracketingTournamentID();
                // filtering here instead of dal to avoid choice being too committal.
                return GoodResult(summaries.Where(s => s.TournamentID == active_tournament_id)); 
            }
            catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult CompletedBracket(string bracketID, ITournamentDAL dal)
        {
            try
            {
                Bracket b = dal.GetBracket(bracketID);
                if(b.IsEmpty())
                {
                    return ErrorResult("could not find bracket");
                }
                return GoodResult(b);
            }catch(Exception ex)
            {
                return ResultFromException(dal.GetDataAccess(), ex);
            }
        }

        public static IResult KenPomDataForActiveTournament(ITournamentDAL dal)
        {
            try
            {
                string active_tournament = dal.GetActiveBracketingTournamentID();
                List<CompetitorKenPomData> data = dal.AllCompetitorKenPomDataForTournament(active_tournament);
                data.Sort((x, y) => x.Competitor.Name.CompareTo(y.Competitor.Name));
                return GoodResult(data);
            }catch(Exception ex) 
            {
                return ResultFromException(dal.GetDataAccess(), ex);
            }
        }

        public static IResult ExposureReportForUser(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = user_dal.UserIDFromToken(token);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                string active_tournament = tournament_dal.GetActiveBracketingTournamentID();
                ExposureReport report = tournament_dal.ExposureReportForUser(user_id, active_tournament);
                return GoodResult(report.ReportByRound(tournament_dal));
            }catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult ExposureReportForAll(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                //todo: decide who is going to be able to run this query. 
                string active_tournament = tournament_dal.GetActiveBracketingTournamentID();
                ExposureReport report = tournament_dal.ExposureReportTournament(active_tournament);
                return GoodResult(report.ReportByRound(tournament_dal));
            }
            catch(Exception ex)
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
            AddPost("/smartfillbracket", SmartFillBracket);
            AddPost("/bracketsforuser", BracketSummariesForUser);
            AddGet("/completedbracket/{bracketID}", CompletedBracket);
            AddGet("/kenpomdata", KenPomDataForActiveTournament);
            AddPost("/exposureforuser", ExposureReportForUser);
        }
    }
}
