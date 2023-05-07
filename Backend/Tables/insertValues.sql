INSERT into users values(
    1,
    19,
    'm',
    "nest1",
    "fitbit1"
);

INSERT INTO fitbitUsers values(
    "userID",
    "AccessToken",
    "refreshToken"
);

UPDATE users 
SET fitbitID = ?fitbitID
WHERE users.userID = ?userID;


UPDATE users 
SET fitbitID = 'fitbit1'
WHERE users.userID = 1;

UPDATE fitbitUsers
SET accessToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIyMzkyRFgiLCJzWdWIiOiJCRENUSkwiLCJpc3MiOiJGaXRiaXQiLCJ0eXAiOiJhY2Nlc3NfdG9rZW4iLCJzY29wZXMiOiJyc29jIHJzZXQgcm94eSBycHJvIHJudXQgcnNsZSByYWN0IHJsb2MgcnJlcyByd2VpIHJociBydGVtIiwiZXhwIjoxNjgxMzY0NzM4LCJpYXQiOjE2ODEzMzU5Mzh9.EIq1lBIsMzmtjWUiI2JwOFgM3uGBZDoh6JSiKlIJYAc",
refreshToken = "ff4aad31a48ccfe2fda21fc44cd465978e0c4b8e8d03427e911ed871a06c6de3"
WHERE userID = "BDCTJL";


INSERT into sleepData values(1, 0, "2023-01-07T09:00:39", 65, "00:01:43", 22.543, 23);
