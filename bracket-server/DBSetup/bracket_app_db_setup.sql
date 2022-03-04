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

CREATE TABLE user_token(
	_fk_user VARCHAR(45) NOT NULL,
    tokenID VARCHAR(45) NOT NULL,
    createTime DATETIME(6) NOT NULL,
    revokedTime DATETIME(6) DEFAULT NULL,
    PRIMARY KEY(tokenID, _fk_user),
    INDEX user_idx_token(_fk_user, createTime, revokedTime),
    CONSTRAINT user_token_fk FOREIGN KEY(_fk_user) REFERENCES user(userID)
);

CREATE TABLE error_log (
	startDate DATETIME(6) NOT NULL DEFAULT NOW(6),
    message VARCHAR(1000),
    callstack VARCHAR(1000)
);
