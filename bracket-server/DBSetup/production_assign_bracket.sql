DELETE FROM string_params WHERE paramKey="activetournament";

INSERT INTO string_params(paramKey, paramValue)
SELECT "activetournament", tournamentID
FROM tournament WHERE name="testtournament3";
