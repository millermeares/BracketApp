use bracket_app;
select * from tournament_division;
INSERT INTO tournament_division(_fk_tournamentType, divisionName)
VALUES
("NCAA64Basketball", "A"),
("NCAA64Basketball", "B"),
("NCAA64Basketball", "C"),
("NCAA64Basketball", "D");
UPDATE competitor_tournament SET _fk_division="A" WHERE _fk_division="WEST";
UPDATE tournament_game SET _fK_division="A" WHERE _fk_division="WEST";
UPDATE competitor_tournament SET _fk_division="B" WHERE _fk_division="EAST";
UPDATE tournament_game SET _fK_division="B" WHERE _fk_division="EAST";
UPDATE competitor_tournament SET _fk_division="C" WHERE _fk_division="SOUTH";
UPDATE tournament_game SET _fK_division="C" WHERE _fk_division="SOUTH";
UPDATE competitor_tournament SET _fk_division="D" WHERE _fk_division="MIDWEST";
UPDATE tournament_game SET _fK_division="D" WHERE _fk_division="MIDWEST";