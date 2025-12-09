<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="web_programming_project.home" %>

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
                <br />
                <br />
                <asp:Button ID="chart" runat="server" Text="Button" />
            </div>
            <!-- 中間 -->
            <div class="main">
                <!-- 收支結餘顯示 -->
                <div class="top">
                    <!-- 收 -->
                    <div class="top-box">
                        <asp:Label ID="incomeLabel" runat="server" Text="收入"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="income" runat="server" Text="0" ForeColor="#198754"></asp:Label>
                        </div>
                        <br />
                    </div>
                    <!-- 支 -->
                    <div class="top-box">
                        <asp:Label ID="expenseLabel" runat="server" Text="支出"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="expense" runat="server" Text="0" ForeColor="#b80404"></asp:Label>
                        </div>
                    </div>
                    <!-- 結餘 -->
                    <div class="top-box">
                        <asp:Label ID="totalLabel" runat="server" Text="結餘"></asp:Label>
                        <div class="top-text">
                            <asp:Label ID="total" runat="server" Text="0" ForeColor="#0d6efd"></asp:Label>
                        </div>
                        <br />
                    </div>
                </div>
                <!-- 明細顯示 -->
                <hr class="line" />
                <div class="bottom" id="main">
                    <!-- 藉由Repeater顯示所有資料 -->
                    <asp:Repeater ID="DateRepeater" runat="server" OnItemDataBound="DateRepeater_ItemDataBound">
                        <ItemTemplate>
                            <div class="date">
                                <asp:Label ID="DateTitle" runat="server" Text='<%# Eval("Month", "{0:D2}") + "-" + Eval("Day", "{0:D2}") %>' CssClass="date-label"></asp:Label>
                                <br />
                                <asp:Repeater ID="DetailRepeater" runat="server">
                                    <ItemTemplate>
                                        <div class="record">
                                            <div class="category">
                                                <asp:Label ID="Categoryl" runat="server" Text='<%# Eval("category") %>'></asp:Label>
                                            </div>
                                            <div class="description">
                                                <asp:Label ID="Descriptionl" runat="server" Text='<%# Eval("description") %>'></asp:Label>
                                            </div>
                                            <div class="amount">
                                                <asp:Label ID="amountLabel" runat="server" Text='<%# Eval("amount") %>' ForeColor='<%# Eval("color") %>'></asp:Label>
                                            </div>
                                            <div class="button">
                                                <!--<asp:Button ID="edit" runat="server" Text="編輯" CausesValidation="False" />-->
                                                <asp:Button ID="delete" runat="server" Text="刪除" CausesValidation="False" CssClass="deleteButton" CommandArgument='<%# Eval("ID") %>' CommandName="Delete" OnClick="delete_Click" />
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <!-- 右側 -->
            <div class="right">
                <!-- 收支選擇 -->
                <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="chooseType" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged">
                    <asp:ListItem Value="expense" Selected="True" Text="支出"></asp:ListItem>
                    <asp:ListItem Value="income" Text="收入"></asp:ListItem>
                </asp:RadioButtonList>

                <br />
                <!-- 類別選擇 -->
                <div class="chooseCategory">
                    <asp:Label ID="Label5" runat="server" Text="類別"></asp:Label>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_Ca" runat="server" ControlToValidate="chooseCategory" Display="None" ErrorMessage="必須選擇一個類別" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <br />
                    <asp:DropDownList ID="chooseCategory" runat="server" CssClass="DDLChooseCategory"></asp:DropDownList>
                </div>
                <br />

                <!-- 輸入日期、金額、備註 -->
                <div class="form">
                    <asp:Label ID="DATE_" runat="server" Text="日期"></asp:Label>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_Date" runat="server" ControlToValidate="date0" Display="None" EnableTheming="True" ErrorMessage="日期沒有填寫" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <br />
                    <asp:TextBox ID="date0" runat="server" TextMode="Date"></asp:TextBox>
                    <br />
                    <asp:Label ID="AMOUNT_" runat="server" Text="金額"></asp:Label>
                    <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="amount0" Display="Dynamic" ErrorMessage="必須在0-99999999整數" MaximumValue="99999999" MinimumValue="0" SetFocusOnError="True" Type="Integer" EnableTheming="True"></asp:RangeValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_Amount" runat="server" ControlToValidate="amount0" ErrorMessage="金額必填" Display="None"></asp:RequiredFieldValidator>
                    <br />
                    <asp:TextBox ID="amount0" runat="server" TextMode="Number"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label1" runat="server" Text="備註"></asp:Label>
                    <br />
                    <asp:TextBox ID="description0" runat="server"></asp:TextBox>
                </div>
                <br />
                <div class="actionButton">
                    <asp:Button ID="cancel" runat="server" Text="取消" CausesValidation="False" OnClick="cancel_Click" />
                    <asp:Button ID="save" runat="server" Text="儲存" OnClick="save_Click" />
                </div>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </div>

        </div>
    </form>
</body>
</html>
