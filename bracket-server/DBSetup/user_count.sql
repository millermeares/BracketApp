SELECT * FROM USER;
SELECT * FROM user_token ORDER BY createtime DESC;

SELECT * FROM bracket_game_prediction;

select u.username, u.email, count(b.bracketID) from user u 
LEFT OUTER JOIN user_bracket b ON u.userID=b._fk_user
#left outer join bracket_game_prediction bgp on b.bracketid=bgp._fk_bracket
GROUP BY u.userID;

set @userid="bf1df4a6-df78-4f9c-a14e-3f3b7b88cc98";
set @tournamentID="74c2a92a-9e04-4f79-898a-58bba73c7abc";
select * from tournament;
select competitorID, competitorName, _fk_seed, count(competitorID) AS 'winCount', AVG(b.completionTime) from user_bracket b 
join bracket_game_prediction p on p._fk_bracket=b.bracketid
join tournament_game g on g.gameid=p._fk_game
join competitor_tournament c on c._fk_tournament=b._fk_tournament AND c.competitorid=p._fk_competitor
where b._fk_user=@userID and g._fk_tournamentRound=6 AND b._fk_tournament=@tournamentId
group by competitorid, competitorName, _fk_seed
ORDER BY _fk_seed, competitorName;


select * from user_role;
select * from user;
insert into user_role(_fk_user, _fk_role)
VALUES("094570b5-cff4-4364-84ad-05bcb2d9a7ac", "Admin");