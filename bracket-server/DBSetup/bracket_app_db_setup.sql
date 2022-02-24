CREATE DATABASE bracket_app;
USE bracket_app;
CREATE TABLE user(
	userID VARCHAR(45) NOT NULL,
    username VARCHAR(200) NOT NULL,
    passwordSalt VARCHAR(45) NOT NULL,
    passwordHash VARCHAR(45) NOT NULL,
    email VARCHAR(320) NOT NULL
);