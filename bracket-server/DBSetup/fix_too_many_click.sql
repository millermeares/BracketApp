# undo FINALIZE script
select * from tournament;
set @tournamentID="d9bc21d3-ddbb-488b-ad0c-b998593a5ce4";
UPDATE tournament SET tournamentFinalized=0 WHERE tournamentID=@tournamentID;
SET FOREIGN_KEY_CHECKS=0;
delete from tournament_game where _fk_tournament=@tournamentID;
SET FOREIGN_KEY_CHECKS=1;
delete from game_participant where _fk_game IN (select gameid from tournament_game where _fk_tournament=@tournamentID);