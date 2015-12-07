using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using engNET;

public partial class Friends : System.Web.UI.Page
{
    //ensure that you're logged in and displays your data
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null || ((User)Session["user"]) == null)
        {
            Response.Redirect("Default.aspx");
        }

        UserButton.InnerText = ((User)Session["user"]).userName;
        UserButton.HRef = "Page.aspx?id=" + ((User)Session["user"]).userNum;
        UserButton.Visible = true;
        UserButtonLi.Attributes["class"] = "active";

        LoadFriends();
    }
    //logout handling, setting user session to null
    protected void LogoutButton_Click(object sender, EventArgs e)
    {
        Session["user"] = null;
        Response.Redirect("Default.aspx");
    }

    //gets your list of freinds, display them in alphabetical order
    private void LoadFriends()
    {
        List<string> fList = Database.GetFriends(((User)Session["user"]).userNum.ToString());

        if (fList == null || fList.Count == 0)
        {
            FriendControls.Visible = false;
            return;
        }
        else
        {
            NoFriends.Visible = false;
        }

        char alpha = 'A';

        foreach (string s in fList)
        {
            HtmlGenericControl li = FriendLi(s.Split(',')[0], s.Split(',')[1], s.Split(',')[2], s.Split(',')[3]);
            if (li != null)
            {
                while (alpha != '!' && (Char)Convert.ToInt16(alpha) < (Char)Convert.ToInt16(s.Split(',')[1][0]))
                {
                    alpha = (Char)(Convert.ToUInt16(alpha) + 1);
                }

                if (alpha != '!' && alpha == s.Split(',')[1][0])
                {
                    HtmlGenericControl alphaLi = new HtmlGenericControl("li");
                    alphaLi.Attributes["style"] = "margin: 0 auto; width:450px;'";
                    alphaLi.InnerHtml = "<p class='lead text-center text-capitalize' style='margin: 0;'>" + alpha + "</p";
                    FriendList.Controls.Add(new HtmlGenericControl("hr"));
                    FriendList.Controls.Add(alphaLi);
                    if (alpha != 'Z')
                        alpha = (Char)(Convert.ToUInt16(alpha) + 1);
                    else
                        alpha = '!';  
                }

                FriendList.Controls.Add(li);
            }
        }
        
    }

    //display friend
    private HtmlGenericControl FriendLi(string ID, string name, string relid, string status)
    {
        HtmlGenericControl Li = new HtmlGenericControl("li");
        Li.Attributes["class"] = "Friend well well-sm";

        HtmlGenericControl a = new HtmlGenericControl("a");
        a.Attributes["href"] = "Page.aspx?id=" + ID;
        a.InnerText = name;

        Button btn = new Button();
        btn.CssClass = "btn btn-sm btn-primary";
        btn.Attributes["style"] = "float: right; position: relative; top: -6px;";
        btn.Click += new EventHandler(BlockUnblock_Click);
        btn.CommandArgument = relid + ',' + status;
        if(status == "FRIENDS")
        {
            btn.Text = "Block";
        }
        else if(status == "REQUESTED")
        {
            return null;
        }
        else
        {
            btn.Text = "Unblock";
        }

        Li.Controls.Add(a);
        Li.Controls.Add(btn);

        return Li;
    }

    //provide ability to block or unblock a friends
    private void BlockUnblock_Click(object sender, EventArgs e)
    {
        string args = ((Button)sender).CommandArgument.ToString();
        string id = args.Split(',')[0];
        string rel = args.Split(',')[1];
        if (id == null || id.Trim() == String.Empty || rel == null || rel.Trim() == String.Empty)
            return;

        //Execute query - Block or Unblock
        if(rel == "FRIENDS")
        {
            //Block
            int result = Database.CheckQuery("UPDATE relationships r SET r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = (SELECT IF(r.Sender = '" + ((User)Session["user"]).userNum + "','SENDER BLOCKED','RECEIVER BLOCKED'))) WHERE r.relNum ='" + id + "';");
        }
        else
        {
            //Unblock
            int result = Database.CheckQuery("UPDATE relationships r SET r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = 'FRIENDS') WHERE r.relNum ='" + id + "';");
        }
        

        //Reload
        Response.Redirect(Request.Url.ToString());

    }

    //provide ability to block all friends
    protected void BlockAllButton_Click(object sender, EventArgs e)
    {
        string id = ((User)Session["user"]).userNum.ToString();
        int result = Database.CheckQuery("UPDATE relationships r SET r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = (SELECT IF(r.Sender = '" + id + "','SENDER BLOCKED','RECEIVER BLOCKED'))) WHERE (r.sender ='" + id + "' Or r.receiver = '" + id + "') And NOT(r.sender = '" + id + "' And r.relDataNum = (SELECT relDataNum FROM relData WHERE relData = 'REQUESTED'));");
        
        //Reload
        Response.Redirect(Request.Url.ToString());
    }
    //p[rovide ability to unblock all friends
    protected void UnBlockAllButton_Click(object sender, EventArgs e)
    {
        string id = ((User)Session["user"]).userNum.ToString();
        int result = Database.CheckQuery("UPDATE relationships r SET r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = 'FRIENDS') WHERE " +
            "(r.sender ='" + id + "' And r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = 'SENDER BLOCKED'))" +
            "OR (r.receiver = '" + id + "' And r.relDataNum = (SELECT d.relDataNum FROM relData d WHERE d.relData = 'RECEIVER BLOCKED'));");

        //Reload
        Response.Redirect(Request.Url.ToString());
    }
}