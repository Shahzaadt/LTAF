using System;
using LTAF;

[WebTestClass]
public class UserManagementTests
{
    [WebTestMethod]
    public void SignInAndSignOut()
    {
        // Navigate to the login page
        HtmlPage page = new HtmlPage("Login.aspx");

        // Fill Login control user/password and click login button
        page.Elements.Find("UserName").SetText("ValidUser");
        page.Elements.Find("Password").SetText("foo");
        page.Elements.Find("LoginButton").Click(WaitFor.Postback);

        // Verify content of the Home page
        Assert.AreEqual("Welcome back ValidUser!", page.Elements.Find("LoginName1").GetInnerText());

        // Click the logout tab
        page.Elements.Find("tab-signout").Click(WaitFor.Postback);

        // Verify login tab now exists.
        Assert.IsTrue(page.Elements.Exists("tab-login"));
    }

    [WebTestMethod]
    public void ErrorIfIncorrectPassword()
    {
        // Navigate to the login page
        HtmlPage page = new HtmlPage("Login.aspx");

        // Verify there is no failure text
        Assert.AreEqual("", page.Elements.Find("FailureText").GetInnerText());

        // Attempt to login with an incorrect password
        page.Elements.Find("UserName").SetText("InvalidUser");
        page.Elements.Find("Password").SetText("foo");
        page.Elements.Find("LoginButton").Click(LTAF.WaitFor.Postback);

        // Verify error message is displayed
        Assert.AreEqual("Your login attempt was not successful. Please try again.", page.Elements.Find("FailureText").GetInnerText());
    }

    [WebTestMethod]
    public void ValidatorsShownIfTextBoxesNotFilled()
    {
        // Navigate to the login page
        HtmlPage page = new HtmlPage("Login.aspx");

        // Verify validators are not visible
        Assert.IsFalse(page.Elements.Find("UserNameRequired").IsVisible());
        Assert.IsFalse(page.Elements.Find("PasswordRequired").IsVisible());
        
        // Click the login button (do not wait for postback)
        page.Elements.Find("LoginButton").Click();

        // Verify validators are now visible
        Assert.IsTrue(page.Elements.Find("UserNameRequired").IsVisible());
        Assert.IsTrue(page.Elements.Find("PasswordRequired").IsVisible());
    }

    [WebTestMethod]
    public void ToggleCreateUser()
    {
        // Navigate to the login page
        HtmlPage page = new HtmlPage("Login.aspx");

        // Verify css classes of the Login control and CreateUserWizard control divs
        Assert.AreEqual("display-block", page.Elements.Find("loginForm").GetAttributes().Class);
        Assert.AreEqual("no-display", page.Elements.Find("signupForm").GetAttributes().Class);

       // Click on the link to toggle divs
       page.Elements.Find("a", "Not a member?", 0).Click();

       // Verify css classes of the Login control and CreateUserWizard control divs
       Assert.AreEqual("no-display", page.Elements.Find("loginForm").GetAttributes().Class);
       Assert.AreEqual("display-block", page.Elements.Find("signupForm").GetAttributes().Class);
    }


    [WebTestMethod]
    public void CreateUserAndSignOut()
    {
        // Navigate to the login page
        HtmlPage page = new HtmlPage("Login.aspx");

        // Click on the link to toggle divs
        page.Elements.Find("a", "Not a member?", 0).Click();

        // Get a hold of the element that represents the CreateUserWizard
        HtmlElement createUserWizard = page.Elements.Find("CreateUserWizard1");

        // Fill control and click the next step button
        createUserWizard.ChildElements.Find("UserName").SetText("NewValidUser");
        createUserWizard.ChildElements.Find("Password").SetText("foo");
        createUserWizard.ChildElements.Find("ConfirmPassword").SetText("foo");
        createUserWizard.ChildElements.Find("Email").SetText("foo@bar.com");
        createUserWizard.ChildElements.Find("StepNextButton").Click(WaitFor.Postback);

        // Verify content of the Home page        
        Assert.AreEqual("Welcome back NewValidUser!", page.Elements.Find("LoginName1").GetInnerText());

        // Click the signout tab
        page.Elements.Find("tab-signout").Click(WaitFor.Postback);

        // Verify sign in tab is now present
        Assert.IsTrue(page.Elements.Exists("tab-login"));
    }
}
