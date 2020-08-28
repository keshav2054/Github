<%@ Page Title="Linkdin" Language="C#"  AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LinkdinApi._Default" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"/>
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"/>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap-select@1.13.9/dist/js/bootstrap-select.min.js" type="text/javascript"></script>
     <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js" type="text/javascript"></script>

    <style type="text/css">
       #fname{
           position: relative;
             display: block;
             margin : 0 auto;
             margin-top:10px;
       }
    </style>
</head>
<body>
    <form id="form1" runat="server"   class="text-center" style="margin-top:80px">

 <label for="fname">Add Your UserName: </label>

      
  <input type="text"  runat="server"  class="form-control" id="fname" placeholder="Add your linkdin email here"  style="width:40%"  name="fname" required/><br/><br/>

       
<asp:Button Text="Upload profile" class="btn btn-primary"  runat="server"  OnClick="Authorize" />



<asp:Panel ID="pnlDetails" runat="server" Visible="false">
    <hr />
    <asp:Image ID="imgPicture" runat="server" /><br />
    Name:
    <asp:Label ID="lblName" runat="server" /><br />
    LinkedInId:
    <asp:Label ID="lblLinkedInId" runat="server" /><br />
    Location:
    <asp:Label ID="lblLocation" runat="server" /><br />
    EmailAddress:
    <asp:Label ID="lblEmailAddress" runat="server" /><br />
  
</asp:Panel>
    </form>
</body>
</html>

