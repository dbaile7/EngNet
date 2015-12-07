using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using engNET;

public partial class Friendship : System.Web.UI.Page
{
    string _sender;
    string _receiver;

    //ensures that you are logged in, check who you are, displays your friend's page
    protected void Page_Load(object sender, EventArgs e)
    {
        _sender = Request.QueryString["sender"];
        _receiver = Request.QueryString["receiver"];
        if (_sender == null || _sender.Trim() == String.Empty || _receiver == null || _receiver.Trim() == String.Empty || Session["user"] == null || ((User)Session["user"]) == null)
        {
            Response.Redirect("Default.aspx");
        }

        UserButton.InnerText = ((User)Session["user"]).userName;
        UserButton.HRef = "Page.aspx?id=" + ((User)Session["user"]).userNum;
        UserButton.Visible = true;

        LoadFriendship();
    }

    //returns a list of friendships that you have from the database
    private void LoadFriendship()
    {
        //If you is true, you are viewing your own profile
        List<Post> posts = Database.LoadFriendship(_sender, _receiver);
        if (posts.Count == 0)
        {
            return;
        }
        NoPostsDiv.Visible = false;

        foreach (Post p in posts)
        {
            HtmlGenericControl div = NewPostDiv(p);


            if (p.comments.Count > 0)
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

    //displays a comment on a post
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
    //handles the 'post comment' button
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
    //display a comment
    private HtmlGenericControl CommentDiv(Comment c)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Attributes["class"] = "";

        div.InnerHtml = "<p class='lead' style='font-size: 14px;'>" + c.data + "</p><p style='text-align: right;'>" + c.date + "</p><a title='View Profile' href='Page.aspx?id=" + c.id + "'>" + c.name + "</a>";

        return div;
    }

    //Generates a div for the post
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

        div.Controls.Add(options);
        div.Controls.Add(p);
        div.Controls.Add(small);

        div.Controls.Add(NewLikeDiv(post.id.ToString()));

        return div;
    }
    //creates a space tpo display likes
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
    //handles what happens when you unlike a post, deletes the tuple from the db
    protected void UnlikePost_Click(object sender, EventArgs e)
    {
        string query = "DELETE FROM likes WHERE likerNum = '" + ((User)Session["user"]).userNum.ToString() + "' And parentPostNum = '" + ((Button)sender).CommandArgument + "';";

        int result = Database.CheckQuery(query);

        Response.Redirect(Request.Url.ToString());
    }
    //handles what happens when you like a post, insertd the tuple from the db
    protected void LikePost_Click(object sender, EventArgs e)
    {
        string query = "INSERT INTO likes SET likerNum = '" + ((User)Session["user"]).userNum.ToString() + "', parentPostNum = '" + ((Button)sender).CommandArgument + "';";

        int result = Database.CheckQuery(query);

        Response.Redirect(Request.Url.ToString());
    }
    //logs out user
    protected void LogoutButton_Click(object sender, EventArgs e)
    {
        Session["user"] = null;
        Response.Redirect("Default.aspx");
    }
}