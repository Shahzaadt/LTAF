<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Untitled Page" %>
<%@ Import Namespace="QAScenarios" %>

<script runat="server">

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
            School school = new School();
            school.AddExtraPoint(int.Parse(e.CommandArgument.ToString()));
        }

        CoursesGridView.DataBind();
    }

    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox check = (CheckBox)sender;
        GridViewRow row = (GridViewRow)check.NamingContainer;
        int courseId = (int)CoursesGridView.DataKeys[row.RowIndex].Value;

        School school = new School();
        school.SetRequiredCourse(courseId, check.Checked);

        PerformDataBind();
    }

    private void PerformDataBind()
    {
        CoursesGridView.DataBind();
        CourseDetailsView.DataBind();
        RequiedCoursesBulletedList.DataBind();
    }
</script>

<asp:Content ID="Content3" ContentPlaceHolderID="PaperContentPlaceHolder" Runat="Server">
	<asp:ScriptManager id="ScriptManager1" runat="server" enablepartialrendering="true"></asp:ScriptManager>
    <div>
        <asp:UpdatePanel id="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:GridView ID="CoursesGridView" runat="server" AllowPaging="True" AllowSorting="True"
                 DataKeyNames="Course_Id" DataSourceID="CoursesObjectDataSource" 
                AutoGenerateColumns="False" OnRowUpdated="CoursesGridView_RowUpdated" 
                OnRowCommand="CoursesGridView_RowCommand" BackColor="White" 
                BorderColor="White" BorderWidth="0px" CellPadding="2" ForeColor="Black" 
                GridLines="None" Width="100%">
                <RowStyle HorizontalAlign="Center" />
                <Columns>
                    <asp:CommandField ShowSelectButton="True" ButtonType="Image" 
						SelectImageUrl="~/images/details.gif" SelectText="Deatils" >
                        <ControlStyle Font-Underline="True" ForeColor="Blue" />
                    </asp:CommandField>
                    <asp:CommandField ShowEditButton="True" UpdateText="Update" ButtonType="Image" 
                        CancelImageUrl="~/images/Edit_No.png" EditImageUrl="~/images/Edit.png" 
                        UpdateImageUrl="~/images/Edit_Yes.png"  >
                        <ControlStyle Font-Underline="True" ForeColor="Blue" />
                    </asp:CommandField>
                    <asp:CommandField ButtonType="Image" ShowDeleteButton="True" 
                        deletetext="Delete" DeleteImageUrl="~/images/trash.png" >
                        <ControlStyle Font-Underline="True" ForeColor="Blue" />
                    </asp:CommandField>
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
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Grade1" HeaderText="Grade 1" SortExpression="Grade1" />
                    <asp:BoundField DataField="Grade2" HeaderText="Grade 2" SortExpression="Grade2" />
                    <asp:BoundField DataField="Grade3" HeaderText="Grade 3" SortExpression="Grade3" />
                    <asp:TemplateField HeaderText="Final" SortExpression="Final">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Final") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:UpdatePanel id="InnerUpdatePanel" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Final") %>'></asp:Label>&nbsp;&nbsp;<asp:ImageButton ID="ExtraPointButton" runat="server" ImageUrl="~/images/star.png" AlternateText="Extra credit" CommandName="AddExtraPoint" CommandArgument='<%# Eval("Course_Id") %>' />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle />
                <PagerStyle BackColor="White" Font-Underline="False" ForeColor="Blue" HorizontalAlign="Left" />
                <SelectedRowStyle BackColor="#ffffd8" />
                <HeaderStyle BackColor="#6a9df0" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#dce0e8" />
            </asp:GridView>
            <asp:ObjectDataSource ID="CoursesObjectDataSource" runat="server" SelectMethod="GetCourses"
                SortParameterName="sortColumn" TypeName="QAScenarios.School" UpdateMethod="UpdateCourseGrades" DeleteMethod="DeleteCourse">
            </asp:ObjectDataSource>
            <br />
            <table>
                <tr>
                    <td>
                        <asp:DetailsView ID="CourseDetailsView" runat="server" AutoGenerateRows="False" DataSourceID="CourseDetailsObjectDataSource"
                            Height="50px" Width="412px" 
                            DataKeyNames="Course_Id" OnItemUpdated="CourseDetailsView_ItemUpdated" 
                            OnItemInserted="CourseDetailsView_ItemInserted" AllowPaging="True" 
                            BackColor="#dce0e8" BorderColor="#363636" BorderWidth="0"
                            CellPadding="2" ForeColor="Black" GridLines="None" >
                            <FooterStyle BackColor="Tan" />
                            <FieldHeaderStyle Font-Bold="True" />
                            <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
                                HorizontalAlign="Center" />
                            <Fields>
                                <asp:BoundField DataField="Course_Id" HeaderText="Id" SortExpression="Course_Id" ReadOnly="True" />
                                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                                <asp:BoundField DataField="Details1" HeaderText="Details" SortExpression="Details1" />
                                <asp:BoundField DataField="Details2"  />
                                <asp:BoundField DataField="Details3" />
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
                                                <i>Book ISBN:</i>
                                                <asp:Label ID="Book_IdLabel" runat="server" Text='<%# Bind("Book_Id") %>'></asp:Label><br />
                                                <i>Title:</i>
                                                <asp:Label ID="TitleLabel" runat="server" Text='<%# Bind("Title") %>'></asp:Label><br />
                                                <i>Author:</i>
                                                <asp:Label ID="AuthorLabel" runat="server" Text='<%# Bind("Author") %>'></asp:Label><br />
                                            </ItemTemplate>
                                        </asp:FormView>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            <asp:CommandField InsertText="Insert" NewText="New" ShowInsertButton="True" 
                                    ButtonType="Image" CancelImageUrl="~/images/Delete.png" 
                                    EditImageUrl="~/images/Edit.png" InsertImageUrl="~/images/Check.png" 
                                    NewImageUrl="~/images/Add.png" />
                                <asp:CommandField ButtonType="Image" CancelImageUrl="~/images/Edit_No.png" 
                                    EditImageUrl="~/images/Edit.png" ShowEditButton="True" 
                                    UpdateImageUrl="~/images/Edit_Yes.png" />
                            </Fields>
                            <EditRowStyle BackColor="#dce0e8" ForeColor="#363636" />
                            <AlternatingRowStyle BackColor="#dce0e8" />
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
                        <b>&nbsp;&nbsp;&nbsp;RequiredClasses:</b><br />
                        <asp:BulletedList ID="RequiedCoursesBulletedList" runat="server" DataSourceID="RequiredCoursesObjectDataSource"
                            DataTextField="Name" DataValueField="Course_Id">
                        </asp:BulletedList>
                        <asp:ObjectDataSource ID="RequiredCoursesObjectDataSource" runat="server" SelectMethod="RequiredClasses"
                            TypeName="QAScenarios.School"></asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

