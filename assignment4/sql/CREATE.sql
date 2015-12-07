#Creates our engnet schema
CREATE SCHEMA IF NOT EXISTS engnet;

#Creates our user table
#Usernames do not have to be unique but emails do
#admin is more of a debug/testing feature (hence the false default)
CREATE TABLE Users
(
userNum bigint auto_increment,
userName varchar(150) NOT NULL,
userEmail varchar(150) UNIQUE,
userPass varchar(150) NOT NULL,
admin boolean DEFAULT false,
joinDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
PRIMARY KEY (userNum)
);

#Create our posts table
#When a user is deleted, the post is also deleted
CREATE TABLE Posts
(
postNum bigint auto_increment,
posterNum bigint,
postData varchar (140) NOT NULL,
postDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
PRIMARY KEY (postNum),
FOREIGN KEY (posterNum) REFERENCES Users(userNum) ON DELETE CASCADE ON UPDATE CASCADE
);

#Create our comments table - see posts
CREATE TABLE Comments
(
commentNum bigint auto_increment,
parentPostNum bigint,
commenterNum bigint,
commentData varchar(140) NOT NULL,
commentDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
PRIMARY KEY (commentNum),
FOREIGN KEY (parentPostNum) references POSTS(postNum) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (commenterNum) references USERS(userNum) ON DELETE CASCADE ON UPDATE CASCADE
);

#Create our likes table
CREATE TABLE Likes
(
likeNum bigint auto_increment,
parentPostNum bigint,
likerNum bigint,
PRIMARY KEY (likeNum),
FOREIGN KEY (parentPostNum) REFERENCES Posts(postNum) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (likerNum) REFERENCES Users(userNum) ON DELETE CASCADE ON UPDATE CASCADE
);

#Create our messages table
CREATE TABLE Messages
(
msgNum bigint auto_increment,
sender bigint,
receiver bigint,
msgData varchar(140) NOT NULL,
msgDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
PRIMARY KEY (msgNum),
FOREIGN KEY (sender) REFERENCES Users(userNum) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (receiver) REFERENCES Users(userNum) ON DELETE CASCADE ON UPDATE CASCADE
);

#Fixed value table for relationship data
CREATE TABLE RelData
(
relDataNum bigint,
relData varchar(50) NOT NULL,
PRIMARY KEY(relDataNum)
);

#Create our relationships table
CREATE TABLE Relationships
(
relNum bigint auto_increment,
sender bigint,
receiver bigint,
relDataNum bigint,
PRIMARY KEY (relNum),
FOREIGN KEY (sender) REFERENCES Users(userNum) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (receiver) REFERENCES Users(userNum) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (relDataNum) REFERENCES RelData(relDataNum) ON UPDATE CASCADE
);