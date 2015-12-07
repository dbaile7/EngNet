<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" %>

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
                                <li>
                                    <a href="Default.aspx">
                                        Home <span class="sr-only">(current)</span>
                                    </a>
                                </li>
                                <li class="active">
                                    <a id="A1" runat="server" href="Register.aspx">
                                        Register
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>
            </header>
            <br /><br /><br /><br /><br /><br />
            <div class="jumbotron container text-center">
                <h1>eng<strong>NET</strong></h1>
                <h3>Register Your New Account</h3>
            </div>
            <asp:UpdatePanel ID="UpdatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="container text-center" style="background-color: rgba(0, 0, 0, 0.03); border-radius: 15px;">
                        <br />
                        <h3>Just a little info to get you started...</h3>
                        <span class="lead">What's your name?</span>
                        <p id="NameMessage" class="text-danger" runat="server"></p>
                        <div style="width: 200px; margin: 0 auto;">
                            <asp:TextBox CssClass="form-control" ID="NameBox" runat="server" placeholder="ex: Daniel Everett Bailey" MaxLength="150"></asp:TextBox>
                        </div>
                        <br />
                        <span class="lead">Now we need your email</span>
                        <p id="EmailMessage" class="text-danger" runat="server"></p>
                        <div style="width: 200px; margin: 0 auto;">
                            <asp:TextBox CssClass="form-control" ID="EmailBox" runat="server" TextMode="Email" placeholder="Email" MaxLength="150"></asp:TextBox>
                        </div>
                        (We aren't going to email you - I promise)
                        <br />
                        <br />
                        <span class="lead">Choose a password</span>
                        <br />
                        Your safety is important
                        <p id="PassMessage" class="text-danger" runat="server"></p>
                        <div style="width: 200px; margin: 0 auto;">
                            <asp:TextBox CssClass="form-control" ID="PassBox" runat="server" TextMode="Password" placeholder="Password" MaxLength="150"></asp:TextBox>
                            <br />
                            Confirm your password
                            <asp:TextBox CssClass="form-control" ID="PassConfirmBox" runat="server" TextMode="Password" placeholder="Confirm Password" MaxLength="150"></asp:TextBox>
                        </div>
                        <br />
                        <p class="lead">
                            That's it! 
                            <br />
                            If it's all good we can make your page...
                        </p>
                        <asp:Button CssClass="btn btn-info" ID="RegisterButton" runat="server" Text="Register" OnClick="RegisterButton_Click" />
                        <br />
                        <p id="ErrorMessage" class="text-danger" runat="server"></p>
                        <br />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
            <br />
        </form>
    </div>

    <footer class="modal-footer footer">
        <p class="text-center">[Bailey & Bartling] - Copyright &copy;2015</p>
    </footer>
</body>
<script src = "https://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
<script src = "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/js/bootstrap.min.js"></script>
</html>