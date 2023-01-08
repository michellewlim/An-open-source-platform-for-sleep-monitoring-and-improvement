select userID, age, sex, nestID, FitbitID from users where userID = 1;

select users.userID, age, sex, nestID, users.fitbitID, fitbitUsers.accessToken, fitbitUsers.refreshToken from users left join fitbitUsers on users.fitbitID = fitbitUsers.userID;