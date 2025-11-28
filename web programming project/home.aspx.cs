using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace web_programming_project
{
    public partial class home : System.Web.UI.Page
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True";

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
        // 選擇月份RadioButton事件
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
        }
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
                testBindData();
            }
        }

        private void testBindData()
        {
            var dateList = new[]
            {
            new { date = DateTime.Now.AddDays(-2) },
            new { date = DateTime.Now.AddDays(-1) },
            new { date = DateTime.Now }
        };

            DateRepeater.DataSource = dateList;
            DateRepeater.DataBind();
        }
        protected void DateRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // 找到內層的 Repeater
            Repeater innerRepeater = (Repeater)e.Item.FindControl("DetailRepeater");
            // 找到日期物件
            object dateObj = DataBinder.Eval(e.Item.DataItem, "date");
            DateTime currentDate = Convert.ToDateTime(dateObj);
            // 模擬資料庫
            //color #198754 green 收
            //color #ff0000 red 支
            Color Red = ColorTranslator.FromHtml("#ff0000");
            Color Green = ColorTranslator.FromHtml("#198754");
            var detailData = new[] {
                new {category = "飲食", description = "午餐便當", amount = 100, color = Red},
                new { category = "交通", description = "捷運儲值", amount = 500 , color = Green},
                new { category = "交通", description = "儲值", amount = 500 , color = Red},
                new { category = "娛樂", description = "看電影", amount = 300 , color = Green}
            };
            innerRepeater.DataSource = detailData;
            innerRepeater.DataBind();
        }

        protected void chooseMonth_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cancel_Click(object sender, EventArgs e)
        {

        }

        protected void save_Click(object sender, EventArgs e)
        {

        }

    }
}