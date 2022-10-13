<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequestAccountNumberNPWP.aspx.cs" Inherits="PortalCRMTraknus.RequestAccountNumberNPWP" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Request Account Number & NPWP</title>
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
            width: 500px;
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
    <div>
    
    </div>
    </form>
</body>
</html>
