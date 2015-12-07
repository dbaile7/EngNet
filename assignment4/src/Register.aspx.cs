using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using engNET;

public partial class Register : System.Web.UI.Page
{
    //loads your page, makes sure you are not logged in yet
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] != null && (User)Session["user"] != null)
        {
            Response.Redirect("Default.aspx");
        }
    }

    //handles when you click the register button
    protected void RegisterButton_Click(object sender, EventArgs e)
    {
        //Reset messages first
        NameMessage.InnerHtml = String.Empty;
        EmailMessage.InnerHtml = String.Empty;
        PassMessage.InnerHtml = String.Empty;
        ErrorMessage.InnerHtml = String.Empty;

        //Validate the form
        //Check that boxes are not empty after trimming them
        NameBox.Text = NameBox.Text.Trim();
        EmailBox.Text = EmailBox.Text.Trim();

        //And verify the local info is formatted correctly
        if(NameBox.Text == String.Empty)
        {
            NameMessage.InnerHtml = "<br />Your name cannot be blank!";
            return;
        }

        if (EmailBox.Text == String.Empty)
        {
            EmailMessage.InnerHtml = "<br />Your email cannot be blank!";
            return;
        }

        if(PassBox.Text == String.Empty)
        {
            PassMessage.InnerHtml = "<br />You must have a password!";
            return;
        }

        if(PassBox.Text.Length < 8)
        {
            PassMessage.InnerHtml = "<br />Your password must at least 8 characters!";
            return;
        }

        if(PassConfirmBox.Text != PassBox.Text)
        {
            PassMessage.InnerHtml = "<br />Passwords do not match!";
            return;
        }

        //Second, verify the email is not already in use
        int e_result = Database.CheckQuery("SELECT COUNT(`userNum`) FROM `users` WHERE `userEmail`='" + EmailBox.Text + "';");

        if(e_result == 1) //True
        {
            //Email already in database
            EmailMessage.InnerHtml = "<br />This email address is already in use!";
            return;
        }
        else if(e_result < 0)
        {
            EmailMessage.InnerHtml = "<br />Error checking email. Please try again later.";
            return;
        }

        //Email is okay!
        //Create the account
        int result = Database.CheckQuery("INSERT `users` (`userName`,`userEmail`,`userPass`)VALUES("+
                                         "'" + NameBox.Text + "'," +
                                         "'" + EmailBox.Text + "'," +
                                         "'" + PassBox.Text + "'" +
                                         ");");
        //result == 0 means created
        if(result != 0)
        {
            ErrorMessage.InnerHtml = "<br />An error occured creating your account. Please try again later.";
            return;
        }

        //Login the new user
        User user = Database.Login(EmailBox.Text, PassBox.Text);

        if(user == null)
        {
            Response.Redirect("Default.aspx");
        }

        //Save to session
        Session["user"] = user;

        //Redirect the user to their own page
        Response.Redirect("Page.aspx?id=" + user.userNum);
    }
}