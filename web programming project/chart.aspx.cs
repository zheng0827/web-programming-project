using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using QuickChart;
using System.Text.Json;

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
        protected SqlDataSource getDetailByMonth(int Month)
        {
            string sql = @"SELECT * FROM [Details] WHERE [Month] = @Month ORDER BY [ID] DESC";

            SqlDataSource1.SelectCommand = sql;

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("Month", System.Data.DbType.Int32, Month.ToString());

            return SqlDataSource1;
        }

        private static List<string> AllCategories = new List<string>
    {
        "早餐", "午餐", "晚餐", "購物", "醫療", "點心",
        "娛樂", "交通", "社交", "數位服務", "薪水",
        "獎金", "禮金", "投資", "其他"
    };
        private (List<string> labels, List<int> data) GetCategoryTotals(int Month)
        {

            Dictionary<string, int> categoryTotals = AllCategories.ToDictionary(key => key, value => 0);


            SqlDataSource monthDetail = getDetailByMonth(Month);

           
            DataView dataView = (DataView)monthDetail.Select(DataSourceSelectArguments.Empty);

       
            foreach (DataRowView rowView in dataView)
            {
                
                string category = rowView["Category"].ToString();
                string type = rowView["Type"].ToString().ToLower(); 

                
                int amount = Convert.ToInt32(rowView["Amount"]);

               
                if (categoryTotals.ContainsKey(category))
                {
                    if (type == "e")
                    {
                        categoryTotals[category] -= amount;
                    }
                    else if (type == "i")
                    {
                        categoryTotals[category] += amount;
                    }
                }
                // 註：如果 Category 不在 AllCategories 清單中，則忽略該筆資料
            }

            // 4. 篩選、排序和結構化數據
            var filteredData = categoryTotals
                .Where(kvp => kvp.Value != 0) // 剔除總結餘為 0 的類別
                .OrderByDescending(kvp => Math.Abs(kvp.Value)) // 按照絕對值降序排列 
                .ToList();

          
            return (
                labels: filteredData.Select(kvp => kvp.Key).ToList(), // 類別名稱 (e.g., "薪水", "午餐")
                data: filteredData.Select(kvp => kvp.Value).ToList() // 對應的總結餘 (e.g., 5000, -300)
            );
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
       

        protected List<List<int>> handleMonthLine(int Month)
        {
            var date = Enumerable.Range(1, 31).ToList();
            var balance = Enumerable.Repeat(0, 31).ToList();

            SqlDataSource monthDetail = getDetailByMonth(Month);
            DataView dataView = (DataView)monthDetail.Select(DataSourceSelectArguments.Empty);

            foreach (DataRowView rowView in dataView)
            {
                int day = Convert.ToInt32(rowView["day"]);
                int inx = day - 1;
                if (inx < 0 || inx >= balance.Count) continue; // 驗證 day 範圍或記錄錯誤
                int amount = Convert.ToInt32(rowView["amount"]);
                switch (rowView["type"].ToString())
                {
                    case "e":
                        balance[inx] -= amount;
                        break;
                    case "i":
                        balance[inx] += amount;
                        break;
                }
            }

            var filtered = date.Zip(balance, (d, b) => new { d, b })
                               .Where(x => x.b != 0)
                               .ToList();

            var newLabels = filtered.Select(x => x.d).ToList();
            var newBalances = filtered.Select(x => x.b).ToList();
            return new List<List<int>> { newLabels, newBalances };
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
                Labels = months,
                Data = data
            };

            return dataForChart; // ASP.NET 會自動將它序列化為 JSON
        }

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


            List<List<int>> qwq1 = handleMonthLine(currentMonth);
            List<int> date = qwq1[0];
            List<int> balance1 = qwq1[1];
            string datelabelsJson = JsonSerializer.Serialize(date.Select(x => x.ToString()));
            string datedataJson = JsonSerializer.Serialize(balance1);
            // 2. 注意這裡多了 '$' 符號，變成 $@"..."
            Chart qc1 = new Chart();
            qc1.Width = 500;
            qc1.Height = 300;
            qc1.Version = "2.9.4";
            qc1.Config = $@"{{
                    type: 'line',
                    data: {{
                        labels: {datelabelsJson},
                        datasets: [{{
                            label: '本月結餘',
                            data: {datedataJson},
                            fill: false,
                        }}]
                    }}
                }}";
            qc1.BackgroundColor = "#1e293b";

            balance_chart.ImageUrl = qc1.GetUrl();
            List<List<int>> qwq = handleYearLine(currentYear);
            List<int> month = qwq[0];
            List<int> balance = qwq[1];

            var result = GetCategoryTotals(currentMonth);



            string labelsJson = JsonSerializer.Serialize(month.Select(x => x.ToString()));
            string dataJson = JsonSerializer.Serialize(balance);

            string categoryLabelsJson = JsonSerializer.Serialize(result.labels);
            string categoryDataJson = JsonSerializer.Serialize(result.data);

            Chart qc2 = new Chart();
            qc2.Width = 500;
            qc2.Height = 300;
            qc2.Version = "2.9.4";

            // 2. 注意這裡多了 '$' 符號，變成 $@"..."
            qc2.Config = $@"{{
                    type: 'bar',
                    data: {{
                        labels: {categoryLabelsJson},
                        datasets: [{{
                            label: '支出',
                            data: {categoryDataJson},
                            fill: false,
                        }}]
                    }}
                }}";
            qc2.BackgroundColor = "#1e293b";

            expense_chart.ImageUrl = qc2.GetUrl();





            Chart qc = new Chart();
            qc.Width = 500;
            qc.Height = 300;
            qc.Version = "2.9.4";

            // 2. 注意這裡多了 '$' 符號，變成 $@"..."
            qc.Config = $@"{{
                    type: 'line',
                    data: {{
                        labels: {labelsJson},
                        datasets: [{{
                            label: '月結餘',
                            data: {dataJson}
                        }}]
                    }}
                }}";
            qc.BackgroundColor = "#1e293b";
            year_line_chart.ImageUrl = qc.GetUrl();
        }



        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                
                int currentYear = DateTime.Now.Year; // 取得目前年份
                int currentMonth = DateTime.Now.Month; // 取得目前月份
                setCurrentYearMonth(currentYear, currentMonth);// 設定隱藏欄位的值

                monthTitle.Text = currentYear + " 年 " + currentMonth + " 月 記帳本"; // 設定標題
                yearLabel.Text = currentYear.ToString(); // 設定年份標籤
                RBLChooseMonth.SelectedIndex = currentMonth - 1; // 設定選擇的月份

               
            }
        }
    }
}