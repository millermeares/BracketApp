CREATE DATABASE bracket_app;
USE bracket_app;

CREATE TABLE user(
	userID VARCHAR(45) NOT NULL,
    username VARCHAR(200) NOT NULL,
    passwordSalt VARCHAR(45) NOT NULL,
    passwordHash BINARY(128) NOT NULL,
    email VARCHAR(320) NOT NULL, 
    PRIMARY KEY (userID),
    UNIQUE KEY email_unique(email),
    UNIQUE KEY username_unique(username)
);

CREATE TABLE `user_token` (
  `_fk_user` varchar(45) NOT NULL,
  `tokenID` varchar(45) NOT NULL,
  `createTime` datetime(6) NOT NULL,
  `revokedTime` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`tokenID`),
  KEY `user_idx_token` (`_fk_user`,`createTime`,`revokedTime`),
  CONSTRAINT `user_token_fk` FOREIGN KEY (`_fk_user`) REFERENCES `user` (`userID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE error_log (
	startDate DATETIME(6) NOT NULL DEFAULT NOW(6),
    message VARCHAR(1000),
    callstack VARCHAR(1000)
);
