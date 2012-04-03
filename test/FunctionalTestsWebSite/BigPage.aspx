<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
        e.Authenticated = true;
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:LoginStatus ID="LoginStatus1" runat="server" />
        <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" TitleText="[Log In]">
        </asp:Login>
        <table>
            <tr>
                <td>
        <asp:ChangePassword ID="ChangePassword1" runat="server">
        </asp:ChangePassword>
                </td>
                <td>
        <asp:PasswordRecovery ID="PasswordRecovery1" runat="server">
        </asp:PasswordRecovery>
                </td>
                <td>
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
                </td>
            </tr>
            <tr>
                <td>
        <asp:CreateUserWizard ID="CreateUserWizard2" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
                </td>
                <td>
        <asp:CreateUserWizard ID="CreateUserWizard3" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
                </td>
                <td>
        <asp:CreateUserWizard ID="CreateUserWizard4" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
                </td>
            </tr>
            <tr>
                <td style="height: 356px">
        <asp:CreateUserWizard ID="CreateUserWizard5" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
                </td>
                <td style="height: 356px">
                    <asp:CreateUserWizard ID="CreateUserWizard6" runat="server">
                        <WizardSteps>
                            <asp:CreateUserWizardStep runat="server">
                            </asp:CreateUserWizardStep>
                            <asp:CompleteWizardStep runat="server">
                            </asp:CompleteWizardStep>
                        </WizardSteps>
                    </asp:CreateUserWizard>
                </td>
                <td style="height: 356px">
                    <asp:CreateUserWizard ID="CreateUserWizard7" runat="server">
                        <WizardSteps>
                            <asp:CreateUserWizardStep runat="server">
                            </asp:CreateUserWizardStep>
                            <asp:CompleteWizardStep runat="server">
                            </asp:CompleteWizardStep>
                        </WizardSteps>
                    </asp:CreateUserWizard>
                </td>
            </tr>
            <tr>
                <td style="height: 356px">
                    <asp:CreateUserWizard ID="CreateUserWizard8" runat="server">
                        <WizardSteps>
                            <asp:CreateUserWizardStep runat="server">
                            </asp:CreateUserWizardStep>
                            <asp:CompleteWizardStep runat="server">
                            </asp:CompleteWizardStep>
                        </WizardSteps>
                    </asp:CreateUserWizard>
                </td>
                <td style="height: 356px">
                    <asp:CreateUserWizard ID="CreateUserWizard9" runat="server">
                        <WizardSteps>
                            <asp:CreateUserWizardStep runat="server">
                            </asp:CreateUserWizardStep>
                            <asp:CompleteWizardStep runat="server">
                            </asp:CompleteWizardStep>
                        </WizardSteps>
                    </asp:CreateUserWizard>
                </td>
                <td style="height: 356px">
                    <asp:CreateUserWizard ID="CreateUserWizard10" runat="server">
                        <WizardSteps>
                            <asp:CreateUserWizardStep runat="server">
                            </asp:CreateUserWizardStep>
                            <asp:CompleteWizardStep runat="server">
                            </asp:CompleteWizardStep>
                        </WizardSteps>
                    </asp:CreateUserWizard>
                </td>
            </tr>
        </table>
        <br />
        &nbsp; &nbsp; &nbsp; &nbsp;
    
    </div>
    </form>
</body>
</html>
