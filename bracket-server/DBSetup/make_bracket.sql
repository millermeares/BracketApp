USE bracket_app;
CREATE TABLE `tournament` (
  `tournamentID` varchar(45) NOT NULL,
  `name` varchar(255) NOT NULL,
  `eventStart` datetime,
  `eventEnd` datetime, 
  PRIMARY KEY(tournamentID)
);

CREATE TABLE `tournament_round` (
  `_fk_tournament` varchar(45) NOT NULL,
  `round` integer NOT NULL,
  `name` varchar(255) NOT NULL, 
  PRIMARY KEY(_fk_tournament, round), 
  CONSTRAINT round_t_fk FOREIGN KEY(_fk_tournament) REFERENCES tournament(tournamentID)
);

CREATE TABLE `competitor` (
  `competitorID` varchar(45) NOT NULL,
  name varchar(255) NOT NULL,
  PRIMARY KEY(competitorID),
  UNIQUE INDEX competitor_unique_name(name)
);

CREATE TABLE `competitor_tournament` (
  `_fk_tournament` varchar(45) NOT NULL,
  `_fk_competitor` varchar(45) NOT NULL, 
  PRIMARY KEY(_fk_tournament, _fk_competitor),
  CONSTRAINT competitor_t_fk FOREIGN KEY(_fk_tournament) REFERENCES tournament(tournamentID),
  CONSTRAINT competitor_c_fk FOREIGN KEY(_fk_competitor) REFERENCES competitor(competitorID)
);

CREATE TABLE `tournament_game` (
  `_fk_tournament` VARCHAR(45) NOT NULL,
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
  CONSTRAINT round_game_fk FOREIGN KEY(_fk_tournament, _fk_tournamentRound) REFERENCES tournament_round(_fk_tournament, round),
  CONSTRAINT left_game_fk FOREIGN KEY (_fk_leftGame) REFERENCES tournament_game(gameID),
  CONSTRAINT right_game_fk FOREIGN KEY (_fk_rightGame) REFERENCES tournament_game(gameID)
);

CREATE TABLE `game_participant` (
  `_fk_competitor` varchar(45) NOT NULL,
  `_fk_game` varchar(45) NOT NULL,
  `score` INTEGER,
  PRIMARY KEY(_fk_game, _fk_competitor),
  CONSTRAINT game_participant_game_fk FOREIGN KEY (_fk_game) REFERENCES tournament_game(gameID),
  CONSTRAINT game_participant_competitor_fk FOREIGN KEY (_fk_competitor) REFERENCES competitor(competitorID)
);
ALTER TABLE tournament_game ADD CONSTRAINT game_winner_fk FOREIGN KEY(gameID, _fk_competitor_winner) REFERENCES game_participant(_fk_game, _fk_competitor);