using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
using bracket_server.Tournaments.Exposure;
using bracket_server.Tournaments.SmartFill;
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
                    return ErrorResult($"tournament name already in use {cat.TournamentName}");
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

        public static IResult DeleteTournament(TournamentIDAuthToken t_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(t_token.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Tournament to_delete = tournament_dal.GetTournamentTopLevelByID(t_token.TournamentID);
                if(to_delete.Finalized)
                {
                    return ErrorResult("tournament already finalized");
                }
                bool success = tournament_dal.DeleteTournament(to_delete.ID);
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
            try
            {
                UserID user_id = ConfirmAuth(comp_token.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                TournamentCompetitor competitor = tournament_dal.CreateTournamentCompetitor(comp_token.TournamentID, comp_token.Competitor);
                if (competitor.IsEmpty()) return ErrorResult("error creating competitor");
                return GoodResult(competitor);
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }

        }

        public static IResult DeleteCompetitor(CompetitorAuthToken competitor, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(competitor.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                bool success = tournament_dal.DeleteCompetitor(competitor.Competitor);
                return success ? EmptyValidResult() : ErrorResult("error deleting competitor");
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult SaveSeedData(SeedDataAuthToken seed_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(seed_token.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                bool success = tournament_dal.SaveSeedData(seed_token.Seed);
                return success ? EmptyValidResult() : ErrorResult("error saving seed");
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult FinalizeTournament(TournamentIDAuthToken tournament_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(tournament_token.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Tournament tournament = tournament_dal.GetTournamentTopLevelByID(tournament_token.TournamentID);
                if (tournament.IsEmpty()) return ErrorResult("could not find tournament");
                List<TournamentCompetitor> competitors = tournament_dal.TournamentCompetitorsForTournament(tournament_token.TournamentID);
                if(competitors.Count != 64) throw new ArgumentException($"Not the correct number of competitors {competitors.Count}");
                tournament.FillOutTournament(competitors);
                bool success = tournament_dal.FinalizeTournament(tournament);
                return ResultFromBool(success, "error finalizing tournament");
            }
            catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }
        public static IResult SaveKenPomData(AuthTokenCompetitorKenPomData token, ITournamentDAL tournament_dal, IUserDAL user_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(token.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                if (token.KenPom == null) return Results.BadRequest();
                bool success = tournament_dal.SaveKenpomData(token.KenPom);
                return ResultFromBool(success, "error saving kenpom");
            }
            catch(Exception ex) 
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            };
        }
        public static IResult TestSmartFill(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                Tournament t = tournament_dal.FullActiveTournament();
                SmartFillEvaluation evaluation = new SmartFillEvaluation(t);
                ExposureReport report = evaluation.DoSimulations(1000, tournament_dal);
                return GoodResult(report.ReportByRound(tournament_dal));

            }
            catch(Exception ex) 
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        public static IResult GetActiveTournament(ITournamentDAL dal)
        {
            try 
            {
                Tournament t = dal.FullActiveTournament();
                return GoodResult(t);
            }
            catch(Exception ex)
            {
                return ResultFromException(dal.GetDataAccess(), ex);
            }
        }

        public static IResult SaveGameOutcome(OutcomeAuthToken outcome_token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                UserID user_id = ConfirmAuth(outcome_token.Token, user_dal);
                if (user_id.IsEmpty()) return Results.Unauthorized();
                bool success = tournament_dal.SaveOutcome(outcome_token.Outcome);
                return ResultFromBool(success, "failed to save");
            }
            catch(Exception ex)
            {
                return ResultFromException(user_dal.GetDataAccess(), ex);
            }
        }

        private static UserID ConfirmAuth(AuthToken token, IUserDAL dal)
        {
            return ConfirmDesiredAuth(token, dal, "admin");
        }

        public override void AddRoutes()
        {
            AddPost("/createtournament", CreateTournament);
            AddPost("/alltournaments", AllTournaments);
            AddPost("/deletetournament", DeleteTournament);
            AddPost("/createcompetitor", CreateCompetitor);
            AddPost("/deletecompetitor", DeleteCompetitor);
            AddPost("/saveseeddata", SaveSeedData);
            AddPost("/finalizetournament", FinalizeTournament);
            AddPost("/savekenpom", SaveKenPomData);
            AddPost("/testsmartfill", TestSmartFill);
            AddGet("/activetournament", GetActiveTournament);
            AddPost("/savegameoutcome", SaveGameOutcome);
        }
    }
}
