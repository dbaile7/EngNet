<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Page.aspx.cs" Inherits="Page" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>engNET</title>
    <link rel="stylesheet" href="styles/bootstrap.min.css" />
    <link rel="stylesheet" href="styles/style.css" />
    <link rel="stylesheet" href="styles/glyphicons.css" />
</head>
<body>
    <div class="wrap">
        <form id="MainForm" runat="server" class="main">
            <asp:ScriptManager ID="sm" runat="server"></asp:ScriptManager>
            <header>
                <nav class="navbar navbar-default MyNav">
                    <div class="container-fluid">
                        <div class="navbar-header">
                            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#header-nav">
                                <span class="sr-only">Toggle navigation</span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                            </button>
                            <a href="Default.aspx" title="engNET" class="navbar-brand">ENG<strong>NET</strong></a>
                        </div>
                        <div class="collapse navbar-collapse" id="header-nav">
                            <ul class="nav navbar-nav">
                                <li>
                                    <a href="Default.aspx" title="Home">
                                        Home <span class="sr-only">(current)</span>
                                    </a>
                                </li>
                            </ul>
                            <ul id="SearchNav" runat="server" class="nav navbar-nav">
                                <li>
                                    <div style="width: 350px">
                                        <input type="text" id="SearchText" class="form-control" placeholder="Search People" maxlength="150" style="margin: 15px; margin-bottom: 0; margin-right: 0; width: 220px; float: left;" onkeydown="if(event.keyCode == 13) {window.location='Search.aspx?name='+ document.getElementById('SearchText').value; return false;}" />
                                        <input type="button" id="SearchButton" class="btn btn-info" value="Search" style="margin: 15px; margin-bottom: 0; margin-left: 0; float: left;" onclick="window.location = 'Search.aspx?name=' + document.getElementById('SearchText').value" />
                                    </div>
                                </li>
                            </ul>
                            <ul class="nav navbar-nav pull-right">
                                <li id="UserButtonLi" runat="server">
                                    <a id="UserButton" runat="server" title="View Profile" href="#"></a>
                                </li>
                                <li>
                                    <a id="LogoutButton" runat="server" title="Logout" href="#" onServerClick="LogoutButton_Click">Logout</a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>
            </header>
            <br /><br /><br /><br /><br /><br />
            <!-- This is the page data-->
            <h1 id="TitleText" runat="server" class="well text-center" style="position:relative; top: -15px; border-radius: 0;">Welcome!</h1>
            <div class="text-center">
                <ul style="list-style: none; position: relative; left: -20px;">
                    <li id="PostCount" runat="server" title="Posts" class="NoSelect well well-sm glyphicon glyphicon-pencil" style="margin-right: 10px">
                        0
                    </li>
                    <li id="LikeCount" runat="server" title="Likes Received" class="NoSelect well well-sm glyphicon glyphicon-plus" style="margin-right: 10px">
                        0
                    </li>
                    <li id="FriendCount" runat="server" title="Friends" class="NoSelect well well-sm glyphicon glyphicon-user">
                        0
                    </li>
                </ul>
                <ul id="RequestList" runat="server" style="list-style: none; position: relative; left: -20px;">
                    <li>
                        <br />
                    </li>
                    <li id="FriendControl" runat="server" class="well well-sm" style="width: 300px; margin: 0 auto;">
                        <p class="lead text-center">Friendship</p>
                        <p id="FriendText" runat="server" class="text-danger text-center"></p>
                        <asp:Button ID="FriendButton" CssClass="btn btn-info" Text="Send Friend Request" runat="server" OnClick="FriendButton_Click" />
                        <asp:Button ID="FriendshipButton" CssClass="btn btn-primary" Text="View Friendship" runat="server" OnClick="FriendshipButton_Click"/>
                    </li>
                </ul>
                <ul id="Suggestions" runat="server" style="list-style: none; padding: 10px; min-width: 200px; width: 100%; max-width: 600px; margin: 0 auto; display: block; position: relative; top: -20px;">
                    <li>
                        <h3 class="text-center">Suggested Friends</h3>
                    </li>
                </ul>
                <br />
            </div>
            <div id="NewPost" runat="server">
                <div class="well well-sm" style="min-width: 300px; width: 100%; max-width: 500px; margin: 0 auto;">
                    <h3 class="lead text-center">Create a new post</h3>
                    <p id="PostMessage" class="text-danger text-center" runat="server"></p>
                    <asp:TextBox CssClass="form-control" ID="PostData" runat="server" placeholder="Enter your post here" style="width: 90%; margin: 0 auto; resize: none;" MaxLength="140" />
                    <br />
                    <asp:Button CssClass="btn btn-info" ID="CreatePost" ToolTip="Create Post" runat="server" Text="Create" OnClick="CreatePost_Click" style="float:right"/>
                    <br />
                    <br />
                </div>
                <br />
                <br />
            </div>            
            <div id="Posts" class="container" runat="server" style="font-size: 16px;">
                <h3 class="text-center">Recent Posts</h3>
                <div id="NoPostDiv" runat="server" class="well">
                    <h3 class="text-center">There are no posts to display</h3>
                </div>
            </div>
        </form>
    </div>
    <footer class="modal-footer footer">
        <p class="text-center">[Bailey & Bartling] - Copyright &copy;2015</p>
    </footer>
</body>
<script>
    function button_click(objTextBox, objBtnID) {
        if (window.event.keyCode == 13) {
            document.getElementById(objBtnID).focus();
            document.getElementById(objBtnID).click();
        }
    }
</script>
<script src = "https://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
<script src = "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/js/bootstrap.min.js"></script>
</html>
