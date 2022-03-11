CREATE TABLE role(
roleKey VARCHAR(45) NOT NULL,
PRIMARY KEY(roleKey)
);
INSERT ignore INTO role(roleKey) VALUES("Admin"), ("Developer");
CREATE TABLE user_role(
	_fk_user VARCHAR(45) NOT NULL,
    _fk_role VARCHAR(45) NOT NULL,
    PRIMARY KEY(_fk_user, _fk_role),
	CONSTRAINT `user_foreign_key` FOREIGN KEY (`_fk_user`) REFERENCES `user` (`userID`),
	CONSTRAINT `role_foreign_key` FOREIGN KEY (`_fk_role`) REFERENCES `role` (`roleKey`)
);

INSERT INTO user_role(_fk_user, _fk_role)
SELECT userId, "Developer" from user where email="benmillermeares3@gmail.com";
INSERT INTO user_role(_fk_user, _fk_role)
SELECT userId, "Admin" from user where email="benmillermeares3@gmail.com";

INSERT INTO user_role(_fk_user, _fk_role)
SELECT userId, "Admin" FROM user where email="rbkillen88@gmail.com";
