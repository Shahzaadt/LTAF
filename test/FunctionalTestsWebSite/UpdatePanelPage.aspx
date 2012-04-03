<%@ Page LANGUAGE="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server" >

    protected void MainPageRefresh_PreRender(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            int refresh = Int32.Parse(MainPageRefresh.Text);
            refresh++;
            MainPageRefresh.Text = refresh.ToString();
        }
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server" >
		<title >Untitled Page</title>
	</head>
	<body >
		<form id="form1" runat="server" >
    [Test Page Loaded]
    
			<asp:ScriptManager runat="Server" ID="ScriptManager1" EnablePartialRendering="true" ></asp:ScriptManager>
			<div >
        Page Refresh #:
				<asp:Label runat="server" ID="MainPageRefresh" Text="0" OnPreRender="MainPageRefresh_PreRender" ></asp:Label>
				<asp:Button runat="server" Id="Button1" Text="Regular Post" />
				<asp:PlaceHolder runat="server" id="PlaceHolder1" ></asp:PlaceHolder>
				<asp:UpdatePanel runat="server" ID="UpdatePanel1" >
					<ContentTemplate >
						<div >
							<strong >CoreControls MiniScenario
								<br />
								<br />
							</strong>
						</div>
						<div >
							<asp:Label runat="server" ID="MiniScenarioRefresh" Text="[Async Refresh #0]" ></asp:Label>
							<asp:Wizard runat="server" ID="Wizard1" Width="543px" ActiveStepIndex="0" FinishCompleteButtonImageUrl="~/Finish.JPG" FinishCompleteButtonType="Image" OnFinishButtonClick="Wizard1_FinishButtonClick" StepNextButtonType="Link" StepNextButtonText="Next" BorderColor="Navy" BorderStyle="Solid" BorderWidth="1px" >
								<WizardSteps >
									<asp:WizardStep runat="server" ID="WizardStep1" Title="Step 1" >
										<asp:Panel runat="server" ID="Panel1" Width="100%" >
											<br />
											<table width="100%" >
												<tr >
													<td style="width: 18px" valign="top" >Name:</td>
													<td style="width: 156px" valign="top" >
														<asp:TextBox runat="server" ID="NameTextBox" AutoPostBack="True" OnTextChanged="NameTextBox_TextChanged" ></asp:TextBox>
														<asp:Literal runat="server" ID="NameLiteral" ></asp:Literal>
													</td>
												</tr>
												<tr >
													<td valign="top" style="width: 18px" >Gender:</td>
													<td valign="top" style="width: 156px" >
														<asp:RadioButtonList runat="server" ID="GenderRadioButtonList" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="GenderRadioButtonList_SelectedIndexChanged" >
															<asp:ListItem >Male</asp:ListItem>
															<asp:ListItem >Female</asp:ListItem>
														</asp:RadioButtonList>
														<asp:Literal runat="server" ID="GenderLiteral" ></asp:Literal>
													</td>
												</tr>
											</table>
										</asp:Panel>
									</asp:WizardStep>
									<asp:WizardStep runat="server" ID="WizardStep2" Title="Step 2" >
										<table width="100%" >
											<tr >
												<td valign="top" style="width: 18px" >
                            Country/Region:</td>
												<td valign="top" >
													<asp:DropDownList runat="server" ID="CountryDropDownList" AutoPostBack="True" OnSelectedIndexChanged="CountryDropDownList_SelectedIndexChanged" >
														<asp:ListItem >USA</asp:ListItem>
														<asp:ListItem >Mexico</asp:ListItem>
														<asp:ListItem >Canada</asp:ListItem>
													</asp:DropDownList>
												</td>
											</tr>
											<tr >
												<td style="width: 18px; height: 72px;" valign="top" >State/Province:</td>
												<td valign="top" style="height: 72px" >
													<asp:ListBox runat="server" ID="StateListBox" AutoPostBack="True" OnSelectedIndexChanged="StateListBox_SelectedIndexChanged" >
														<asp:ListItem >Please Select a Country/Region</asp:ListItem>
													</asp:ListBox>
												</td>
											</tr>
											<tr >
												<td style="width: 18px; height: 21px" valign="top" >City:</td>
												<td style="height: 21px" valign="top" >
													<asp:ListBox runat="server" ID="CityListBox" >
														<asp:ListItem >Please Select a State/Province</asp:ListItem>
													</asp:ListBox>
												</td>
											</tr>
										</table>
									</asp:WizardStep>
									<asp:WizardStep runat="server" ID="WizardStep3" Title="Step 3" >
										<table width="100%" >
											<tr >
												<td valign="top" colspan="2" >
													<asp:CheckBox runat="server" ID="NewsCheckBox" AutoPostBack="True" OnCheckedChanged="NewsCheckBox_CheckedChanged" Text="I want to receive news letter" />
													<asp:Literal runat="server" ID="NewsLiteral" ></asp:Literal>
												</td>
											</tr>
											<tr >
												<td colspan="2" valign="top" >
													<asp:RadioButton runat="server" ID="ULARadioButton" AutoPostBack="True" OnCheckedChanged="ULARadioButton_CheckedChanged" Text="I read the ULA" />
													<asp:Literal runat="server" ID="ULALiteral" ></asp:Literal>
												</td>
											</tr>
											<tr >
												<td colspan="2" valign="top" >
													<asp:CheckBoxList runat="server" ID="GamesCheckBoxList" AutoPostBack="True" OnSelectedIndexChanged="GamesCheckBoxList_SelectedIndexChanged" >
														<asp:ListItem Value="PlayStation" >I own PlayStation</asp:ListItem>
														<asp:ListItem Value="XBox" >I own XBox</asp:ListItem>
														<asp:ListItem Value="GameCube" >I own GameCube</asp:ListItem>
													</asp:CheckBoxList>
													<asp:Literal runat="server" ID="GamesLiteral" ></asp:Literal>
												</td>
											</tr>
										</table>
									</asp:WizardStep>
									<asp:WizardStep runat="server" ID="WizardStep4" Title="Step 4" >
										<table width="100%" >
											<tr >
												<td style="width: 200px" valign="top" >Select a Date:</td>
												<td valign="top" >
													<asp:Calendar runat="server" ID="DateCalendar" OnSelectionChanged="DateCalendar_SelectionChanged" OnVisibleMonthChanged="DateCalendar_VisibleMonthChanged" VisibleDate="2006-06-01" NextMonthText="NextMonth" ></asp:Calendar>
													<asp:Literal runat="server" ID="DateLiteral" ></asp:Literal>
													<br />
													<asp:Literal runat="server" ID="MonthLiteral" ></asp:Literal>
												</td>
											</tr>
										</table>
									</asp:WizardStep>
									<asp:WizardStep runat="server" ID="WizardStep5" StepType="Complete" Title="Step 5" >
										<strong >Info Captured:</strong>
										<asp:Label runat="server" ID="ReceiptLabel" ></asp:Label>
										<br />
									</asp:WizardStep>
								</WizardSteps>
								<NavigationStyle VerticalAlign="Top" />
								<SideBarStyle BackColor="#C0FFC0" VerticalAlign="Top" />
								<HeaderStyle HorizontalAlign="Right" BackColor="#C0FFC0" />
							</asp:Wizard>
							
            &nbsp;
        
						</div>
						<div >
							<asp:HiddenField runat="server" ID="RefreshNumberHiddenField" Value="0" />
							
            &nbsp;
    
						</div>
						<strong >
        Controls: </strong>
						<ul >
							<li >TextBox (AutoPostback)</li>
							<li >CheckBox (AutoPostback)</li>
							<li >RadioButton (AutoPostback)</li>
							<li >CheckBoxList (AutoPostback)</li>
							<li >RadioButtonList (AutoPostback)</li>
							<li >DropDownList (AutoPostback)</li>
							<li >ListBox (AutoPostback)</li>
							<li >Calendar</li>
							<li >HiddenField</li>
							<li >Button</li>
							<li >LinkButton</li>
							<li >ImageButton</li>
							<li >Wizard (back/forward navigation; sidebar navigation)&nbsp;</li>
						</ul>
					</ContentTemplate>
				</asp:UpdatePanel>
			</div>
		</form>
	</body>
</html>
<script runat="server" >
    
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            int refresh = Int32.Parse(RefreshNumberHiddenField.Value);
            refresh++;
            RefreshNumberHiddenField.Value = refresh.ToString();
            MiniScenarioRefresh.Text = "[Async Refresh #" + RefreshNumberHiddenField.Value + "]";
        }
    }

    protected void NameTextBox_TextChanged(object sender, EventArgs e)
    {
        NameLiteral.Text = "[Your Name:" + NameTextBox.Text + "]";
    }

    protected void GenderRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        GenderLiteral.Text = "[Your Gender:" + GenderRadioButtonList.SelectedValue + "]";
    }

    protected void CountryDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        ArrayList states = new ArrayList();
        switch (CountryDropDownList.SelectedIndex)
        {
            case 0:
                states.Add("Washington");
                states.Add("Texas");
                break;
            case 1:
                states.Add("Nuevo Leon");
                states.Add("Guadalajara");
                break;
            case 2:
                states.Add("Quebec");
                states.Add("Ontario");
                break;
        }
        StateListBox.DataSource = states;
        StateListBox.DataBind();
    }

    protected void StateListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (StateListBox.Items.Count == 1)
        {
            return;
        }

        ArrayList cities = new ArrayList();
        cities.Add(StateListBox.SelectedValue + "_City1");
        cities.Add(StateListBox.SelectedValue + "_City2");
        cities.Add(StateListBox.SelectedValue + "_City3");
        cities.Add(StateListBox.SelectedValue + "_City4");

        CityListBox.DataSource = cities;
        CityListBox.DataBind();
    }


    protected void NewsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (NewsCheckBox.Checked)
        {
            NewsLiteral.Text = "[You want to receieve News]";
        }
        else
        {
            NewsLiteral.Text = "[You don't want to receive News]";
        }
    }

    protected void ULARadioButton_CheckedChanged(object sender, EventArgs e)
    {
        if (ULARadioButton.Checked)
        {
            ULALiteral.Text = "[You have read the license]";
        }
    }

    protected void GamesCheckBoxList_SelectedIndexChanged(object sender, EventArgs e)
    {
        StringBuilder games = new StringBuilder();
        foreach (ListItem i in GamesCheckBoxList.Items)
        {
            if (i.Selected)
            {
                games.Append(i.Value + "-");
            }
        }
        if (games.Length > 0)
        {
            games.Length--;
        }
        GamesLiteral.Text = "[" + games.ToString() + "]";
    }

    protected void DateCalendar_SelectionChanged(object sender, EventArgs e)
    {
        DateLiteral.Text = "[Your date=" + DateCalendar.SelectedDate.ToString() + "]";
    }

    protected void DateCalendar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
    {
        MonthLiteral.Text = "[Your month=" + e.NewDate.Month + "]";
    }

    protected void Wizard1_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        StringBuilder receipt = new StringBuilder();
        receipt.Append("Name:" + NameTextBox.Text);
        receipt.Append("|Gender:" + GenderRadioButtonList.SelectedValue);
        receipt.Append("|Country:" + CountryDropDownList.SelectedValue);
        ReceiptLabel.Text = receipt.ToString();
    }

</script>
