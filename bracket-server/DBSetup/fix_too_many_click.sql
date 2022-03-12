# undo FINALIZE script

set @tournamentID="5089058a-5e2e-4a30-a310-26f3b607da6a";
UPDATE tournament SET tournamentFinalized=0 WHERE tournamentID=@tournamentID;
SET FOREIGN_KEY_CHECKS=0;
delete from tournament_game where _fk_tournament=@tournamentID;
SET FOREIGN_KEY_CHECKS=1;
delete from game_participant where _fk_game IN (select gameid from tournament_game where _fk_tournament=@tournamentID);