CREATE TABLE ken_pom_data(
	_fk_tournament VARCHAR(45) NOT NULL,
    _fk_competitor VARCHAR(45) NOT NULL,
    offensiveEfficiency DOUBLE DEFAULT NULL,
    defensiveEfficiency DOUBLE DEFAULT NULL,
    overallEfficiency DOUBLE DEFAULT NULL,
    tempo DOUBLE DEFAULT NULL,
    PRIMARY KEY(_fk_tournament, _fk_competitor),
    CONSTRAINT ken_pom_fk FOREIGN KEY(_fk_tournament, _fk_competitor) REFERENCES competitor_tournament(_fk_tournament, competitorID)
);