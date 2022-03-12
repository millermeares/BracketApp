
CREATE TABLE user_bracket(
	_fk_user VARCHAR(45) NOT NULL,
    _fk_tournament VARCHAR(45) NOT NULL,
    BracketID VARCHAR(45) NOT NULL,
    champTotalPoints INT DEFAULT 120,
    completed BOOLEAN DEFAULT FALSE,
    creationTime DATETIME(6) NOT NULL,
    PRIMARY KEY (_fk_user, _fk_tournament, bracketID), 
    unique index bracket_id_idx(bracketID), 
    INDEX bracket_tournament_idx(bracketID, _fk_tournament),
    CONSTRAINT bracket_user_fk FOREIGN KEY(_fk_user) REFERENCES user(userID),
    CONSTRAINT bracket_tournament_fk FOREIGN KEY(_fk_tournament) REFERENCES tournament(tournamentID)
);
ALTER TABLE tournament_game ADD INDEX game_tournament_idx(gameID, _fk_tournament); 
ALTER TABLE tournament_game ADD CONSTRAINT game_t_fk FOREIGN KEY(_fk_tournament) REFERENCES tournament(tournamentID);
CREATE TABLE bracket_game_prediction(
	_fk_bracket VARCHAR(45) NOT NULL,
    _fk_tournament VARCHAR(45) NOT NULL,
    _fk_game VARCHAR(45) NOT NULL,
    _fk_competitor VARCHAR(45) NOT NULL,
    pickTime DATETIME(6) NOT NULL,
    PRIMARY KEY(_fk_bracket, _fk_game),
    INDEX prediction_game_idx(_fk_game, _fk_tournament),
    INDEX prediction_competitor_idx(_fk_competitor, _fk_tournament),
    INDEX prediction_tournament_idx(_fk_bracket, _fk_tournament),
    CONSTRAINT prediction_bracket_fk FOREIGN KEY(_fk_bracket, _fk_tournament) REFERENCES user_bracket(bracketID, _fk_tournament),
    CONSTRAINT prediction_game_fk FOREIGN KEY(_fk_game, _fk_tournament) REFERENCES tournament_game(gameID, _fk_tournament), 
    CONSTRAINT prediction_competitor_fk FOREIGN KEY(_fk_competitor, _fk_tournament) REFERENCES competitor_tournament(competitorID, _fk_tournament)
);