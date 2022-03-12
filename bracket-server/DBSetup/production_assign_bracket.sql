INSERT INTO string_params(paramKey, paramValue)
SELECT "activetournament", tournamentID
FROM tournament WHERE name="taco";