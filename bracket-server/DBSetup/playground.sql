USE bracket_app;
select * from user;
set @userID="cf79a314-e3f6-4448-833a-29d6984c311d";
select * from string_params;
set @tournamentID="05501964-c6da-4542-bee3-7edd307ed575";
# i can get the current score pretty easily i think.
SELECT bracketID, creationTime, completionTime, tournamentName, pointsEarned, trueMax, potentialLost, 
maxRound, trueMax-potentialLost AS bracketMax, c.competitorName AS champName FROM 
(
SELECT b.bracketID, b.creationTime, b.completionTime, t.tournamentID, t.name AS tournamentName, 
SUM(IF(bgp._fk_competitor=g._fk_competitor_winner, r.rewardPoints, 0)) AS 'pointsEarned', SUM(r.rewardPoints) AS 'trueMax', 
SUM(IF(team_loss._fk_competitor IS NULL, 0, r.rewardPoints)) AS 'potentialLost', MAX(r.round) AS maxRound
FROM user_bracket b
        JOIN tournament t ON t.tournamentID=b._fk_tournament
        JOIN tournament_game g ON t.tournamentID=g._fk_tournament
        JOIN bracket_game_prediction bgp ON b.bracketID=bgp._fk_bracket AND g.gameID=bgp._fk_game
        JOIN tournament_round r ON t._fk_type=r._fk_tournamentType AND r.round=g._fk_tournamentRound
		LEFT OUTER JOIN (
        #todo: i want to add constraints to this - as of now, this subquery will go across all tournaments all time. not just ones that user is in. that seems bad. 
			SELECT g.gameID, g._fk_tournamentRound, gp._fk_competitor, g._fk_tournament FROM tournament_game g
			JOIN game_participant gp ON gp._fk_game=g.gameID AND g._fk_competitor_winner<>gp._fk_competitor
			WHERE g._fk_competitor_winner IS NOT NULL
        ) team_loss ON g._fk_tournament=team_loss._fk_tournament AND g._fk_tournamentRound >= team_loss._fk_tournamentRound AND bgp._fk_competitor=team_loss._fk_competitor
        WHERE b.completed IS TRUE AND b._fk_user=@userID
	GROUP BY b.bracketID
        ORDER BY b.completionTime DESC
) summary
JOIN tournament_game champ_game ON champ_game._fk_tournament=summary.tournamentID AND champ_game._fk_tournamentRound=summary.maxRound
JOIN bracket_game_prediction bgp ON summary.bracketID=bgp._fk_bracket AND bgp._fk_game=champ_game.gameID
JOIN competitor_tournament c ON bgp._fk_competitor=c.competitorID AND c._fk_tournament=champ_game._fk_tournament;
        
        
# now i need to figure out the max. 

# i really want team and round maybe? 

# i really want to add constraints to this. like this will select from all tournaments every which is less than ideal. 
SELECT g.gameID, g._fk_tournamentRound, gp._fk_competitor, g._fk_tournament
FROM tournament_game g
JOIN game_participant gp ON gp._fk_game=g.gameID AND g._fk_competitor_winner<>gp._fk_competitor
WHERE g._fk_competitor_winner IS NOT NULL;

 SELECT bracketID, creationTime, completionTime, tournamentName, pointsEarned, trueMax, potentialLost, 
maxRound, trueMax-potentialLost AS bracketMax, c.competitorName AS champName FROM 
(
SELECT b.bracketID, b.creationTime, b.completionTime, t.tournamentID, t.name AS tournamentName, 
SUM(IF(bgp._fk_competitor=g._fk_competitor_winner, r.rewardPoints, 0)) AS 'pointsEarned', SUM(r.rewardPoints) AS 'trueMax', 
SUM(IF(team_loss._fk_competitor IS NULL, 0, r.rewardPoints)) AS 'potentialLost', MAX(r.round) AS maxRound
FROM user_bracket b
        JOIN tournament t ON t.tournamentID=b._fk_tournament
        JOIN tournament_game g ON t.tournamentID=g._fk_tournament
        JOIN bracket_game_prediction bgp ON b.bracketID=bgp._fk_bracket AND g.gameID=bgp._fk_game
        JOIN tournament_round r ON t._fk_type=r._fk_tournamentType AND r.round=g._fk_tournamentRound
		LEFT OUTER JOIN (
        #todo: i want to add constraints to this - as of now, this subquery will go across all tournaments all time. not just ones that user is in. that seems bad. 
			SELECT g.gameID, g._fk_tournamentRound, gp._fk_competitor, g._fk_tournament FROM tournament_game g
			JOIN game_participant gp ON gp._fk_game=g.gameID AND g._fk_competitor_winner<>gp._fk_competitor
			WHERE g._fk_competitor_winner IS NOT NULL
        ) team_loss ON g._fk_tournament=team_loss._fk_tournament AND g._fk_tournamentRound >= team_loss._fk_tournamentRound AND bgp._fk_competitor=team_loss._fk_competitor
        WHERE b.completed IS TRUE AND b._fk_user=@userID
	GROUP BY b.bracketID
) summary
JOIN tournament_game champ_game ON champ_game._fk_tournament=summary.tournamentID AND champ_game._fk_tournamentRound=summary.maxRound
JOIN bracket_game_prediction bgp ON summary.bracketID=bgp._fk_bracket AND bgp._fk_game=champ_game.gameID
JOIN competitor_tournament c ON bgp._fk_competitor=c.competitorID AND c._fk_tournament=champ_game._fk_tournament
ORDER BY summary.completionTime DESC;


        

