<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>engNET</title>
    <link rel="stylesheet" href="styles/bootstrap.min.css" />
    <link rel="stylesheet" href="styles/style.css" />
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
                                <li class="active">
                                    <a href="Default.aspx" title="Home">
                                        Home <span class="sr-only">(current)</span>
                                    </a>
                                </li>
                                <li>
                                    <a id="RegisterButton" runat="server" title="Register" href="Register.aspx">
                                        Register
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
                                <li>
                                    <a id="UserButton" runat="server" title="View Profile" href="#"></a>
                                </li>
                                <li>
                                    <a id="LogoutButton" runat="server" title="Logout" href="#" onServerClick="LogoutButton_Click">Logout</a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>
                <br /><br /><br /><br /><br /><br />
                <div id="BigHeader" runat="server" class="jumbotron container text-center">
                    <h1>eng<strong>NET</strong></h1>
                    <br />
                    <p>
                        By <strong>Eng</strong> students...<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;...for <strong>Eng</strong> students
                    </p>
                </div>
            </header>
    
            <!-- Login container -->
            <div id="Login" runat="server" class="container">
                <asp:UpdatePanel ID="LoginPanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="well">
                            <h3 class="text-center">Login To engNET</h3>
                            <br />
                            <div style="width: 300px; margin: 0 auto;">
                                <p id="LoginMessage" class="text-danger" runat="server"></p>
                                <asp:TextBox CssClass="form-control" ID="EmailBox" runat="server" placeholder="Email" TextMode="Email" MaxLength="150"></asp:TextBox>
                                <br />
                                <asp:TextBox CssClass="form-control" ID="PassBox" runat="server" placeholder="Password" TextMode="Password" MaxLength="150"></asp:TextBox>
                                <br /> 
                                <div style="height: 60px;">
                                    <asp:Button CssClass="btn btn-info" ID="LoginButton" runat="server" Text="Login" style="float: right;" OnClick="LoginButton_Click" />
                                </div>
                                <hr />
                                <br />
                                <h3>OR...</h3>
                                <p class="lead text-center">Start your story today<p>
                                <div style="height: 60px">
                                    <a href="Register.aspx"><input type="button" class="btn btn-primary" style="float: right;" value="Register" /></a>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <!-- Feed / Dashboard -->
            <div id="Feed" runat="server">
                <h1 class="well text-center" style="position:relative; top: -35px; border-radius: 0;">YOUR ENG<strong>NET</strong> FEED</h1>
                <div id="Posts" runat="server" class="container">
                    <div id="NoPosts" runat="server" class="well text-center">
                        <h3>No Posts Yet</h3>
                        <p class="lead">
                            (Or they might be loading...)
                        </p>
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
