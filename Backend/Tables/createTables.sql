CREATE TABLE users(
    userID int,
    age int,
    sex varchar(1),
    nestID varchar(255),
    fitbitID varchar(255)
);
INSERT into users values(
    100,
    19,
    'm',
    "nestId",
    "fitbitID"
);

CREATE TABLE dailyQuizes(
    userID int,
    sleepQuality int,
    disturbance boolean,
    disturbanceDetails varchar(500),
    sleepTime datetime,
    wakeTime datetime
);


