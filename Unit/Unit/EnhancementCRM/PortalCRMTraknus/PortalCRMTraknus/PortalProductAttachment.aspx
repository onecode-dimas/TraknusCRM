<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PortalProductAttachment.aspx.cs" Inherits="PortalCRMTraknus.PortalProductAttachment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Select Product Attachment</title>
    <style type="text/css">
        body
        {
            background-color:#E9EDF1;
            margin:0; padding:0;
            font-family: Segoe UI, Tahoma, Arial;
            font-size:11px;
        }
        .ms-crm-Error-Header
        {
            margin-bottom: 4px;
            font-size:14px;
            color:#000;
            font-weight:bold;
        }
        #ErrorMessage
        {
            font-size:12px;
            color:#000;
        }
        .hr-crm-Dialog-Footer
        {
        border-top-color: #A4ABB2;
        }
        .ms-crm-Dialog-Footer
        {
        text-align: right;
        height:			30px;
        padding:		10px;
        border-top-style: solid;
        border-top-width: 1px;
        border-top-color: #A4ABB2;
        }

        #container1
        {
            width: 700px;
            height: 300px;
            margin:0 auto;
        }
        .ms-crm-Button
        {
            margin:0 auto;
            line-height: 18px;
            height: 20px;
            width: 84px;
            display:block;
            text-align: center;
            cursor: pointer;
            border-width: 1px;
            border-style: solid;
            border-color:#333;
            color:#333;
            background-repeat: repeat-x;
            background-image: url(ButtonNormalGradient.png);
            padding-left: 5px;
            padding-right: 5px;
            font-size:11px;
            font-family: Segoe UI, Tahoma, Arial;
            text-decoration: none;
            overflow:hidden;
        }
        a:hover
        {
            background-image: url(ButtonSelectedGradient.png);
            text-decoration: none;
        }
        table
        {            
            border:1px solid #a5acb5;
            width: 99%;
        }
        th,
        td
        {
            margin:0;padding:1px 5px;   
        }
        th
        {
            padding:5px 5px;   
            border:none;
            background-color:#F7F7FF;
            font-weight:normal;
            border:1px solid #a5acb5;
            border-bottom:none;
            border-top:none;
        }
        td
        {
            border:1px solid #a5acb5;
            background-color:#FFFFFF;
        }
        tr.ms-crm-control,
        td
        {
            
        }
        tr.ms-crm-control th,
        th
        {
            
        }
        .ms-crm-List-DataCell	
        {
            border:medium none;
            border-bottom:#dbdee1 1px solid;
            margin:0;padding:0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container1" runat="server">
            <h3>Add Product Attachment</h3>
            
            <center>
                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack=true
                    onselectedindexchanged="DropDownList1_SelectedIndexChanged">
                    <asp:ListItem Text="  Replacement   " Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="   Non Replacement   " Value="0"></asp:ListItem>
                </asp:DropDownList>
            </center>
            </br>

            <asp:HiddenField ID="HiddenFieldListProductNumber" runat="server" />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                EmptyDataText="No Product Attachment" >
                <Columns>
                    <asp:TemplateField HeaderText="ID Product" SortExpression="ID Product">
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true"
                                Enabled="true" Text='<%# Bind("ProductNumber") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ProductName" SortExpression="ProductName" >
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="headerTable" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" SortExpression="ProductDesc">
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("ProductDesc") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="UnitProduct" SortExpression="UnitProduct" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("UnitProduct") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ProductPrice" SortExpression="ProductPrice" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("ProductPrice") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br /><br />
            <asp:Button ID="submit" runat="server" Text="Add" onclick="submit_Click" CssClass="ms-crm-Button" Visible=false/>
            <asp:LinkButton ID="submit1" runat="server" Text="Add" onclick="submit_Click" CssClass="ms-crm-Button" PostBackUrl="#"></asp:LinkButton>
        </div>
    
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
            SelectMethod="GetProducts" TypeName="ProductAttachmentList">
        </asp:ObjectDataSource>
    </form>
</body>
</html>
