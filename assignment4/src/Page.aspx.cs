using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using engNET;

public partial class Page : System.Web.UI.Page
{
    private string pageName;
    private string pageNum;

    //loads a page, either your friend or yours
    protected void Page_Load(object sender, EventArgs e)
    {
        string userid = Request.QueryString["id"];
        if(userid == null || userid.Trim() == String.Empty || Session["user"] == null || ((User)Session["user"]) == null)
        {
            Response.Redirect("Default.aspx");
        }

        pageNum = userid;

        UserButton.InnerText = ((User)Session["user"]).userName;
        UserButton.HRef = "Page.aspx?id=" + ((User)Session["user"]).userNum;
        UserButton.Visible = true;

        FriendshipButton.Visible = false;

        long myid = ((User)Session["user"]).userNum;

        //Your page
        if (Convert.ToInt64(userid) == myid)
        {
            UserButtonLi.Attributes["class"] = "active";
            TitleText.InnerHtml = "Welcome Home " + ((User)Session["user"]).userName.Split(' ')[0] + "!<br /><span class='lead' style='font-size: 20px;'>(This is your page)</span>";
            Title = "engNET - My Page";
            FriendControl.Visible = false;
            FriendCount.Attributes["class"] = "NoSelect MyPost MyPostHover well well-sm glyphicon glyphicon-user";
            FriendCount.Attributes["title"] = "View My Friends";
            FriendCount.Attributes["style"] = "cursor: pointer";
            FriendCount.Attributes["onclick"] = "window.location='Friends.aspx'";
            LoadSuggestions(myid.ToString());
        }
        else //Someone else's page
        {
            pageName = Database.GetUserName(userid);
            if (pageName == null || pageName == String.Empty)
                Response.Redirect("Default.aspx");
            NewPost.Visible = false;
            Suggestions.Visible = false;
            TitleText.InnerHtml = "Welcome to " + pageName.Split(' ')[0] + "'s Page!<br /><span class='lead' style='font-size: 20px;'>(" + pageName + ")</span>";
            Title = "engNET - " + pageName.Split(' ')[0] + "'s Page";
            LoadFriend(userid);
        }
        
        LoadPage(userid);
        LoadPosts(userid);
    }

    //Loads friend suggestions if you're on your own page
    private void LoadSuggestions(string id)
    {
        List<string> suggestions = Database.GetSuggestions(id);

        if(suggestions == null)
        {
            Suggestions.Visible = false;
            return;
        }

        string num;
        string name;

        foreach(string s in suggestions)
        {
            num = s.Split(',')[0];
            name = s.Split(',')[1];
            Suggestions.Controls.Add(SuggestLi(num, name));
        }

    }
    //gets a list of suggested friends, uses query 
    private HtmlGenericControl SuggestLi(string num, string name)
    {

        HtmlGenericControl li = new HtmlGenericControl("li");
        li.Attributes["class"] = "well well-sm";
        li.Attributes["style"] = "margin: 5px; display: inline-block;";

        HtmlGenericControl a = new HtmlGenericControl("a");
        a.Attributes["class"] = "lead";
        a.Attributes["href"] = "Page.aspx?id=" + num;
        a.InnerText = name;
        a.Attributes["title"] = "View " + name.Split(' ')[0] + "'s Profile";

        li.Controls.Add(a);

        return li;
    }

    //Used to configure the Add Friend button
    private void LoadFriend(string id)
    {
        //Check if this user and you have a relationship
        string you = ((User)Session["user"]).userNum.ToString();

        string query = "SELECT d.relData FROM relationships r, relData d WHERE " +
            "((sender='" + id + "' And receiver='" + you + "') Or " +
            "(sender='" + you + "' And receiver='" + id + "')) And " +
            "d.relDataNum = r.relDataNum;";

        string result = Database.CheckResult(query);

        if(result == null)
        {
            FriendButton.Visible = false;
            FriendText.InnerText = "An error has occured";
            return;
        }

        if (result != "NO REL")
        {
            FriendButton.Visible = false;
            if (result == "FRIENDS")
            {
                FriendText.InnerText = "You are friends";
                FriendshipButton.Visible = true;
            }
            else if (result == "REQUESTED")
                FriendText.InnerText = "A request has been made";
            else
                FriendText.InnerText = "One of you has blocked the other";
        }
        else
            FriendButton.CommandArgument = id;
    }
    //gets page info
    private void LoadPage(string id)
    {
        string data = Database.LoadPage(id);

        if (data == null)
            Response.Redirect("Default.aspx");

        string posts = data.Split(',')[0];
        string likes = data.Split(',')[1];
        string friends = data.Split(',')[2];

        PostCount.InnerHtml = "&nbsp;" + posts;
        LikeCount.InnerHtml = "&nbsp;" + likes;
        FriendCount.InnerHtml = "&nbsp;" + friends;

        if (Convert.ToInt64(id) != ((User)Session["user"]).userNum)
            return;

        //Load Friend Requests
        List<string> requests = Database.GetFriendRequests(id);

        if(requests == null || requests.Count == 0)
        {
            RequestList.Visible = false;
            return;
        }

        RequestList.Controls.Clear();

        //foreach Request
        foreach (string s in requests)
        {
            RequestList.Controls.Add(RequestLi(s.Split(',')[0], s.Split(',')[1], s.Split(',')[2]));
        }
        
    }
    //displays friend requests
    private HtmlGenericControl RequestLi(string relid, string userid, string name)
    {
        HtmlGenericControl li = new HtmlGenericControl("li");
        li.Attributes["class"] = "FriendRequest well well-sm";

        HtmlGenericControl p = new HtmlGenericControl("p");
        p.Attributes["style"] = "font-size: 16px; font-weight: bold;";
        p.InnerHtml = "<a href='Page.aspx?id=" + userid + "' title=\"View " + name.Split(' ')[0] + "'s Profile\">" + name + "</a> wants to be friends!";

        Button accept = new Button();
        accept.CssClass = "btn btn-sm btn-info";
        accept.CommandArgument = relid;
        accept.Text = "Accept";
        accept.Click += new EventHandler(AcceptRequest_Click);

        Button block = new Button();
        block.CssClass = "btn btn-sm btn-primary";
        block.CommandArgument = relid;
        block.Text = "Block";
        block.Click += new EventHandler(BlockRequest_Click);

        li.Controls.Add(p);
        li.Controls.Add(accept);
        li.Controls.Add(block);
        return li;
    }
    //handles the block request option
    protected void BlockRequest_Click(object sender, EventArgs e)
    {
        string id = ((Button)sender).CommandArgument.ToString();
        if (id == null || id.Trim() == String.Empty)
            return;

        //Execute query - Block Request
        int result = Database.CheckQuery("UPDATE relationships r SET r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = 'RECEIVER BLOCKED') WHERE r.relNum ='" + id + "';");

        //Reload
        LoadPage(((User)Session["user"]).userNum.ToString());
        
    }
    //handles the add request option
    protected void AcceptRequest_Click(object sender, EventArgs e)
    {
        string id = ((Button)sender).CommandArgument.ToString();
        if (id == null || id.Trim() == String.Empty)
            return;

        //Execute query - Accept Request
        int result = Database.CheckQuery("UPDATE relationships r SET r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = 'FRIENDS') WHERE r.relNum ='" + id + "';");

        //Reload
        LoadPage(((User)Session["user"]).userNum.ToString());

    }
    //load posts
    private void LoadPosts(string id)
    {
        //If you is true, you are viewing your own profile
        List<Post> posts = Database.LoadPosts(id);
        NoPostDiv.Visible = false;
        if(posts.Count == 0)
        {
            NoPostDiv.Visible = true;
            return;
        }

        foreach (Post p in posts)
        {            
            HtmlGenericControl div = NewPostDiv(p);
            

            if(p.comments.Count > 0)
            {
                foreach (Comment c in p.comments)
                {
                    div.Controls.Add(new HtmlGenericControl("hr"));
                    div.Controls.Add(CommentDiv(c));
                }
            }

            div.Controls.Add(NewCommentDiv(p.id.ToString()));
            Posts.Controls.Add(div);
        }
    }
    //creates a space to display comments
    private HtmlGenericControl NewCommentDiv(string id)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Controls.Add(new HtmlGenericControl("hr"));

        HtmlGenericControl innerdiv = new HtmlGenericControl("div");
        innerdiv.Attributes["style"] = "width: 100%; max-width: 600px; margin: 0 auto; text-align: center";

        Button button = new Button();
        button.ID = "Button_" + id;
        button.CssClass = "btn btn-sm btn-info";
        button.Text = "Post Comment";
        button.Attributes["style"] = "margin-top: 10px;";
        button.Click += button_Click;

        TextBox textbox = new TextBox();
        textbox.ID = "TextBox_" + id;
        textbox.CssClass = "form-control";
        textbox.MaxLength = 140;
        textbox.Attributes["placeholder"] = "Comment on this post";
        textbox.Attributes["onkeypress"] = "if(event.keyCode == 13) { document.getElementById('" + button.ClientID + "').click(); return false; } ";

        innerdiv.Controls.Add(textbox);
        innerdiv.Controls.Add(button);

        div.Controls.Add(innerdiv);

        return div;
    }
    //handles the post comment button
    void button_Click(object sender, EventArgs e)
    {
        string id = ((Button)sender).ID.Split('_')[1];
        TextBox box = (TextBox)FindControl("TextBox_" + id);

        box.Text = box.Text.Trim();

        if (box.Text == String.Empty)
        {
            Response.Write("alert('Comment cannot be empty');");
            return;
        }

        int result = Database.CreateComment(id, ((User)Session["user"]).userNum.ToString(), box.Text);

        if (result >= 0)
            Response.Redirect(Request.Url.ToString());
    }
    //creates a space to display a comment
    private HtmlGenericControl CommentDiv(Comment c)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Attributes["class"] = "";

        div.InnerHtml = "<p class='lead' style='font-size: 14px;'>" + c.data + "</p><p style='text-align: right;'>" + c.date + "</p><a title='View Profile' href='Page.aspx?id=" + c.id + "'>" + c.name + "</a>";

        return div;
    }

    //Generates a place for a post
    private HtmlGenericControl NewPostDiv(Post post)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Attributes["class"] = "well well-sm";

        HtmlGenericControl p = new HtmlGenericControl("p");
        p.Attributes["class"] = "blockquote";

        HtmlGenericControl span = new HtmlGenericControl("span");
        span.Attributes["style"] = "font-size: 22px;";
        span.InnerText = post.data;

        HtmlGenericControl small = new HtmlGenericControl("p");
    
        HtmlGenericControl user = new HtmlGenericControl("a");
        user.InnerText = post.posterName;
        user.Attributes["href"] = "Page.aspx?id=" + post.posterNum;

        HtmlGenericControl date = new HtmlGenericControl("p");
        date.InnerText = post.date;
        date.Attributes["style"] = "text-align: right;";

        small.Controls.Add(date);
        small.Controls.Add(user);
        p.Controls.Add(span);

        HtmlGenericControl options = new HtmlGenericControl("div");
        options.Attributes["style"] = "text-align: right; margin: 0; padding: 0;";

        //You are the poster, you may delete the post
        if (post.posterNum == ((User)Session["user"]).userNum)
        {
            Button delete = new Button();
            delete.CssClass = "btn btn-default";
            delete.CommandArgument = post.id.ToString();
            delete.Text = "x";
            delete.ToolTip = "Delete Post";
            delete.Click += new EventHandler(DeletePost_Click);

            options.Controls.Add(delete);
        }

        div.Controls.Add(options);
        div.Controls.Add(p);
        div.Controls.Add(small);
        div.Controls.Add(NewLikeDiv(post.id.ToString()));
        return div;
    }
    //creates a space for displaying likes
    private HtmlGenericControl NewLikeDiv(string id)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");

        string data = Database.GetLikes(id, ((User)Session["user"]).userNum.ToString());

        if(data == null)
        {
            div.InnerHtml = "<p>Error loading like data</p>";
            return div;
        }

        string status = data.Split(',')[0]; //LIKED or NOT LIKED
        string likes = data.Split(',')[1]; //Number of likes

        HtmlGenericControl p = new HtmlGenericControl("p");
        p.Attributes["style"] = "display: inline-block;";
        p.InnerHtml = "Likes:&nbsp;<span style='font-weight: bold'>" + likes + "</span>&nbsp;&nbsp;";

        Button button = new Button();
        button.CssClass = "btn btn-sm btn-primary";
        button.CommandArgument = id;
        if (status == "LIKED")
        {
            //Button used to unlike the post
            button.Text = "Unlike";
            button.Click += UnlikePost_Click;
        }
        else
        {
            //Button used to like the post
            button.Text = "Like";
            button.Click += LikePost_Click;
        }

        p.Controls.Add(button);

        div.Controls.Add(p);

        return div;
    }
    //handles when you click the unlike button
    protected void UnlikePost_Click(object sender, EventArgs e)
    {
        string query = "DELETE FROM likes WHERE likerNum = '" + ((User)Session["user"]).userNum.ToString() + "' And parentPostNum = '" + ((Button)sender).CommandArgument + "';";

        int result = Database.CheckQuery(query);

        Response.Redirect(Request.Url.ToString());
    }
    //handles when you click the like button
    protected void LikePost_Click(object sender, EventArgs e)
    {
        string query = "INSERT INTO likes SET likerNum = '" + ((User)Session["user"]).userNum.ToString() + "', parentPostNum = '" + ((Button)sender).CommandArgument + "';";

        int result = Database.CheckQuery(query);

        Response.Redirect(Request.Url.ToString());
    }
    //handles when you click the /x/ to delete your on post
    protected void DeletePost_Click(object sender, EventArgs e)
    {
        //Delete this post using command arg
        string id = ((Button)sender).CommandArgument.ToString();
        if (id == null || id.Trim() == String.Empty)
            return;

        //Execute query
        int result = Database.CheckQuery("DELETE FROM posts WHERE postNum = '" + id + "';");

        if(result == 0) //Success
        {

        }
        else
        {
            //Failure

        }
        
        //Reload
        Response.Redirect(Request.Url.ToString());
    }
    //handles logout button
    protected void LogoutButton_Click(object sender, EventArgs e)
    {
        Session["user"] = null;
        Response.Redirect("Default.aspx");
    }
    //when you create a post and press the button
    protected void CreatePost_Click(object sender, EventArgs e)
    {
        //Validate Post Data
        PostMessage.InnerText = String.Empty;
        PostData.Text = PostData.Text.Trim();
        if(PostData.Text == String.Empty)
        {
            PostMessage.InnerText = "Your post cannot be empty!";
            return;
        }

        int result = Database.CreatePost(((User)Session["user"]).userNum.ToString(), PostData.Text);
        
        if(result != 0)
        {
            //Error posting
            PostMessage.InnerText = "An error occurred. Please try again later!";     
        }
        else
        {
            Response.Redirect(Request.Url.ToString());
        }
    }

   //when you add a friend using the 'request' button, inserts new tuple into friendship table
    protected void FriendButton_Click(object sender, EventArgs e)
    {
        string id = ((Button)sender).CommandArgument;

        string query = "INSERT INTO relationships SET sender='"+ ((User)Session["user"]).userNum.ToString() + "', receiver='" + id + "', relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = 'REQUESTED');";

        Database.CheckQuery(query);

        Response.Redirect(Request.Url.ToString());
    }

    //redirects you when you want to view your friendship with someone
    protected void FriendshipButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Friendship.aspx?sender=" + ((User)Session["user"]).userNum + "&receiver=" + pageNum);
    }
}