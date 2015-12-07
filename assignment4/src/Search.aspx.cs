using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using engNET;

public partial class Search : System.Web.UI.Page
{
    string searchname;

    //verifies that there is search data
    protected void Page_Load(object sender, EventArgs e)
    {
        searchname = Request.QueryString["name"];

        //verify that user is logged in
        if (searchname == null || searchname.Trim() == String.Empty || Session["user"] == null || ((User)Session["user"]) == null)
        {
            Response.Redirect("Default.aspx");
        }

        
        UserButton.InnerText = ((User)Session["user"]).userName;
        UserButton.HRef = "Page.aspx?id=" + ((User)Session["user"]).userNum;
        UserButton.Visible = true;

        ResultTitle.InnerText = "Search Results for \"" + searchname + "\"";

        List<string> results = Database.SearchResults(searchname);

        foreach(string s in results)
        {
            Results.Controls.Add(GetResult(s));
            
        }

        if (results.Count > 0)
            NoResults.Visible = false;
    }

    //displays the search results
    private HtmlGenericControl GetResult(string result)
    {
        HtmlGenericControl a = new HtmlGenericControl("a");
        a.Attributes["class"] = "lead well well-sm";
        a.Attributes["style"] = "width: 100%; padding: 10px; display: inline-block;";
        a.Attributes["href"] = "Page.aspx?id=" + result.Split(',')[0];
        a.Attributes["title"] = "View " + result.Split(',')[1].Split(' ')[0] + "'s profile";
        a.InnerText = result.Split(',')[1];

        return a;
    }
    //handles logout
    protected void LogoutButton_Click(object sender, EventArgs e)
    {
        Session["user"] = null;
        Response.Redirect("Default.aspx");
    }
}