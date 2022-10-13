<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuoteEmailPDF.aspx.cs" Inherits="PortalCRMTraknus.QuoteEmailPDF" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .success
        {
            font-weight:bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <center>
            <table>
                <tr>
                    <td style="text-align: left">
                        Customer Name
                    </td>
                    <td>
                        :
                    </td>
                    <td>
                        <asp:TextBox ID="CustomerTextBox" runat="server" style="text-align: left">
                        </asp:TextBox>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td style="text-align: left">
                        Email To
                    </td>
                    <td>
                        :
                    </td>
                    <td>
                        <asp:TextBox ID="EmailToTextBox" runat="server" style="text-align: left">
                        </asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqEmail"  runat="server" ControlToValidate="EmailToTextBox" 
                          ErrorMessage="Field Email" CssClass="form-required" Display="Dynamic"><span class="required-field">Required</span></asp:RequiredFieldValidator>
                        <%--<asp:RegularExpressionValidator ID="RegExEmail" runat="server" 
                          ErrorMessage="Field Email" ControlToValidate="EmailToTextBox" 
                          ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\;*" 
                          Display="Dynamic"><span class="required-field">Your email not valid</span></asp:RegularExpressionValidator>--%>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td style="text-align: right">
                        <asp:Button ID="SendButton" Text="Send Email" ToolTip="Send Email" 
                            runat="server" onclick="SendButton_Click"/>
                    </td>
                    <td></td>
                </tr>
            </table>
            
        </center>
    </div>
    <div>
        <center><asp:Label ID="success_message" runat="server" CssClass=success></asp:Label></center>
    </div>
    </form>
</body>
</html>
