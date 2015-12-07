#VIEW 1 - Shows all friendships (no blocks included) between all users
CREATE VIEW Friendships As
SELECT 
	#Sender
	(SELECT u.userNum FROM users u WHERE u.userNum = r.sender) As 'sender', 
    #Receiver
    (SELECT u.userNum FROM users u WHERE u.userNum = r.receiver) As 'receiver', 
    #Friendship
    (SELECT d.relData FROM reldata d WHERE d.relDataNum = r.relDataNum) As 'rel' 
FROM relationships r
WHERE r.relDataNum = 0;

#View 2 - Shows user statistics for displaying on their page
CREATE VIEW Pages As
SELECT
	#Show the user number as 'user'
	u.userNum As 'user',
    #Show number of posts this user has made
	(SELECT COUNT(post.postNum) FROM posts post WHERE post.posterNum = u.userNum) As 'posts',
    #Show total number of likes on all posts this user has made
    (SELECT COUNT(l.likeNum) FROM likes l, posts post WHERE l.parentPostNum = post.postNum And post.posterNum = u.userNum) As 'likes',
    #Show number of friends this user has
    (SELECT COUNT(f.sender) FROM Friendships f WHERE (f.sender = u.userNum Or f.receiver = u.userNum)) As 'friends'
FROM users u
GROUP BY u.userNum #Remove duplicate pages
;