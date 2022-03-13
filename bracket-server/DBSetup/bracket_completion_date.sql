ALTER TABLE user_bracket ADD COLUMN completionTime DATETIME(6) DEFAULT NULL;
UPDATE user_bracket SET completionTime=now(6);