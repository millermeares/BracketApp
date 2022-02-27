USE bracket_app;
select * from user;

delete from user where email="benmillermeares3@gmail.com";

select * from user_token;
UPDATE user_token SET loggedOutTime=now(6)
WHERE _fk_user=@user AND DATE_SUB(UTC_TIMESTAMP(), INTERVAL 15 MINUTE) > expireTime AND revokedTime IS NULL;
INSERT INTO user_token(_fk_user, tokenID, createTime)
VALUES(@user, @token, @utc_now);
SELECT _fk_user, tokenID, expireTime, revokedTime FROM user_token 
WHERE _fk_user=@user AND DATE_SUB(UTC_timestamp(), INTERVAL 15 MINUTE) > expireTime AND revokedTime IS NULL
ORDER BY expireTime DESC 
LIMIT 1;
