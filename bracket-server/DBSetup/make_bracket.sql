USE bracket_app;
CREATE TABLE tournament_type (
	tournamentType VARCHAR(45) NOT NULL DEFAULT "NCAA64Basketball" PRIMARY KEY
);
INSERT INTO tournament_type(tournamentType) VALUES("NCAA64Basketball");

CREATE TABLE `tournament` (
  `tournamentID` varchar(45) NOT NULL,
  `name` varchar(255) NOT NULL,
  `eventStart` datetime,
  `eventEnd` datetime, 
  PRIMARY KEY(tournamentID)
);
ALTER TABLE tournament ADD COLUMN creator VARCHAR(45) NOT NULL, 
ADD CONSTRAINT creator_fk FOREIGN KEY(creator) REFERENCES user(userID), 
ADD INDEX tournament_name_idx(name, tournamentID), 
ADD COLUMN _fk_type VARCHAR(45) NOT NULL DEFAULT "NCAA64Basketball", 
ADD INDEX t_type_idx(_fk_type), 
ADD CONSTRAINT type_fk FOREIGN KEY(_fk_type) REFERENCES tournament_type(tournamentType);

CREATE TABLE `tournament_round` (
  `_fk_tournamentType` varchar(45) NOT NULL,
  `round` integer NOT NULL,
  `name` varchar(255) NOT NULL, 
  PRIMARY KEY(_fk_tournamentType, round), 
  
  CONSTRAINT round_t_fk FOREIGN KEY(_fk_tournamentType) REFERENCES tournament_type(tournamentType)
);
ALTER TABLE tournament_round ADD INDEX round_int_idx(round, _fk_tournamentType);
INSERT INTO tournament_round(_fk_tournamentType, round, name) 
VALUES
("NCAA64Basketball", 1, "Seed Round"),
("NCAA64Basketball", 2, "Round of 32"),
("NCAA64Basketball", 3, "Sweet 16"),
("NCAA64Basketball", 4, "Elite 8"),
("NCAA64Basketball", 5, "Final 4"),
("NCAA64Basketball", 6, "Championship");

CREATE TABLE tournament_division(
	_fk_tournamentType VARCHAR(45) NOT NULL,
    divisionName VARCHAR(45) NOT NULL,
    PRIMARY KEY(_fk_tournamentType, divisionName),
    CONSTRAINT divsion_t_fk FOREIGN KEY(_fk_tournamentType) REFERENCES tournament_type(tournamentType), 
    INDEX d_name_idx(divisionName, _fk_tournamentType)
);
# COULD PUT DISPLAY INFO ON THIS TABLE?
INSERT INTO tournament_division(_fk_tournamentType, divisionName)
VALUES
("NCAA64Basketball", "EAST"),
("NCAA64Basketball", "South"),
("NCAA64Basketball", "West"),
("NCAA64Basketball", "Midwest");



CREATE TABLE seed (
	seedID INT PRIMARY KEY, 
    _fk_tournamentType VARCHAR(45) NOT NULL DEFAULT "NCAA64Basketball", 
	CONSTRAINT seed_t_fk FOREIGN KEY(_fk_tournamentType) REFERENCES tournament_type(tournamentType)
);
INSERT INTO seed(seedID) VALUES(1), (2), (3), (4), (5), (6), (7), (8), (9), (10), (11), (12), (13), (14), (15), (16);

# i don't love hard-coding these for ncaa only but not dealing with it now.
CREATE TABLE seed_data (
	_fk_seed INT NOT NULL,
    FinalFourOdds DOUBLE, 
    EliteEightOdds DOUBLE,
	PRIMARY KEY(_fk_seed), 
    CONSTRAINT seed_data_fk FOREIGN KEY(_fk_seed) REFERENCES seed(seedID)
);


CREATE TABLE `competitor_tournament` (
  `_fk_tournament` varchar(45) NOT NULL,
  _fk_division VARCHAR(45) NOT NULL,
  _fk_seed INT NOT NULL,
  `competitorID` varchar(45) NOT NULL, 
  competitorName VARCHAR(100
  ) NOT NULL,
  PRIMARY KEY(_fk_tournament, competitorID),
  UNIQUE INDEX unique_name_idx(_fk_tournament, competitorName),
  INDEX competitor_idx(competitorID),
  CONSTRAINT competitor_t_fk FOREIGN KEY(_fk_tournament) REFERENCES tournament(tournamentID),
  CONSTRAINT competitor_d_fk FOREIGN KEY(_fk_division) REFERENCES tournament_division(divisionName),
  CONSTRAINT competitor_s_fk FOREIGN KEY(_fk_seed) REFERENCES seed(seedID)
);
CREATE TABLE `tournament_game` (
  `_fk_tournament` VARCHAR(45) NOT NULL,
  `_fk_division` VARCHAR(45) DEFAULT NULL,
  `_fk_tournamentround` INTEGER NOT NULL,
  `gameID` varchar(45) NOT NULL,
  `_fk_leftGame` VARCHAR(45) DEFAULT NULL,
  `_fk_rightGame` VARCHAR(45) DEFAULT NULL,
  `_fk_competitor_winner` varchar(45) DEFAULT NULL, 
  PRIMARY KEY(gameID),
  INDEX game_round_idx(_fk_tournament, _fk_tournamentRound), 
  INDEX game_winner_idx(_fk_tournament, _fk_competitor_winner), 
  INDEX left_game_idx(_fk_leftGame),
  INDEX right_game_idx(_fk_rightGame),
  CONSTRAINT round_game_fk FOREIGN KEY(_fk_tournamentRound) REFERENCES tournament_round(round),
  CONSTRAINT left_game_fk FOREIGN KEY (_fk_leftGame) REFERENCES tournament_game(gameID),
  CONSTRAINT right_game_fk FOREIGN KEY (_fk_rightGame) REFERENCES tournament_game(gameID), 
  CONSTRAINT game_division_fk FOREIGN KEY(_fk_division) REFERENCES tournament_division(divisionName)
);

CREATE TABLE `game_participant` (
  `_fk_competitor` varchar(45) NOT NULL,
  `_fk_game` varchar(45) NOT NULL,
  `score` INTEGER,
  PRIMARY KEY(_fk_game, _fk_competitor),
  CONSTRAINT game_participant_game_fk FOREIGN KEY (_fk_game) REFERENCES tournament_game(gameID),
  CONSTRAINT game_participant_competitor_fk FOREIGN KEY (_fk_competitor) REFERENCES competitor_tournament(competitorID)
);
ALTER TABLE tournament_game ADD CONSTRAINT game_winner_fk FOREIGN KEY(gameID, _fk_competitor_winner) REFERENCES game_participant(_fk_game, _fk_competitor);





