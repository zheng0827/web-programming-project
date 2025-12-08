<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="chart.aspx.cs" Inherits="web_programming_project.chart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="Style.css" />
</head>
<body>
    <form id="form1" runat="server" submitdisabledcontrols="False">
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True" ProviderName="System.Data.SqlClient" SelectCommand="SELECT * FROM [Details]"></asp:SqlDataSource>
        <!-- 標題 -->
        <div class="title">
            <asp:Label ID="monthTitle" runat="server" Text="monthTitle"></asp:Label>
            <asp:HiddenField ID="currentYearMonth" runat="server" Value="1970 1"/>
        </div>
        <!-- 主內容-->
        <div class="content">
            <!-- 左側 -->
            <div class="sidebar">
                <!--<asp:DropDownList ID="chooseMonth" runat="server" CssClass="chooseMonth">
                </asp:DropDownList> -->

                <div class="changeYear">
                    <asp:Button ID="prevYear" runat="server" Text="&lt;" CssClass="yearButton" CausesValidation="False" OnClick="prevYear_Click" />
                    <asp:Label ID="yearLabel" runat="server" Text="2025" CssClass="yearLabel"></asp:Label>
                    <asp:Button ID="nextYear" runat="server" Text="&gt;" CssClass="yearButton" CausesValidation="False" OnClick="nextYear_Click" />
                </div>
                <div class="chooseMonth2">
                    <asp:RadioButtonList ID="RBLChooseMonth" runat="server" RepeatDirection="Horizontal"
                        CssClass="RBLChooseMonth" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="RBLChooseMonth_SelectedIndexChanged">
                        <asp:ListItem Value="1" Text="1"></asp:ListItem>
                        <asp:ListItem Value="2" Text="2"></asp:ListItem>
                        <asp:ListItem Value="3" Text="3"></asp:ListItem>
                        <asp:ListItem Value="4" Text="4"></asp:ListItem>
                        <asp:ListItem Value="5" Text="5"></asp:ListItem>
                        <asp:ListItem Value="6" Text="6"></asp:ListItem>
                        <asp:ListItem Value="7" Text="7"></asp:ListItem>
                        <asp:ListItem Value="8" Text="8"></asp:ListItem>
                        <asp:ListItem Value="9" Text="9"></asp:ListItem>
                        <asp:ListItem Value="10" Text="10"></asp:ListItem>
                        <asp:ListItem Value="11" Text="11"></asp:ListItem>
                        <asp:ListItem Value="12" Text="12"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>
            <!-- 主要頁面 -->
            <div class="main">
                <asp:Label ID="Label1" runat="server" Text="年度總攬"></asp:Label>
                <br />
                <asp:Image ID="year_line_chart" runat="server" />
                <br />
                <div class="month">
                    <asp:Label ID="Label2" runat="server" Text="月結餘"></asp:Label>
                    <br />
                    <asp:Image ID="balance_chart" runat="server" />
                    <br />

                    <asp:Label ID="Label3" runat="server" Text="類別支出長條圖"></asp:Label>
                    <br />
                    <asp:Image ID="expense_chart" runat="server" />
                    <br />

                    <asp:Label ID="Label4" runat="server" Text="日支出趨勢"></asp:Label>
                    <br />
                    <asp:Image ID="daliy_expense_chart" runat="server" />
                    <br />

                </div>
                
                
                
            </div>

        </div>
    </form>
</body>
</html>
