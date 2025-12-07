using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace web_programming_project
{
    public partial class home : System.Web.UI.Page
    {

        /* ================================== 變數宣告 ================================== */
        private Dictionary<string, string> category = new Dictionary<string, string>();
        private List<string> categoryList = new List<string> {
            "早餐","午餐","晚餐","購物","醫療","點心","娛樂","交通","社交","數位服務",
            "薪水","獎金","禮金","投資","其他"
        };
        private List<string> categoryListE = new List<string> {
            "早餐","午餐","晚餐","購物","醫療","點心","娛樂","交通","社交","數位服務"
        };
        private List<string> categoryListI = new List<string> {
            "薪水","獎金","禮金","投資","其他"
        };
        
        /* ========= 透過隱藏項 currentYearMonth，取得/設置目前選擇的年份與月份 ========= */

        // 回傳格式: List<int> { year, month }
        protected List<int> getCurrentYearMonth()
        {
            int year;
            int month;
            string curren = currentYearMonth.Value.ToString();
            year = int.Parse(curren.Split(' ')[0]);
            month = int.Parse(curren.Split(' ')[1]);
            return new List<int> { year, month };
        }

        // 設定年份與月份到隱藏項 currentYearMonth 中，格式: "1970 1"
        protected void setCurrentYearMonth(int year = 1970, int month = 1)
        {
            currentYearMonth.Value = year.ToString() + " " + month.ToString();
        }

        /* ================================== 按鈕事件 ================================== */

        // 前一年按鈕事件
        protected void prevYear_Click(object sender, EventArgs eq)
        {
            int currentYear = getCurrentYearMonth()[0];
            int currentMonth = getCurrentYearMonth()[1];
            int selectedYear = int.Parse(yearLabel.Text);

            RBLChooseMonth.ClearSelection(); // 清除月份選擇
            if (DateTime.Now.Year - selectedYear < 10) yearLabel.Text = (selectedYear - 1).ToString(); // 設定年份標籤，但有前10年內的限制
            if (int.Parse(yearLabel.Text) == currentYear) RBLChooseMonth.SelectedIndex = currentMonth - 1; // 回到當前年份時，設定月份選擇
        }

        // 後一年按鈕事件
        protected void nextYear_Click(object sender, EventArgs eq)
        {
            int currentYear = getCurrentYearMonth()[0];
            int currentMonth = getCurrentYearMonth()[1];
            int selectedYear = int.Parse(yearLabel.Text);

            RBLChooseMonth.ClearSelection(); // 清除月份選擇
            if (selectedYear - DateTime.Now.Year < 10) yearLabel.Text = (selectedYear + 1).ToString(); // 設定年份標籤，但有後10年內的限制
            if (int.Parse(yearLabel.Text) == currentYear) RBLChooseMonth.SelectedIndex = currentMonth - 1; // 回到當前年份時，設定月份選擇
        }
        // 取消按鈕事件
        protected void cancel_Click(object sender, EventArgs e)
        {
            RadioButtonList1.SelectedIndex = 0;// 重設收支類型選擇
            chooseCategory.SelectedIndex = -1;// 重設類別選擇
            date0.Text = "";// 重設日期輸入欄位
            amount0.Text = "";// 重設金額輸入欄位
            description0.Text = "";// 重設備註輸入欄位
            BindCategory(categoryList);
        }

        // 儲存按鈕事件
        protected void save_Click(object sender, EventArgs e)
        {
            if (!IsValid) return;
            string type = (RadioButtonList1.SelectedIndex == 0) ? "e" : "i";
            string categoryValue = categoryList[chooseCategory.SelectedIndex];
            string dateStr = date0.Text;
            string description = description0.Text;
            int amount = int.Parse(amount0.Text);
            int year = int.Parse(dateStr.Split('-')[0]);
            int month = int.Parse(dateStr.Split('-')[1]);
            int day = int.Parse(dateStr.Split('-')[2]);
            string id = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            string sql = @"INSERT INTO [Details] ([Year], [Month], [Day], [Type], [Category], [Description], [Amount], [ID]) VALUES (@Year, @Month, @Day,@type, @category, @description, @amount, @id)";
            SqlDataSource1.InsertCommand = sql;

            SqlDataSource1.InsertParameters.Clear();
            SqlDataSource1.InsertParameters.Add("Year", System.Data.DbType.Int16, year.ToString());
            SqlDataSource1.InsertParameters.Add("Month", System.Data.DbType.Int16, month.ToString());
            SqlDataSource1.InsertParameters.Add("Day", System.Data.DbType.Int16, day.ToString());
            SqlDataSource1.InsertParameters.Add("type", System.Data.DbType.String, type);
            SqlDataSource1.InsertParameters.Add("category", System.Data.DbType.String, categoryValue);
            SqlDataSource1.InsertParameters.Add("description", System.Data.DbType.String, description);
            SqlDataSource1.InsertParameters.Add("amount", System.Data.DbType.Int32, amount.ToString());
            SqlDataSource1.InsertParameters.Add("id", System.Data.DbType.String, id);
            SqlDataSource1.Insert();
            // 重新綁定資料
            BindDetailData();
            BindSummaryData();
            // 重設輸入欄位
            RadioButtonList1.SelectedIndex = 0;
            chooseCategory.SelectedIndex = -1;
            date0.Text = "";
            amount0.Text = "";
            description0.Text = "";
            Response.Redirect("home.aspx"); // 重新整理頁面
        }



        // 選擇月份 RadioButton 事件
        protected void RBLChooseMonth_SelectedIndexChanged(object sender, EventArgs eq)
        {
            int currentYear = getCurrentYearMonth()[0];
            int currentMonth = getCurrentYearMonth()[1];

            if (RBLChooseMonth.SelectedIndex == -1) return; // 沒有選擇月份
            if (int.Parse(yearLabel.Text) == currentYear && RBLChooseMonth.SelectedIndex + 1 == currentMonth) return; // 選擇當前年月

            currentYear = int.Parse(yearLabel.Text); // 設置目前選擇的年份
            currentMonth = RBLChooseMonth.SelectedIndex + 1; // 設置目前選擇的月份
            setCurrentYearMonth(currentYear, currentMonth); // 設定隱藏欄位的值

            monthTitle.Text = currentYear + " 年 " + currentMonth + " 月 記帳本"; // 設定標題
            BindSummaryData();
            BindDetailData(); // 綁定明細資料
        }

        // 
        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedIndex == 0) BindCategory(categoryList);
            else BindCategory(categoryList);

            chooseCategory.SelectedIndex = -1;// 重設類別選擇
        }

        // 大 Repeater 的 ItemDataBound 事件，用來綁定內層的明細 Repeater
        protected void DateRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            int year = getCurrentYearMonth()[0];
            int month = getCurrentYearMonth()[1];

            // 找到內層的 Repeater
            Repeater innerRepeater = (Repeater)e.Item.FindControl("DetailRepeater");
            // 找到日期物件
            int day = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "Day"));

            // 取得該日期的所有明細資料
            SqlDataSource detailData = getDetailByDay(year, month, day);
            // 將明細資料轉成C#可處理的資料檢視
            DataView detailDataView = (DataView)detailData.Select(DataSourceSelectArguments.Empty);

            List<object> detailList = new List<object>();
            // 逐筆處理明細資料
            foreach (DataRowView rowView in detailDataView)
            {
                string category = rowView["category"].ToString();
                string type = rowView["type"].ToString();
                string description = rowView["description"].ToString();
                string id = rowView["id"].ToString();
                int amount = Convert.ToInt32(rowView["amount"]);
                string colorStr;
                // 根據收支類型設定顏色
                switch (type)
                {
                    case "e":
                        colorStr = "#b80404";  // 紅
                        break;
                    case "i":
                        colorStr = "#198754";  // 綠
                        break;
                    default:
                        colorStr = "#ffffff";  // 黑
                        break;
                }

                Color color = ColorTranslator.FromHtml(colorStr);
                // 將處理後的明細資料加入清單
                detailList.Add(new
                {
                    category,
                    description,
                    amount,
                    color,
                    id
                }
                );
            }

            innerRepeater.DataSource = detailList.ToArray();
            innerRepeater.DataBind();
        }

        /* ================================== 獲取資料 ================================== */

        // 取得某年某月的所有日期
        protected SqlDataSource getDate(int Year, int Month)
        {
            string sql = @"SELECT DISTINCT [Month], [Day] FROM [Details] WHERE [Year] = @Year AND [Month] = @Month ORDER BY [Day] DESC";

            SqlDataSource1.SelectCommand = sql;

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("Year", System.Data.DbType.Int32, Year.ToString());
            SqlDataSource1.SelectParameters.Add("Month", System.Data.DbType.Int32, Month.ToString());

            return SqlDataSource1;
        }
        
        // 取得某年某月的所有明細
        protected SqlDataSource getDetail(int Year, int Month)
        {
            string sql = @"SELECT * FROM [Details] WHERE [Year] = @Year AND [Month] = @Month";

            SqlDataSource1.SelectCommand = sql;

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("Year", System.Data.DbType.Int32, Year.ToString());
            SqlDataSource1.SelectParameters.Add("Month", System.Data.DbType.Int32, Month.ToString());

            return SqlDataSource1;
        }

        // 取得某年某月某日的所有明細
        protected SqlDataSource getDetailByDay(int Year, int Month, int Day)
        {
            string sql = @"SELECT * FROM [Details] WHERE [Year] = @Year AND [Month] = @Month AND [Day] = @Day ORDER BY [ID] DESC";

            SqlDataSource1.SelectCommand = sql;

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("Year", System.Data.DbType.Int32, Year.ToString());
            SqlDataSource1.SelectParameters.Add("Month", System.Data.DbType.Int32, Month.ToString());
            SqlDataSource1.SelectParameters.Add("Day", System.Data.DbType.Int32, Day.ToString());

            return SqlDataSource1;
        }

        /* ============================ 頁面載入與綁定資料 ============================ */
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)//第一次進入頁面
            {
                int currentYear = DateTime.Now.Year; // 取得目前年份
                int currentMonth = DateTime.Now.Month; // 取得目前月份
                setCurrentYearMonth(currentYear, currentMonth); // 設定隱藏欄位的值

                monthTitle.Text = currentYear + " 年 " + currentMonth + " 月 記帳本"; // 設定標題
                yearLabel.Text = currentYear.ToString(); // 設定年份標籤
                RBLChooseMonth.SelectedIndex = currentMonth - 1; // 設定選擇的月份

                BindDetailData();
                BindSummaryData();
                BindCategory(categoryList);
            }
        }
        // 綁定日期到大Repeater
        protected void BindDetailData()
        {
            int year = getCurrentYearMonth()[0];
            int month = getCurrentYearMonth()[1];

            SqlDataSource dateData = getDate(year, month);

            DateRepeater.DataSource = dateData;
            DateRepeater.DataBind();
        }

        // 綁定類別到下拉選單
        protected void BindCategory(List<string> categoryList)
        {
            category.Clear();
            // 綁定類別下拉選單
            foreach (string cat in categoryList)
            {
                category.Add(cat, cat);
            }
            chooseCategory.DataSource = category;
            chooseCategory.DataTextField = "Key";
            chooseCategory.DataValueField = "Value";

            chooseCategory.DataBind();

        }
        
        protected void BindSummaryData()
        {
            int year = getCurrentYearMonth()[0];
            int month = getCurrentYearMonth()[1];

            SqlDataSource detailData = getDetail(year, month);
            DataView detailDataView = (DataView)detailData.Select(DataSourceSelectArguments.Empty);
            
            int totalExpense = 0;
            int totalIncome = 0;

            foreach (DataRowView rowView in detailDataView)
            {
                string type = rowView["type"].ToString();
                int amount = Convert.ToInt32(rowView["amount"]);
                
                if (type == "e")
                {
                    totalExpense += amount;
                }
                else if (type == "i")
                {
                    totalIncome += amount;
                }
            }
            
            expense.Text = "-$" + totalExpense.ToString();
            income.Text = "+$" + totalIncome.ToString();
            total.Text = "$" + (totalIncome - totalExpense).ToString();
        }
    }
}