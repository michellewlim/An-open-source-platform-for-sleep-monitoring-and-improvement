CREATE TABLE users(
    userID int primary key,
    age int,
    sex varchar(1),
    nestID varchar(255) unique,
    fitbitID varchar(255) unique
);

CREATE TABLE dailyQuizes(
    userID int,
    sleepQuality int,
    disturbance boolean,
    disturbanceDetails varchar(500),
    sleepTime datetime,
    wakeTime datetime
);

CREATE TABLE fitbitUsers(
    userID varchar(255) primary key,
    accessToken varchar(500),
    refreshToken varchar(255)
);
