<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Friendship.aspx.cs" Inherits="Friendship" %>

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
            <div id="Feed">
                <h1 class="well text-center" style="position:relative; top: -35px; border-radius: 0;">Friendship</h1>
                <div id="Posts" runat="server" class="container">
                    <div id="NoPostsDiv" runat="server">
                        <h3 class="text-center">There are no posts to display</h3>
                        <p class="lead text-center" style="font-size: 14px;">(Posts show up here if you like your friend's post and they like your posts)</p>
                    </div>
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
