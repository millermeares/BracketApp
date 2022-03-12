CREATE TABLE string_params(
	paramKey VARCHAR(45) NOT NULL,
	paramValue VARCHAR(200) NOT NULL,
    endDate DATETIME(6) NOT NULL DEFAULT '9999-12-31 23:59:59',
	PRIMARY KEY(paramKey, endDate)
);