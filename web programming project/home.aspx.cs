using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True";
        //private List<Dictionary<string, string>> category = new List<Dictionary<string, string>>();
        private Dictionary<string, string> category = new Dictionary<string, string>();
        // 透過隱藏項 currentYearMonth，取得目前選擇的年份與月份
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
            BindDetailData(); // 綁定明細資料
        }

        protected void BindDetailData()
        {
            int year = getCurrentYearMonth()[0];
            int month = getCurrentYearMonth()[1];

            SqlDataSource dateData = getDate(year, month);

            DateRepeater.DataSource = dateData;
            DateRepeater.DataBind();
        }
        protected SqlDataSource getDate(int Year, int Month)
        {
            string sql = @"SELECT DISTINCT [Month], [Day] FROM [Details] WHERE [Year] = @Year AND [Month] = @Month ORDER BY [Day] DESC";

            SqlDataSource1.SelectCommand = sql;

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("Year", System.Data.DbType.Int32, Year.ToString());
            SqlDataSource1.SelectParameters.Add("Month", System.Data.DbType.Int32, Month.ToString());

            return SqlDataSource1;
        }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            category.Add("c1", "早餐");
            category.Add("c2", "午餐");
            category.Add("c3", "晚餐");
            category.Add("c4", "購物");
            category.Add("c5", "醫療");
            category.Add("c6", "點心");
            category.Add("c7", "娛樂");
            category.Add("c8", "交通");
            category.Add("c9", "社交");
            category.Add("c10", "數位服務");

            chooseCategory.DataSource = category;
            chooseCategory.DataTextField = "Value";
            chooseCategory.DataValueField = "Key";
            chooseCategory.DataBind();
            if (!IsPostBack)//第一次進入頁面
            {
                int currentYear = DateTime.Now.Year; // 取得目前年份
                int currentMonth = DateTime.Now.Month; // 取得目前月份
                setCurrentYearMonth(currentYear, currentMonth); // 設定隱藏欄位的值

                monthTitle.Text = currentYear + " 年 " + currentMonth + " 月 記帳本"; // 設定標題
                yearLabel.Text = currentYear.ToString(); // 設定年份標籤
                RBLChooseMonth.SelectedIndex = currentMonth - 1; // 設定選擇的月份

                BindDetailData();
            }
        }
        protected void DateRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            int year = getCurrentYearMonth()[0];
            int month = getCurrentYearMonth()[1];

            // 找到內層的 Repeater
            Repeater innerRepeater = (Repeater)e.Item.FindControl("DetailRepeater");
            // 找到日期物件
            //int day = int.Parse(e.Item.F);
            SqlDataSource detailData = getDetailByDay(year, month,1);
            // 模擬資料庫
            //color #198754 green 收
            //color #ff0000 red 支
            Color Red = ColorTranslator.FromHtml("#ff0000");
            Color Green = ColorTranslator.FromHtml("#198754");
            var detailData111 = new[] {
                new {category = "飲食", description = "午餐便當", amount = 100, color = Red},
                new { category = "交通", description = "捷運儲值", amount = 500 , color = Green},
                new { category = "交通", description = "儲值", amount = 500 , color = Red},
                new { category = "娛樂", description = "看電影", amount = 300 , color = Green}
            };
            innerRepeater.DataSource = detailData111;
            innerRepeater.DataBind();
        }

        protected void chooseMonth_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            RadioButtonList1.SelectedIndex = 0;
            chooseCategory.SelectedIndex = -1;
            date0.Text = "";
            amount0.Text = "";
            description0.Text = "";
        }

        protected void save_Click(object sender, EventArgs e)
        {

        }

    }
}