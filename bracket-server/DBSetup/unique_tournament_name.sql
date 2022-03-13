ALTER TABLE tournament ADD UNIQUE INDEX unique_name(_fk_type, name);
ALTER TABLE tournament ADD COLUMN createdTime DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP);
