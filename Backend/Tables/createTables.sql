CREATE TABLE users(
    userID int primary key,
    age int,
    sex varchar(1),
    fitbitID varchar(255) unique
);

CREATE TABLE dailyQuizes(
    userID int,
    sleepSession int,
    q1 int,
    q2 int,
    q3 int,
    q4 int,
    q5 int,
    q6 int,
    q7 int,
    wakeTime datetime
);

CREATE TABLE sleepData(
    userID int,
    sleepSession int,
    logTime datetime,
    heartBeat int,
    heartBeatLag time,
    currentTemp FLOAT(20),
    targetTemp FLOAT(20)
);

CREATE TABLE fitbitUsers(
    userID varchar(255) primary key,
    accessToken varchar(500),
    refreshToken varchar(255),
    expires datetime
);

CREATE TABLE nestUsers(
    userID varchar(255) primary key,
    accessToken varchar(500),
    refreshToken varchar(255),
    expires datetime
);