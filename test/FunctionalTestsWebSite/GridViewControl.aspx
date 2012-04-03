<%@ Page Language="C#" Debug="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox check = (CheckBox)sender;
        GridViewRow row = (GridViewRow) check.NamingContainer;
        int courseId = (int)GridView1.DataKeys[row.RowIndex].Value;

        QAScenarios.School school = new QAScenarios.School();
        school.SetRequiredCourse(courseId, check.Checked);

        PerformDataBind();
    }

    private void PerformDataBind()
    {
        GridView1.DataBind();
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager id="ScriptManager1" runat="server" enablepartialrendering="false"></asp:ScriptManager>
    <br />
        <asp:UpdatePanel id="UpdatePanel1" runat="server">
        <ContentTemplate>
            &nbsp;<br />
            <asp:GridView ID="GridView1" runat="server" 
                AllowPaging="True" 
                AllowSorting="True"
                DataKeyNames="Course_Id" 
                DataSourceID="CoursesObjectDataSource" 
                AutoGenerateColumns="False" PageSize="3" >
                <Columns>
                    <asp:CommandField ShowEditButton="True" UpdateText="ReadyToUpdateRow"  />
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:CommandField ShowDeleteButton="True" deletetext="ReadyToDelete" />
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
                    
                </Columns>
                <SelectedRowStyle BackColor="#80FF80" />
                <PagerSettings PageButtonCount="4" />
            </asp:GridView>
            <asp:ObjectDataSource ID="CoursesObjectDataSource" runat="server" SelectMethod="GetCourses"
                SortParameterName="sortColumn" TypeName="QAScenarios.School" UpdateMethod="UpdateCourseGrades" DeleteMethod="DeleteCourse">
            </asp:ObjectDataSource>
            <br />
            <asp:GridView ID="GridView2" runat="server" 
                AllowPaging="True" 
                AllowSorting="True"
                DataKeyNames="Course_Id" 
                DataSourceID="CoursesObjectDataSource" 
                AutoGenerateColumns="False" PageSize="3" >
                <PagerSettings Mode="NextPrevious" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" UpdateText="ReadyToUpdateRow"  />
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:CommandField ShowDeleteButton="True" deletetext="ReadyToDelete" />
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
                    
                </Columns>
                <SelectedRowStyle BackColor="#80FF80" />
            </asp:GridView>
            <br />
            <asp:GridView ID="GridView3" runat="server" 
                AllowPaging="True" 
                AllowSorting="True"
                DataKeyNames="Course_Id" 
                DataSourceID="CoursesObjectDataSource" 
                AutoGenerateColumns="False" PageSize="3" >
                <PagerSettings Mode="NextPreviousFirstLast" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" UpdateText="ReadyToUpdateRow"  />
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:CommandField ShowDeleteButton="True" deletetext="ReadyToDelete" />
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
                    
                </Columns>
                <SelectedRowStyle BackColor="#80FF80" />
            </asp:GridView>
            <br />
            <asp:GridView ID="GridView4" runat="server" 
                AllowPaging="True" 
                AllowSorting="True"
                DataKeyNames="Course_Id" 
                DataSourceID="CoursesObjectDataSource" 
                AutoGenerateColumns="False" PageSize="3" >
                <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" UpdateText="ReadyToUpdateRow"  />
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:CommandField ShowDeleteButton="True" deletetext="ReadyToDelete" />
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
                    
                </Columns>
                <SelectedRowStyle BackColor="#80FF80" />
            </asp:GridView>
        </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
