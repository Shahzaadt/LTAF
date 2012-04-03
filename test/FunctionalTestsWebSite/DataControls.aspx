<%@ Page Language="C#" Debug="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            int refresh = Int32.Parse(RefreshHiddenField.Value);
            refresh++;
            RefreshHiddenField.Value = refresh.ToString();
            UpdatePanelRefreshLabel.Text = "UpdatePanel Refresh #:" + refresh.ToString();
            MainPageRefreshLabel.Text = "MainPage Refresh #:" + refresh.ToString();
        }
    }

    protected void CourseDetailsView_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
    {
        PerformDataBind();
    }

    protected void CoursesGridView_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        PerformDataBind();
    }

    protected void CourseDetailsView_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
    {
        PerformDataBind();        
    }

    protected void CoursesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "AddExtraPoint")
        {
            QAScenarios.School school = new QAScenarios.School();
            school.AddExtraPoint(int.Parse(e.CommandArgument.ToString()));
        }

		if (e.CommandName == "RemoveExtraPoint")
		{
			QAScenarios.School school = new QAScenarios.School();
			school.RemoveExtraPoint(int.Parse(e.CommandArgument.ToString()));
		}

        CoursesGridView.DataBind();
    }

    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox check = (CheckBox)sender;
        GridViewRow row = (GridViewRow) check.NamingContainer;
        int courseId = (int) CoursesGridView.DataKeys[row.RowIndex].Value;

        QAScenarios.School school = new QAScenarios.School();
        school.SetRequiredCourse(courseId, check.Checked);

        PerformDataBind();
    }

    private void PerformDataBind()
    {
        CoursesGridView.DataBind();
        CourseDetailsView.DataBind();
        RequiedCoursesBulletedList.DataBind();
    }

    protected void AuthorLinkButton_Click(object sender, EventArgs e)
    {
        FavoriteAuthorLabel.Text = ((LinkButton)sender).Text;
        
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager id="ScriptManager1" runat="server" enablepartialrendering="false"></asp:ScriptManager>
    <div>
        <strong>DataControls Mini-Scenario</strong><br />
        <asp:Label ID="MainPageRefreshLabel" runat="server" Text="MainPage Refresh #:0"></asp:Label>
        <asp:UpdatePanel id="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="RefreshHiddenField" Value="0" runat="server" />
            <br />
            <strong><asp:Label ID="UpdatePanelRefreshLabel" runat="server" Text="UpdatePanel Refresh #:0"></asp:Label></strong>
            <br />
            <asp:GridView ID="CoursesGridView" runat="server" AllowPaging="True" AllowSorting="True"
                 DataKeyNames="Course_Id" DataSourceID="CoursesObjectDataSource" AutoGenerateColumns="False" OnRowUpdated="CoursesGridView_RowUpdated" OnRowCommand="CoursesGridView_RowCommand">
                <Columns>
                    <asp:CommandField ShowEditButton="True" UpdateText="ReadyToUpdateRow"  />
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:CommandField ButtonType="Link" ShowDeleteButton="True" deletetext="ReadyToDelete" />
                    <asp:BoundField DataField="Course_Id" HeaderText="Id" ReadOnly="True" SortExpression="Course_Id" />
                    <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" SortExpression="Name" />
                    <asp:TemplateField HeaderText="Required" SortExpression="Required">
                        <EditItemTemplate>
                            <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Required") %>' />
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" Checked='<%# Bind("Required") %>'
                                OnCheckedChanged="CheckBox1_CheckedChanged" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Grade1" HeaderText="Grade1" SortExpression="Grade1" />
                    <asp:BoundField DataField="Grade2" HeaderText="Grade2" SortExpression="Grade2" />
                    <asp:BoundField DataField="Grade3" HeaderText="Grade3" SortExpression="Grade3" />
                    <asp:TemplateField HeaderText="Final" SortExpression="Final">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Final") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:UpdatePanel id="InnerUpdatePanel" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Final") %>'></asp:Label>
                                    <asp:Button ID="ExtraPointButton" runat="server" Text="Extra Point!" CommandName="AddExtraPoint" CommandArgument='<%# Eval("Course_Id") %>' />
                                    <asp:Button ID="RemoveExtraPointButton" runat="server" Text="Remove Extra Point!" CommandName="RemoveExtraPoint" CommandArgument='<%# Eval("Course_Id") %>' />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <SelectedRowStyle BackColor="#80FF80" />
            </asp:GridView>
            <asp:ObjectDataSource ID="CoursesObjectDataSource" runat="server" SelectMethod="GetCourses"
                SortParameterName="sortColumn" TypeName="QAScenarios.School" UpdateMethod="UpdateCourseGrades" DeleteMethod="DeleteCourse">
            </asp:ObjectDataSource>
            <br />
            <table>
                <tr>
                    <td>
                        <asp:DetailsView ID="CourseDetailsView" runat="server" AutoGenerateRows="False" DataSourceID="CourseDetailsObjectDataSource"
                            Height="50px" Width="253px" AutoGenerateEditButton="True" DataKeyNames="Course_Id" OnItemUpdated="CourseDetailsView_ItemUpdated" OnItemInserted="CourseDetailsView_ItemInserted" AllowPaging="True">
                            <Fields>
                                <asp:BoundField DataField="Course_Id" HeaderText="Id" SortExpression="Course_Id" ReadOnly="True" />
                                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                                <asp:BoundField DataField="Details1" HeaderText="Details1" SortExpression="Details1" />
                                <asp:BoundField DataField="Details2" HeaderText="Details2" SortExpression="Details2" />
                                <asp:BoundField DataField="Details3" HeaderText="Details3" SortExpression="Details3" />
                                <asp:CheckBoxField DataField="Required" HeaderText="Required" SortExpression="Required" />
                                <asp:TemplateField HeaderText="Book" SortExpression="Book_Id">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Book_Id") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <InsertItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Book_Id") %>'></asp:TextBox>
                                    </InsertItemTemplate>
                                    <ItemTemplate>
                                        <asp:FormView ID="BookFormView" runat="server" DataSourceID="BookObjectDataSource">
                                            <ItemTemplate>
                                                Book_Id:
                                                <asp:Label ID="Book_IdLabel" runat="server" Text='<%# Bind("Book_Id") %>'></asp:Label><br />
                                                Title:
                                                <asp:Label ID="TitleLabel" runat="server" Text='<%# Bind("Title") %>'></asp:Label><br />
                                                Author:
                                                <asp:LinkButton ID="AuthorLinkButton" runat="server" OnClick="AuthorLinkButton_Click"
                                                    Text='<%# Eval("Author") %>'></asp:LinkButton><br />
                                            </ItemTemplate>
                                        </asp:FormView>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            <asp:CommandField InsertText="ReadyToInsertRow" NewText="ClickToCreateNewRow" ShowInsertButton="True" />
                            </Fields>
                        </asp:DetailsView>
                        <asp:ObjectDataSource ID="CourseDetailsObjectDataSource" runat="server" SelectMethod="GetCourseById"
                            TypeName="QAScenarios.School" UpdateMethod="UpdateCourseDetails" InsertMethod="InsertCourse">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="CoursesGridView" Name="Course_Id" PropertyName="SelectedValue"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                                        <asp:ObjectDataSource ID="BookObjectDataSource" runat="server" SelectMethod="GetBookForCourse"
                                            TypeName="QAScenarios.School">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="CoursesGridView" Name="Course_Id" PropertyName="SelectedValue"
                                                    Type="Int32" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                    </td>
                    <td valign="top">
                        RequiredClasses:<br />
                        <asp:BulletedList ID="RequiedCoursesBulletedList" runat="server" DataSourceID="RequiredCoursesObjectDataSource"
                            DataTextField="Name" DataValueField="Course_Id">
                        </asp:BulletedList>
                        <br />
                        <br />
                        Favorite Author:
                        <asp:Label ID="FavoriteAuthorLabel" runat="server" Font-Bold="True"></asp:Label><br />
                        <asp:ObjectDataSource ID="RequiredCoursesObjectDataSource" runat="server" SelectMethod="RequiredClasses"
                            TypeName="QAScenarios.School"></asp:ObjectDataSource>
                        &nbsp; &nbsp;</td>
                </tr>
            </table>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
