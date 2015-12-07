# Update someone's name
UPDATE users u SET u.userName='MANDOLIN_LI_BARTLING' WHERE u.userName='Ed_MonkeyMan_Samantha';

#Unblock all of a user's blocked friends
UPDATE relationships r, users u SET r.relDataNum=0 
WHERE 
	((r.sender = u.userNum And r.relDataNum = 1) Or (r.receiver = u.userNum And r.relDataNum = 2)) 
    And u.userNum = 396;
    
#Delete all posts from today for a user
DELETE FROM posts 
WHERE
	(postDate > DATE_SUB(NOW(), INTERVAL 1 DAY))
    And posterNum = 401;