INSERT into users values(
    1,
    19,
    'm',
    "nest1",
    "fitbit1"
);

INSERT INTO fitbitUsers values(
    "fitbit1",
    "access",
    "refresh"
);

UPDATE users 
SET fitbitID = ?fitbitID
WHERE users.userID = ?userID;


UPDATE users 
SET fitbitID = 'fitbit1'
WHERE users.userID = 1;