USE bracket_app;
DROP TABLE user_token;
CREATE TABLE `user_token` (
  `_fk_user` varchar(45) NOT NULL,
  `tokenID` varchar(45) NOT NULL,
  `createTime` datetime(6) NOT NULL,
  `revokedTime` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`tokenID`),
  KEY `user_idx_token` (`_fk_user`,`createTime`,`revokedTime`),
  CONSTRAINT `user_token_fk` FOREIGN KEY (`_fk_user`) REFERENCES `user` (`userID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
