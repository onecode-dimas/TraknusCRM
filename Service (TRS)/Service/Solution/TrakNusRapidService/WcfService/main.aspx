<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="TrakNusRapidServiceWcfService.main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        File&nbsp;Name&nbsp;:&nbsp;
        <asp:DropDownList ID="ddlFileName" runat="server" OnSelectedIndexChanged="ddlFileName_SelectedIndexChanged">
        </asp:DropDownList>
    
    &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnShow" runat="server" OnClick="btnShow_Click" Text="Show" />
        <br />
        <br />
        <asp:Label ID="lblContent" runat="server"></asp:Label>
    
    </div>
    </form>
</body>
</html>
