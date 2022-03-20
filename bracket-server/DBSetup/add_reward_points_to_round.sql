ALTER TABLE tournament_round ADD COLUMN rewardPoints INT DEFAULT 0;
UPDATE tournament_round SET rewardPoints=10 WHERE round=1;
UPDATE tournament_round SET rewardPoints=20 WHERE round=2;
UPDATE tournament_round SET rewardPoints=40 WHERE round=3;
UPDATE tournament_round SET rewardPoints=80 WHERE round=4;
UPDATE tournament_round SET rewardPoints=160 WHERE round=5;
UPDATE tournament_round SET rewardPoints=320 WHERE round=6;