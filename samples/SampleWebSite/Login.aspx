<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Untitled Page" %>

<script runat="server">

    protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
    {
        FormsAuthentication.SetAuthCookie(CreateUserWizard1.UserName, false);
        Response.Redirect("Home.aspx");
    }

</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentPlaceHolder" Runat="Server">

    <script type="text/javascript">
		function showSignUp(){
			document.getElementById("loginForm").className = "no-display";
			document.getElementById("signupForm").className = "display-block";
		}
		function showSignIn(){
			document.getElementById("loginForm").className = "display-block";
			document.getElementById("signupForm").className = "no-display";
		}
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PaperContentPlaceHolder" Runat="Server">
    <div id="twoCol">
	<div id="left">
		<br />
		<blockquote class="blockQuote">LTAF is a web automation framework designed to run within an ASP.NET application.</blockquote>
	</div>
	<div id="right">
		<div id="loginForm" class="display-block">
		    <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/Home.aspx">
                <LayoutTemplate>
                   <a class="redLink" href="JavaScript:showSignUp();">Not a member?</a>			
			        <div class="textboxBorder">
				        <img class="textboxGraphic" src="images/user.png" alt="" title="Username" />
				        <asp:TextBox ID="UserName" runat="server" Width="200px" CssClass="customTextBox"></asp:TextBox>
				        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                            ControlToValidate="UserName" ErrorMessage="User Name is required." 
                            ToolTip="User Name is required." ValidationGroup="Login1" CssClass="validator" >*</asp:RequiredFieldValidator>
			        </div>
			        
			        <div class="textboxBorder">
				        <img class="textboxGraphic" src="images/lock.png" alt="" title="password" />
				        <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px" CssClass="customTextBox"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                            ControlToValidate="Password" ErrorMessage="Password is required." 
                            ToolTip="Password is required." ValidationGroup="Login1" CssClass="validator">*</asp:RequiredFieldValidator>
			        </div>	
                    <asp:Label ID="FailureText" runat="server" EnableViewState="False" ForeColor="Red" Font-Bold="true"></asp:Label>
			        	
			         <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" 
                            ValidationGroup="Login1" CssClass="redButton" />			
            </LayoutTemplate>
        </asp:Login>
						
		</div>
		<div id="signupForm" class="no-display">
		
		  <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" 
                oncreateduser="CreateUserWizard1_CreatedUser" >
        <WizardSteps>
            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                <ContentTemplate>
                  	<a class="redLink" href="JavaScript:showSignIn();">Member Login..</a>
			<div class="textboxBorder">
				<img class="textboxGraphic" src="images/user.png" alt="" title="Username" />
				<asp:TextBox ID="UserName" runat="server" Width="200px" CssClass="customTextBox"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                            ControlToValidate="UserName" ErrorMessage="User Name is required." 
                            ToolTip="User Name is required." ValidationGroup="CreateUserWizard1" CssClass="validator" >*</asp:RequiredFieldValidator>

			</div>
			<div class="textboxBorder">
				<img class="textboxGraphic" src="images/lock.png" alt="" title="password" />
                <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px" CssClass="customTextBox"></asp:TextBox>
			     <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                            ControlToValidate="Password" ErrorMessage="Password is required." 
                            ToolTip="Password is required." ValidationGroup="CreateUserWizard1" CssClass="validator">*</asp:RequiredFieldValidator>
			</div>
			<div class="textboxBorder">
				<img class="textboxGraphic" src="images/lock.png" alt="" title="confirm password" />
                <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" Width="200px" CssClass="customTextBox"></asp:TextBox>
				<asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" 
                            ControlToValidate="ConfirmPassword" 
                            ErrorMessage="Confirm Password is required." 
                            ToolTip="Confirm Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
			</div>
			<div class="textboxBorder">
				<img class="textboxGraphic" src="images/mail.png" alt="" title="Email" />
                <asp:TextBox ID="Email" runat="server" Width="200px" CssClass="customTextBox"></asp:TextBox>
				<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                            ControlToValidate="Email" ErrorMessage="Email is required." 
                            ToolTip="Email is required." ValidationGroup="CreateUserWizard1" CssClass="validator">*</asp:RequiredFieldValidator>
			</div>
			<asp:CompareValidator ID="PasswordCompare" runat="server" 
                                    ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                                    Display="Dynamic" 
                                    ErrorMessage="The Password and Confirmation Password must match." 
                                    ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
                <asp:Label ID="ErrorMessage" runat="server" EnableViewState="False" Font-Bold="true"></asp:Label>
                
                </ContentTemplate>
                <CustomNavigationTemplate>
                    <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" 
                                    Text="Sign Up" ValidationGroup="CreateUserWizard1" CssClass="redButton" />
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
<asp:CompleteWizardStep runat="server"></asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
		
		
		</div>
	</div>
</div>
</asp:Content>

