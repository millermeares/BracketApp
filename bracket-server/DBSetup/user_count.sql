SELECT * FROM USER;
SELECT * FROM user_token ORDER BY createtime DESC;

SELECT * FROM bracket_game_prediction;

select u.username, u.email, count(b.bracketID) from user u 
LEFT OUTER JOIN user_bracket b ON u.userID=b._fk_user
#left outer join bracket_game_prediction bgp on b.bracketid=bgp._fk_bracket
GROUP BY u.userID;