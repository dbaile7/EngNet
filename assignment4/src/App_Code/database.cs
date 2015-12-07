using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace engNET
{
    //create a user class
    public class User
    {
        public long userNum;
        public string userName;
        public string userEmail;

        public User(long num, string name, string email)
        {
            userNum = num;
            userName = name;
            userEmail = email;
        }
    }
    //create a post class
    public class Post
    {
        public long id;
        public long posterNum;
        public string posterName;
        public string data;
        public string date;
        public List<Comment> comments;

        public Post(long pId, long pNum, string pName, string pData, string pDate)
        {
            id = pId;
            posterNum = pNum;
            posterName = pName;
            data = pData;
            date = Database.FormatTimestamp(pDate, 3); //3 hours forwards
            comments = Database.LoadComments(pId.ToString());
        }


    }
    //create a comment class
    public class Comment
    {
        public long id;
        public string name;
        public string data;
        public string date;

        public Comment(long cId, string uName, string cData, string cDate)
        {
            id = cId;
            name = uName;
            data = cData;
            date = Database.FormatTimestamp(cDate, 3); //3 hours forwards
        }


    }

    //classs which handles interaction with db
    public class Database
    {
        //form connection with db
        public static string connStr = "Server=67.231.21.30;Database=engnet;Uid=connector;Pwd=Famz29!2;";      

        //parses the timestamp into a readable format
        public static string FormatTimestamp(string timestamp, int hourdiff)
        {
            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            //'2015/12/04 2:06:26 PM'
            string date = timestamp.Split(' ')[0];
            string time = timestamp.Split(' ')[1];
            //date
            string year = date.Split('/')[2];
            string month = months[Convert.ToInt16(date.Split('/')[0]) - 1];
            string day = (Convert.ToInt16(date.Split('/')[1])).ToString();
            //time
            string daytime = timestamp.Split(' ')[2];
            int hour = Convert.ToInt16(time.Split(':')[0]);
            hour += hourdiff; //account for time zone difference between user and db
            if (hour > 12)
            {
                hour -= 12;
                daytime = "PM";
            }
            else if (hour < 0)
            {
                hour += 12;
                daytime = "PM";
            }
            string min = (Convert.ToInt16(time.Split(':')[1])).ToString();

            if (min.Length < 2)
                min = "0" + min;
           

            return (hour.ToString() + ':' + min.ToString() + ' ' + daytime + " - " + month + " " + day + ", " + year);
        }
        //handles query to get a username given their id
        public static string GetUserName(string userid)
        {
            string query = "SELECT `userName` FROM `users` WHERE `userNum` = '" + userid + "';";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                string name = (string)cmd.ExecuteScalar();

                conn.Close();

                return name;
            }
            catch 
            {
                return null;
            }
        }

        //gets a list of friend requests given a user id
        public static List<string> GetFriendRequests(string userid)
        {
            string query = "SELECT r.relNum, u.userNum, u.userName FROM users u, relationships r, relData d " +
                "WHERE r.receiver = '" + userid + "' And r.relDataNum = d.relDataNum And d.relData = 'REQUESTED' And r.sender = u.userNum";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                List<string> data = new List<String>();

                for(int i = 0; i < dataTable.Rows.Count; i++)
                {
                    data.Add(dataTable.Rows[i][0].ToString() + ',' + dataTable.Rows[i][1].ToString() + ',' + dataTable.Rows[i][2].ToString());
                }

                return data;
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
        //get a list of friends given a user id
        public static List<string> GetFriends(string userid)
        {
            string query = "SELECT u.userNum, u.userName, r.relNum, (SELECT d.relData FROM relData d WHERE d.relDataNum = r.relDataNum) As 'relationship' " +
                            "FROM relationships r, users u " +
                            "WHERE u.userNum = if(r.sender = '" + userid + "', r.receiver, r.sender) And (r.sender = '" + userid + "' OR r.receiver = '" + userid + "') " +
                            "ORDER BY u.userName;";
            
            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                List<string> data = new List<String>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    data.Add(dataTable.Rows[i][0].ToString() + ',' + dataTable.Rows[i][1].ToString() + ',' + dataTable.Rows[i][2].ToString() + ',' + dataTable.Rows[i][3].ToString());
                }

                return data;
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
        //load the page view for a given user
        public static String LoadPage(string userid)
        {
            string query = "SELECT * FROM Pages WHERE user = '" + userid + "';";
            
            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                string data;

                if (dataTable.Rows.Count == 0)
                    return null;

                data = dataTable.Rows[0][1].ToString() + ',' + dataTable.Rows[0][2].ToString() + ',' + dataTable.Rows[0][3].ToString();

                return data;
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        //Searches for a user
        public static List<string> SearchResults(string search)
        {
            //parses the search name into parameters
            string[] search_params = search.Split(' ').ToArray();
            for(int i = 0; i < search_params.Length; i++)
            {
                if (search_params[i].Length > 0)
                {//add wildcard characters before and after each parameter
                    search_params[i] = '%' + search_params[i] + '%';
                    if(i < search_params.Length - 1)
                    {
                        search_params[i] += ' ';
                    }
                }
            }

            string query = "SELECT userNum, userName FROM users WHERE userName LIKE '";
            foreach(string s in search_params)
            {
                if(s.Length > 0)
                {
                    query += s;
                }
            }
            query += "' GROUP BY userName LIMIT 0, 20;";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                List<string> data = new List<String>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    //Num,Name
                    data.Add(dataTable.Rows[i][0].ToString() + ',' + dataTable.Rows[i][1].ToString());
                }

                return data;
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        //Loads this user's feedl, inclduing pots from their friends and themselves, in order, limited number
        public static List<Post> LoadFeed(string userid)
        {
            string query =
                "(SELECT u.userName, u.userNum, p.postData, p.postDate, p.postNum, (SELECT COUNT(l.likeNum) FROM likes l WHERE l.parentPostNum = p.postNum) As 'likes' " +
                "FROM users u, posts p, friendships f " +
                "WHERE (f.sender = '" + userid + "' Or f.receiver = '" + userid + "') " +
                    "And u.userNum = IF(f.sender = '" + userid + "', f.receiver, f.sender) " +
                    "And p.posterNum = u.userNum )" +
                "UNION (SELECT u.userName, u.userNum, p.postData, p.postDate, p.postNum, (SELECT COUNT(l.likeNum) FROM likes l WHERE l.parentPostNum = p.postNum) As 'likes' " +
                "FROM users u, posts p " +
                "WHERE u.userNum = '" + userid + "' And p.posterNum = '" + userid + "') " +
                "ORDER BY postDate DESC LIMIT 0, 20;";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                //userName, userNum, postData, postDate

                List<Post> returnList = new List<Post>();

                //dataTable contains post data
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    returnList.Add(new Post(
                        Convert.ToInt64(dataTable.Rows[i][4]),
                        Convert.ToInt64(dataTable.Rows[i][1]),
                        dataTable.Rows[i][0].ToString(),
                        dataTable.Rows[i][2].ToString(),
                        dataTable.Rows[i][3].ToString()
                        ));
                }

                return returnList;
            }
            catch (MySqlException e)
            {
                return new List<Post>();
            }
            catch
            {
                return new List<Post>();
            }
        }
        //loads 20 most recent posts for a user
        public static List<Post> LoadPosts(string userid)
        {
            string query = "SELECT u.userNum, u.userName, p.postData, p.postDate, p.postNum, " +
                           "(SELECT COUNT(l.likeNum) FROM likes l WHERE l.parentPostNum = p.postNum) As 'likes' " +
                           "FROM users u, posts p " +
                           "WHERE u.userNum = '" + userid + "' " +
                           "And p.posterNum = '" + userid + "' " +
                           "ORDER BY p.postDate DESC " +
                           "LIMIT 0, 20;";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                List<Post> returnList = new List<Post>();

                //dataTable contains post data
                for(int i = 0; i < dataTable.Rows.Count; i++)
                {                 
                    returnList.Add(new Post(Convert.ToInt64(dataTable.Rows[i][4]),
                        Convert.ToInt64(dataTable.Rows[i][0]),
                        dataTable.Rows[i][1].ToString(),
                        dataTable.Rows[i][2].ToString(),
                        dataTable.Rows[i][3].ToString()));
                }


                return returnList;
            }
            catch(MySqlException e)
            {
                return new List<Post>();
            }
            catch
            {
                return new List<Post>();
            }
        }
        //loads a list of poosts for a friendship; those posted by one friend and liked by the other
        public static List<Post> LoadFriendship(string sender, string receiver)
        {
            string query = "SELECT p.postNum, u.userNum, u.userName, p.postData, p.postDate " +
                           "FROM users u, posts p , likes fl, friendships f " +
                           "WHERE ((u.userNum = f.sender And p.posterNum = f.sender And fl.likerNum = f.receiver And fl.parentPostNum = p.postNum) Or " +
                           "(u.userNum = f.receiver And p.posterNum = f.receiver And fl.likerNum = f.sender And fl.parentPostNum = p.postNum)) And " +
                           "((f.sender = '" + sender + "' And f.receiver = '" + receiver + "') Or " +
                           "(f.sender = '" + receiver + "' And f.receiver = '" + sender + "')) " +
                           "GROUP BY p.postNum;";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                List<Post> returnList = new List<Post>();

                //dataTable contains post data
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    returnList.Add(new Post(Convert.ToInt64(dataTable.Rows[i][0]),
                        Convert.ToInt64(dataTable.Rows[i][1]),
                        dataTable.Rows[i][2].ToString(),
                        dataTable.Rows[i][3].ToString(),
                        dataTable.Rows[i][4].ToString()));
                }


                return returnList;
            }
            catch (MySqlException e)
            {
                return new List<Post>();
            }
            catch
            {
                return new List<Post>();
            }
        }
        //loads commetns for admny post given a post id
        public static List<Comment> LoadComments(string postid)
        {
            string query = "SELECT u.userNum, u.userName, c.commentData, c.commentDate FROM users u, posts p, comments c WHERE u.userNum = c.commenterNum AND c.parentPostNum = p.postNum AND p.postNum = '" + postid + @"' ORDER BY c.commentDate ASC;";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                List<Comment> returnList = new List<Comment>();

                //dataTable contains comment data
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    returnList.Add(new Comment(Convert.ToInt64(dataTable.Rows[i][0]),
                        dataTable.Rows[i][1].ToString(),
                        dataTable.Rows[i][2].ToString(),
                        dataTable.Rows[i][3].ToString()));
                }


                return returnList;
            }
            catch (MySqlException e)
            {
                return new List<Comment>();
            }
            catch
            {
                return new List<Comment>();
            }
        }
        //handles login
        public static User Login(string userEmail, string userPass)
        {
            string query = "SELECT `userNum`, `userName`, `userEmail` FROM `users` WHERE " +
                            "`userEmail` = '" + userEmail + "' And " +
                            "`userPass` = '" + userPass + "';";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                if (dataTable.Rows.Count == 0) //Not logged in
                    return null;

                User user = new User(Convert.ToInt64(dataTable.Rows[0][0]), dataTable.Rows[0][1].ToString(), dataTable.Rows[0][2].ToString());

                return user;
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null; //Unhandled exception
            }  
        }
        //handles adding a new tuple into comment table
        public static int CreateComment(string id, string commenter, string comment)
        {
            //POST THAT DATA!
            string query = "INSERT INTO comments (parentPostNum, commenterNum, commentData) VALUES (@id, @num, @comment);";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@num", commenter);
                cmd.Parameters.AddWithValue("@comment", comment);

                conn.Open();

                Object obj = cmd.ExecuteScalar();

                conn.Close();

                try
                {
                    int result = Convert.ToInt16(obj);

                    if (result == 0)
                        return 0; //Does not exist
                    else
                        return 1; //Exists

                }
                catch (Exception e)
                {
                    return 0; //Does not exist
                }
            }
            catch (MySqlException e)
            {
                return e.ErrorCode;
            }
            catch (Exception e)
            {
                return -1; //Unhandled exception
            }
        }
        //inserts new tuple into posts table when a new post is made
        public static int CreatePost(string id, string post)
        {
            //POST THAT DATA!
            string query = "INSERT INTO posts (posterNum, postData) VALUES (@id, @post);";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@post", post);

                conn.Open();

                Object obj = cmd.ExecuteScalar();

                conn.Close();

                try
                {
                    int result = Convert.ToInt16(obj);

                    if (result == 0)
                        return 0; //Does not exist
                    else
                        return 1; //Exists

                }
                catch (Exception e)
                {
                    return 0; //Does not exist
                }
            }
            catch (MySqlException e)
            {
                return e.ErrorCode;
            }
            catch (Exception e)
            {
                return -1; //Unhandled exception
            }
        }
        //gets a list of suggested friends
        public static List<string> GetSuggestions(string id)
        {
            //Load 5 friend suggestions
            string query =
                "SELECT " +
                    "IF(f.sender = friend.num, f.receiver, f.sender) As 'suggested', " +
                    "(SELECT u.userName FROM users u WHERE u.userNum = suggested) As 'futureFriend?' " +
                  "FROM " +
                    "users u, " +
                    "friendships f, " +
                    "(SELECT IF(fs.sender = '" + id + "', fs.receiver, fs.sender) As 'num' " +
                    "FROM friendships fs " +
                    "WHERE (fs.sender = '" + id + "' Or fs.receiver = '" + id + "') " +
                    "GROUP BY num) friend " +
                "WHERE " +
                  "(f.sender = friend.num And f.receiver != '" + id + "') Or " +
                  "(f.receiver = friend.num And f.sender != '" + id + "') " +
                "GROUP BY suggested LIMIT 0, 5;";

            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                List<string> data = new List<String>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    //SuggestedNum,SuggestedName
                    data.Add(dataTable.Rows[i][0].ToString() + ',' + dataTable.Rows[i][1].ToString());
                }

                return data;
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string CheckResult(string query)
        {
            //Queries database expecting one string result
            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                Object result = cmd.ExecuteScalar();

                conn.Close();

                if (result == null)
                    return "NO REL";
                else
                    return result.ToString();
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null; //Unhandled exception
            }  
        }
        //Queries database expecting a numerical result
        public static int CheckQuery(string query)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                Object obj = cmd.ExecuteScalar();

                conn.Close();

                try
                {
                    int result = Convert.ToInt16(obj);

                    if (result == 0)
                        return 0; //Does not exist
                    else
                        return 1; //Exists

                }
                catch(Exception e)
                {
                    return 0; //Does not exist
                }
            }
            catch(MySqlException e)
            {
                return e.ErrorCode;
            }
            catch(Exception e)
            {
                return -1; //Unhandled exception
            }            
        }
        //gets the total number of likes on a post, and also if the user has liked this post (to determine whether to display like or unlike)
        public static string GetLikes(string postId, string userNum)
        {
            try
            {

                string query = "SELECT IF(EXISTS (SELECT likeNum FROM likes WHERE likerNum = '" + userNum + "' AND parentPostNum = '" + postId + "'), 'LIKED', 'NOT LIKED') AS 'status', COUNT(likeNum) AS 'likes' " +
                    "FROM posts, likes WHERE parentPostNum = postNum AND postNum = '" + postId + "';";

                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                conn.Open();

                DataTable dataTable = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                //Query database and get result table
                da.Fill(dataTable);

                conn.Close();
                da.Dispose();

                if (dataTable.Rows.Count == 0)
                    return null;

                string data = dataTable.Rows[0][0].ToString() + ',' + dataTable.Rows[0][1].ToString();

                return data;
            }
            catch (MySqlException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null; //Unhandled exception
            }  
        }

    }
}