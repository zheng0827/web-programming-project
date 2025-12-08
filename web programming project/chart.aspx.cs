using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace web_programming_project
{
    public partial class chart : System.Web.UI.Page
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}