<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PortalAddProductRetentionCPO.aspx.cs" Inherits="PortalCRMTraknus.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Product Retention CPO</title>
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
            color:#eee;
            font-weight:bold;
            background-color:#a00;
            padding: 10px;
        }
        #ErrorMessage
        {
            font-size: 12px;
            color: #000;
        }
        .hr-crm-Dialog-Footer
        {
            border-top-color: #A4ABB2;
        }
        .ms-crm-Dialog-Footer
        {
            text-align: right;
            height: 30px;
            padding: 10px;
            border-top-style: solid;
            border-top-width: 1px;
            border-top-color: #A4ABB2;
        }

        #container1
        {
            width: 800px;
            height: 300px;
            margin:0 auto;
        }
        .ms-crm-Button
        {
            border: 1px solid #333;
            line-height: 18px;
            height: 20px;
            width: 84px;
            display:block;
            text-align: center;
            cursor: pointer;
            color:#333;
            background-repeat: repeat-x;
            background-image: url('ButtonNormalGradient.png');
            padding : 0px 5px 2px 5px;
            font-size:11px;
            font-family: Segoe UI, Tahoma, Arial;
            text-decoration: none;
            overflow:hidden;
            margin-left: auto;
            margin-right: auto;
            margin-bottom: 0;
        }
        .ms-crm-Button_recal
        {
            border: 1px solid #333;
            line-height: 18px;
            height: 20px;
            width: 84px;
            text-align: center;
            cursor: pointer;
            color:#333;
            background-repeat: repeat-x;
            background-image: url('img/ButtonNormalGradient.png');
            padding : 0px 5px 2px 5px;
            font-size:11px;
            font-family: Segoe UI, Tahoma, Arial;
            text-decoration: none;
            overflow:hidden;
        }
        a:hover
        {
            background-image: url(img/ButtonSelectedGradient.png);
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
       <%-- <div style="display:block;width-max:600px;height:auto;clear:both">
            <div style="display:block; float:left; width:50px;height:auto; border:1px solid #000"">
                <img src="img/err_48_4.gif" />
            </div>
            <div style="display:block; float:left; width-max:500px;height:auto; border:1px solid #000">
                <div class="ms-crm-Error-Header">
                    ms-crm-Error-Header
                </div>
                <div id="ErrorMessage" style="width-max:400px;height:auto; border:1px solid #000; overflow:auto">
                    ErrorMessage1 ErrorMessage2 ErrorMessage3 ErrorMessage4 ErrorMessage5 ErrorMessage6 ErrorMessage7 ErrorMessage8 ErrorMessage9 ErrorMessage0 ErrorMessageA1 ErrorMessageA2 ErrorMessageA3 ErrorMessageA4 ErrorMessageA5 ErrorMessageA6 ErrorMessageA7 ErrorMessageA8 ErrorMessageA9 ErrorMessageA0 ErrorMessageB1 ErrorMessageB2 ErrorMessageB3 ErrorMessageB4 ErrorMessageB5 ErrorMessageB6 ErrorMessageB7 ErrorMessageB8 ErrorMessageB9 ErrorMessageB0
                </div>
            </div>
        </div>
        <div class="ms-crm-Dialog-Footer" style="display:block;height:auto; clear:both;">
            <asp:LinkButton ID="LinkButton12" runat="server" Text="Recalculate" CssClass="ms-crm-Button_recal" PostBackUrl="#"></asp:LinkButton>
        </div>--%>
        
        <div id="container2" runat="server">
            <%--<asp:RangeValidator ID="RangeValidator1" runat="server" 
                    ErrorMessage="Value minimum 0 and maximum 100" 
                    ControlToValidate="PercentageRetentionTextBox" ForeColor="Red" 
                    MaximumValue="100" MinimumValue="0"></asp:RangeValidator>
                
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ErrorMessage="Can't null" ControlToValidate="PercentageRetentionTextBox" 
                    ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:LinkButton ID="LinkButton1" runat="server" Text="Ok" onclick="submitError_Click" CssClass="ms-crm-Button" PostBackUrl="#"></asp:LinkButton>
                --%>
        </div>
        <div id="container1" runat="server">
            <h3>Add Product Retention CPO</h3>
            <div>
                <asp:Label ID="PercentageRetentionLabel" runat="server" Text="Percentage Retention "></asp:Label>
                <asp:TextBox ID="PercentageRetentionTextBox" runat="server" ></asp:TextBox>
                <asp:LinkButton ID="RecalculateLinkButton" runat="server" Text="Recalculate" OnClick="RecalculateLinkButton_Click" CssClass="ms-crm-Button_recal" PostBackUrl="#"></asp:LinkButton>
                <asp:LinkButton ID="submit1" runat="server" Text="Save" onclick="submit_Click" CssClass="ms-crm-Button_recal" PostBackUrl="#"></asp:LinkButton>
            </div>
            <br />
            <asp:HiddenField ID="HiddenFieldListProductNumber" runat="server" />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                EmptyDataText="No CPO Product" >
                <Columns>
                    <asp:TemplateField HeaderText="ID" SortExpression="ID">
                        <ItemTemplate>
                            <asp:Label ID="IDLabel" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item Number" SortExpression="ItemNumber">
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true"
                                Enabled="true" Text='<%# Bind("ItemNumber") %>' OnCheckedChanged="CheckBox1_Click" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Product Number" SortExpression="ProductNumberr" >
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("ProductNumber") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="headerTable" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ProductName" SortExpression="ProductName" >
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="headerTable" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="PricePerUnit" SortExpression="PricePerUnit" >
                        <ItemTemplate>
                            <asp:Label ID="PricePerUnitLabel" runat="server" Text='<%# Bind("PricePerUnit") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Discount" SortExpression="Discount">
                        <ItemTemplate>
                            <asp:Label ID="DiscountLabel" runat="server" Text='<%# Bind("Discount") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Retention" SortExpression="Retention">
                        <ItemTemplate>
                            <%--<asp:Label ID="Label5" runat="server" Text='<%# Bind("Retention") %>'></asp:Label>--%>
                            <asp:TextBox ID="TextBoxRetention" runat="server" Text='<%# Bind("Retention") %>' ></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TotalPrice" SortExpression="TotalPrice">
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotalPrice") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" SortExpression="ProductDesc">
                        <ItemTemplate>
                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("ProductDesc") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
            SelectMethod="GetProducts" TypeName="ProductAttachmentList">
        </asp:ObjectDataSource>
    </form>
</body>
</html>
