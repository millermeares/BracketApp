SET @tournamentID="05501964-c6da-4542-bee3-7edd307ed575";
SELECT c.competitorName, g._fk_tournamentRound, COUNT(bgp._fk_game) AS 'appearances' FROM tournament t
JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID
LEFT OUTER JOIN bracket_game_prediction bgp ON bgp._fk_tournament=c._fk_tournament AND c.competitorID=bgp._fk_competitor
LEFT OUTER JOIN tournament_game g ON g.gameID=bgp._fk_game AND g._fk_tournament=c._fk_tournament
WHERE t.tournamentID=@tournamentID
GROUP BY c.competitorID, g._fk_tournamentRound 
ORDER BY competitorName, _fk_tournamentRound;

select * from tournament;
set @userID="cf79a314-e3f6-4448-833a-29d6984c311d";
SELECT c.competitorName, c.competitorID, c._fk_division, c._fk_seed, c._fk_tournament, g._fk_tournamentRound, COUNT(bgp._fk_game) AS 'appearances' FROM tournament t
        JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID
        LEFT OUTER JOIN bracket_game_prediction bgp ON bgp._fk_tournament=c._fk_tournament AND c.competitorID=bgp._fk_competitor
        LEFT OUTER JOIN user_bracket b ON b.bracketID=bgp._fk_bracket AND b.completed IS TRUE
        LEFT OUTER JOIN tournament_game g ON g.gameID=bgp._fk_game AND g._fk_tournament=c._fk_tournament
        WHERE t.tournamentID=@tournamentID AND b._fk_user=@userID
        GROUP BY c.competitorID, g._fk_tournamentRound 
        ORDER BY competitorName, _fk_tournamentRound;
        
        
SELECT r.round, r.name 
FROM tournament_round r
JOIN tournament t ON t._fk_type=_fk_tournamentType
WHERE t.tournamentID=@tournamentID;


SELECT tournamentID, name, t.createdTime, creator, t._fk_type AS tournamentType, tournamentFinalized,
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, g._fk_competitor_Winner,
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision
        FROM tournament t
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID
        LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
        WHERE t.tournamentID=@tournamentID
        ORDER BY g._fk_tournamentRound, gameDivision, g.gameID;
        
select * from tournament_round;
select * from tournament_game where _fk_tournament=@tournamentID;
set @gameID="043dad5a-6e57-4638-be29-9f33848f18ee";
SELECT * FROM game_participant gp 
JOIN tournament_game g ON g.gameID=gp._fk_game
JOIN competitor_tournament c ON c.competitorID=gp._fk_competitor AND g._fk_tournament=c._fk_tournament
WHERE gp._fk_game=@gameID;


UPDATE tournament_game SET winner=@competitorID 
WHERE _fk_tournament=@tournament AND _fk_game=@gameID;
INSERT INTO game_participant(_fk_competitor, _fk_game)
SELECT @competitorID, gameID 
FROM tournament_game
WHERE _fk_tournament=@tournament AND @gameID IN (_fk_leftGame, _fk_rightGame);
UPDATE tournament_game SET WINNER=NULL 
WHERE _fk_tournament=@tournamentID AND _fk_tournamentRound > @roundOfGame AND _fk_competitor_winner=@otherCompetitorID;
DELETE game_participant FROM game_participant gp
JOIN game g ON g.gameID=gp._fk_game AND g._fk_tournament=@tournamentID
WHERE gp._fk_competitor=@otherCompetitorID AND g._fk_tournamentRound > @roundOfGame;


select * from user_bracket order by creationTime DESC;
select * from bracket_game_prediction where _fk_bracket="307b02f9-08a2-4548-a83d-eece62363882";

select * from tournament_game;
select * from tournament_game where gameID="4a693b60-84af-44d7-9020-a651c01765a9";
select * from tournament_game;
select * from competitor_tournament where competitorid="1b68b26e-4856-4bad-a2ce-632d2faa6f0d";
select * from tournament_game where _fk_leftGame = "4a693b60-84af-44d7-9020-a651c01765a9" OR _fk_rightGame="4a693b60-84af-44d7-9020-a651c01765a9";