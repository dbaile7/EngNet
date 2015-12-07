#Inserts a new user into users
INSERT users VALUES(
	'Daniel_Everett_Bailey',
    'dbaile7@uwo.ca',
    'password',
    1
);

#Inserts new post and references columns to fill
INSERT INTO posts (
	posterNum,
    postData
) VALUES (
	100,
    'Hey guys my password is: ' + (SELECT userPass FROM users WHERE userNum = 100)
);

#Creates a relationship between user 100 and 200 and select reldata from the reldata table
INSERT INTO relationships (
	sender,
    receiver,
    relDataNum
) VALUES (
	100,
    200,
    (SELECT reldata.relDataNum FROM reldata WHERE reldata.relData = 'FRIENDS')
);
