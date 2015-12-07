#QUERY 1 - Get 20 of the most recent posts from your friends
SELECT
	u.userName,
	u.userNum,
	p.postData,
	p.postDate,
	(SELECT COUNT(l.likeNum) FROM likes l WHERE l.parentPostNum = p.postNum) As 'likes'
FROM 
	users u,
	posts p,
	friendships f
WHERE
	#Where Dan (401) is on either side of relationship
	(f.sender = 401 Or f.receiver = 401)
    #Depending on if Dan is sender or receiver, the poster is the opposite
	And u.userNum = IF(f.sender = 401, f.receiver, f.sender)
	And p.posterNum = u.userNum    
ORDER BY p.postDate DESC
LIMIT 0, 20
;

#QUERY 2 - Get comments and comment number on a post
SELECT
	#Increment result rownum after each result
	@rownum:=@rownum+1 comNum,
    u.userName,
    c.commentData,
    c.commentDate
FROM
	#Create new table where rownum starts at 0
	(SELECT @rownum:=0) r,
    users u,
    posts p,
    comments c
WHERE
	u.userNum = c.commenterNum
    And c.parentPostNum = p.postNum
    And p.postNum = 2007 #Mandolin's Post ('I am awesomely lame!')
ORDER BY c.commentDate ASC #We want oldest comments first 'ascending'
;

#QUERY 3 - Suggested Friends (friend of a friend but not your friend (yet?)) 
SELECT
	#Check sides of mutual friend and suggested friend, return the suggested
	IF(f.sender = friend.num, f.receiver, f.sender) As 'suggested',
    #Use the above value and find the username of the suggested friend
    (SELECT u.userName FROM users u WHERE u.userNum = suggested) As 'futureFriend?'
FROM
	users u, 
	friendships f, 
	(SELECT  #Current user's friends 
		#Current user is Dan (401) compare it to his friend (num)
		IF(fs.sender = 401, fs.receiver, fs.sender) As 'num' 
	FROM 
		friendships fs
	WHERE 
		(fs.sender = 401 Or fs.receiver = 401) 
	GROUP BY num) friend
WHERE
	#Find friends of 'friend' but make sure we exclude Dan (401)
	(f.sender = friend.num  And f.receiver != 401) 
	Or (f.receiver = friend.num And f.sender != 401)
GROUP BY suggested
LIMIT 0, 20 #Only return 20 suggested friends max
;
        
#QUERY 4 - Show my posts
SELECT
	u.userName,
    p.postData,
    p.postDate,
    #Counts total likes on this post
    (SELECT COUNT(l.likeNum) FROM likes l WHERE l.parentPostNum = p.postNum) As 'likes'
FROM 
	users u,
    posts p
WHERE
	#My userNum is 401 (Dan), so select me and my posts
	u.userNum = 401
    And p.posterNum = 401
ORDER BY p.postDate DESC #Newest posts first
LIMIT 0, 20 #20 most recent
;

#QUERY 5 - View relationship (Posts that one user made, that their friend liked) with a friend (Dan - 401, Mandolin - 287)
SELECT
	#Poster of the relationship post (Dan or Mandolin)
	u.userName As 'posterName',
    #Friend of the poster (opposite to first userName)
    (SELECT us.userName FROM users us WHERE us.userNum = fl.likerNum And fl.parentPostNum = p.postNum) As 'friendName',
    #Same post data as all other post-related queries (data, date, total likes)
    p.postData,
    p.postDate,
    (SELECT COUNT(l.likeNum) FROM likes l WHERE l.parentPostNum = p.postNum) As 'likes'
FROM
	users u,
    posts p,
    likes fl,
    friendships f
WHERE
	#First section of OR checks if Dan posted and Mandolin liked
	((u.userNum = f.sender
    And
	p.posterNum = f.sender
    And
    fl.likerNum = f.receiver
    And
    fl.parentPostNum = p.postNum
    )
    OR
    #Second checks the reverse
    (u.userNum = f.receiver
    And
	p.posterNum = f.receiver
    And
    fl.likerNum = f.sender
    And
    fl.parentPostNum = p.postNum    
    ))
    And
		#Verify that we only show Dan and Mandolin's relationship posts
		((f.sender = 401 And f.receiver = 287) 
		Or (f.sender = 287 And f.receiver = 401))
GROUP BY p.postNum #Group identical results
;
