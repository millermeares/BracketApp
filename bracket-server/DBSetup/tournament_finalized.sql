USE bracket_app;
ALTER TABLE tournament ADD COLUMN tournamentFinalized BOOL DEFAULT FALSE;
ALTER TABLE error_log DROP COLUMN callstack,
ADD COLUMN callstack longtext;