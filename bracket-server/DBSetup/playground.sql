USE bracket_app;
select * from user;

delete from user where email="benmillermeares3@gmail.com";

select * from user_token;
set @user="cf79a314-e3f6-4448-833a-29d6984c311d";

SELECT _fk_user, tokenID, createTime, revokedTime FROM user_token 
        WHERE _fk_user=@user AND DATE_ADD(createTime, INTERVAL 15 MINUTE) > UTC_TIMESTAMP() AND revokedTime IS NULL
        ORDER BY createTime DESC 
        LIMIT 1;
        
        
select * from error_log;
select * from tournament
;
SELECT name, tournamentID from tournament;

SELECT * FROM competitor_tournament;
set sql_safe_updates=1;
  delete from competitor_tournament where competitorname="teamsameseed";