CREATE TABLE bracket_result(
	_fk_bracket VARCHAR(45) NOT NULL,
    bracketMax DOUBLE NOT NULL,
    pointsEarned DOUBLE NOT NULL,
    PRIMARY KEY(_fk_bracket),
    CONSTRAINT bracket_fk FOREIGN KEY(_fk_bracket) REFERENCES user_bracket(bracketID)
);

