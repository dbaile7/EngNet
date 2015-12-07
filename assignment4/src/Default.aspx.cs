using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using engNET;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //makes sure you are logged in
        Login.Visible = false;
        if (Session["user"] != null && (User)(Session["user"]) != null)
        {
            //User is logged in
            RegisterButton.Visible = false;
            UserButton.InnerText = ((User)Session["user"]).userName;
            UserButton.HRef = "Page.aspx?id=" + ((User)Session["user"]).userNum;
            UserButton.Visible = true;
            BigHeader.Visible = false;
            //Load 20 recent posts from you and all friends
            LoadPosts();
        }
        else
        {
            //User not logged in
            Login.Visible = true;
            LogoutButton.Visible = false;
            UserButton.Visible = false;
            Feed.Visible = false;
            SearchNav.Visible = false;

        }        
    }

    //loads all posts into a list of posts
    private void LoadPosts()
    {
        List<Post> posts = Database.LoadFeed(((User)Session["user"]).userNum.ToString());

        if (posts.Count > 0)
        {
            NoPosts.Visible = false;
            foreach (Post p in posts)
            {
                Posts.Controls.Add(NewPostDiv(p));
            }
        }
    }

    //Generates a div for the post
    private HtmlGenericControl NewPostDiv(Post post)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Attributes["class"] = "well well-sm";

        if (post.posterNum == ((User)Session["user"]).userNum)
            div.Attributes["class"] = "well well-sm MyPost";

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

        div.Controls.Add(options);
        div.Controls.Add(p);
        div.Controls.Add(small);

        div.Controls.Add(NewLikeDiv(post.id.ToString()));

        //Add comments

        foreach (Comment c in post.comments)
        {
            div.Controls.Add(new HtmlGenericControl("hr"));
            div.Controls.Add(CommentDiv(c));
        }

        //Allow creating a comment
        div.Controls.Add(NewCommentDiv(post.id.ToString()));

        return div;
    }

    //determines how many likes are on a post, and wether or not you have liked it
    private HtmlGenericControl NewLikeDiv(string id)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");

        string data = Database.GetLikes(id, ((User)Session["user"]).userNum.ToString());

        if (data == null)
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

    //when unliking a post, deletes the tuple from the likes table
    protected void UnlikePost_Click(object sender, EventArgs e)
    {
        string query = "DELETE FROM likes WHERE likerNum = '" + ((User)Session["user"]).userNum.ToString() + "' And parentPostNum = '" + ((Button)sender).CommandArgument + "';";

        int result = Database.CheckQuery(query);

        Response.Redirect(Request.Url.ToString());
    }

    //when liking a post, inserts a new tuple
    protected void LikePost_Click(object sender, EventArgs e)
    {
        string query = "INSERT INTO likes SET likerNum = '" + ((User)Session["user"]).userNum.ToString() + "', parentPostNum = '" + ((Button)sender).CommandArgument + "';";

        int result = Database.CheckQuery(query);

        Response.Redirect(Request.Url.ToString());
    }

    //adds comments
    private HtmlGenericControl NewCommentDiv(string id)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Controls.Add(new HtmlGenericControl("hr"));

        HtmlGenericControl innerdiv = new HtmlGenericControl("div");
        innerdiv.Attributes["style"] = "width: 100%; max-width: 600px; margin: 0 auto; text-align: center";

        TextBox textbox = new TextBox();
        textbox.ID = "TextBox_" + id;
        textbox.CssClass = "form-control";
        textbox.MaxLength = 140;
        textbox.Attributes["placeholder"] = "Comment on this post";

        Button button = new Button();
        button.ID = "Button_" + id;
        button.CssClass = "btn btn-sm btn-info";
        button.Text = "Post Comment";
        button.Attributes["style"] = "margin-top: 10px;";
        button.Click += button_Click;

        innerdiv.Controls.Add(textbox);
        innerdiv.Controls.Add(button);

        div.Controls.Add(innerdiv);

        return div;
    }

    //the function called when cliking the 'post' button
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

    //creating a space to insert comments
    private HtmlGenericControl CommentDiv(Comment c)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Attributes["class"] = "";

        div.InnerHtml = "<p class='lead' style='font-size: 14px;'>" + c.data + "</p><p style='text-align: right;'>" + c.date + "</p><a title='View Profile' href='Page.aspx?id=" + c.id + "'>" + c.name + "</a>";

        return div;
    }
    //handling the login button, setting the session of the user
    protected void LoginButton_Click(object sender, EventArgs e)
    {
        //Execute the login query
        LoginMessage.InnerText = String.Empty;
        EmailBox.Text = EmailBox.Text.Trim();
        if(EmailBox.Text == String.Empty)
        {
            LoginMessage.InnerText = "Email cannot be empty!";
            return;
        }
        if(PassBox.Text == String.Empty)
        {
            LoginMessage.InnerText = "Password cannot be empty!";
            return;
        }

        User user = Database.Login(EmailBox.Text, PassBox.Text);

        if(user == null)
        {
            LoginMessage.InnerText = "Email or Password is incorrect";
        }
        else
        {
            Session["user"] = user;
            Response.Redirect("Default.aspx");
        }
    }

    //when logging out, setting the user session to 'null'
    protected void LogoutButton_Click(object sender, EventArgs e)
    {
        Session["user"] = null;
        Response.Redirect("Default.aspx");
    }
}