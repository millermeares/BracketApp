DELETE FROM string_params WHERE paramKey="activetournament";

INSERT INTO string_params(paramKey, paramValue)
SELECT "activetournament", tournamentID
FROM tournament WHERE tournamentid="05501964-c6da-4542-bee3-7edd307ed575";
