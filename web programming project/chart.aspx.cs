using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;


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
        protected SqlDataSource getDetailByYear(int Year)
        {
            string sql = @"SELECT * FROM [Details] WHERE [Year] = @Year ORDER BY [ID] DESC";

            SqlDataSource1.SelectCommand = sql;

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("Year", System.Data.DbType.Int32, Year.ToString());

            return SqlDataSource1;
        }

        protected List<List<int>> handleYearLine(int Year)
        {
            // 定義List
            List<int> month = new List<int> {1,2,3,4,5,6,7,8,9,10,11,12};
            List<int> balance = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // 獲取年度資料
            SqlDataSource yearDetail = getDetailByYear(Year);
            // 將 SqlDataSource 轉換成 DataView
            DataView dataView = (DataView)yearDetail.Select(DataSourceSelectArguments.Empty);

            foreach (DataRowView rowView in dataView)
            {
                // 月份 - 1 = 在list中的索引值
                int inx = Convert.ToInt32(rowView["month"]) - 1;
                int amount = Convert.ToInt32(rowView["amount"]);
                // 根據類別加或減取得結餘
                switch (rowView["type"].ToString())
                {
                    case "e":
                        balance[inx] -= amount;
                        break;
                    case "i":
                        balance[inx] += amount;
                        break;
                    default:
                        balance[inx] += 0;
                        break;
                }

            }
            return new List<List<int>> {month,balance};
        }

        [WebMethod]
        public static object GetYearLineChartData()
        {   
            // 建立 chart 實例以呼叫非靜態方法
            chart page = (chart)HttpContext.Current.Handler;
            int currentYear = page.getCurrentYearMonth()[0];
            List<int> months = page.handleYearLine(currentYear)[0];
            List<int> data = page.handleYearLine(currentYear)[1];
            // 從資料庫或其他來源獲取數據
            var dataForChart = new
            {
                labels = months.ToArray(),
                data = data.ToArray()
            };

            return dataForChart; // ASP.NET 會自動將它序列化為 JSON
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int currentYear = DateTime.Now.Year; // 取得目前年份
                int currentMonth = DateTime.Now.Month; // 取得目前月份
                setCurrentYearMonth(currentYear, currentMonth); // 設定隱藏欄位的值

                monthTitle.Text = currentYear + " 年 " + currentMonth + " 月 記帳本"; // 設定標題
                yearLabel.Text = currentYear.ToString(); // 設定年份標籤
                RBLChooseMonth.SelectedIndex = currentMonth - 1; // 設定選擇的月份

                List<List<int>> qwq = handleYearLine(currentYear);
                // qwq[0] = month List => {1,2,3,...,12}
                // qwq[1] = balance List => {...}
            }
        }
    }
}