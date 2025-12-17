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
using System.Drawing;
using WebGrease;

namespace web_programming_project
{

    public partial class chart : System.Web.UI.Page
    {
        private List<string> AllCategories = new List<string> {
            "早餐","午餐","晚餐","購物","醫療","點心","娛樂","交通","社交","數位服務"
        };
        private List<string> chart_type = new List<string> {
            "year_line", "balance_doughnut", "expense_pie", "daily_expense"
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
            handleChart(currentYear, currentMonth);
        }

        // 回到主畫面
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("home.aspx");
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

        protected SqlDataSource getDetailByMonthAndYear(int Year, int Month)
        {

            string sql = @"SELECT * FROM [Details] WHERE [Year] = @Year AND [Month] = @Month ORDER BY [ID] DESC";

            SqlDataSource1.SelectCommand = sql;
            SqlDataSource1.SelectParameters.Clear();

            SqlDataSource1.SelectParameters.Add("Year", System.Data.DbType.Int32, Year.ToString());
            SqlDataSource1.SelectParameters.Add("Month", System.Data.DbType.Int32, Month.ToString());

            return SqlDataSource1;
        }

        /* ================================== 處理資料 ================================== */

        // 年度總覽資料。回傳{月份Label}、{月份結餘}
        protected List<List<int>> handleYearLine(int Year)
        {
            // 定義List
            List<int> month = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
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
            return new List<List<int>> { month, balance };
        }

        // 月份結餘資料。回傳{當月支出、當月收入、當月結餘}
        protected List<int> handleBalanceDoughnut(int Year, int Month)
        {
            int expense = 0;
            int income = 0;
            int balance = 0;
            SqlDataSource monthDetail = getDetailByMonthAndYear(Year, Month);
            DataView dataView = (DataView)monthDetail.Select(DataSourceSelectArguments.Empty);

            foreach (DataRowView rowView in dataView)
            {
                // 月份 - 1 = 在list中的索引值
                int amount = Convert.ToInt32(rowView["amount"]);
                // 根據類別加或減取得結餘
                switch (rowView["type"].ToString())
                {
                    case "e":
                        expense += amount;
                        break;
                    case "i":
                        income += amount;
                        break;
                    default:
                        income += 0;
                        break;
                }

            }
            balance = income - expense;
            return new List<int> { expense, income, balance };
        }

        // 月份支出類別資料。回傳({支出類別Label}、{類別總支出金額})
        protected (List<string> labels, List<int> data) handleExpenseCategoryBar(int Year, int Month)
        {
            Dictionary<string, int> categoryLDict = AllCategories.ToDictionary(key => key, value => 0);

            SqlDataSource monthDetail = getDetailByMonthAndYear(Year, Month);
            DataView dataView = (DataView)monthDetail.Select(DataSourceSelectArguments.Empty);

            foreach (DataRowView rowView in dataView)
            {
                string category = rowView["category"].ToString();
                string type = rowView["type"].ToString().ToLower();
                int amount = Convert.ToInt32(rowView["amount"]);

                if (categoryLDict.ContainsKey(category) && type == "e")
                {
                    categoryLDict[category] += amount;
                }
            }
            List<KeyValuePair<string,int>> filteredData =
                categoryLDict
                .Where(category => category.Value != 0)
                .OrderByDescending(category => Math.Abs(category.Value))
                .ToList();

            return (
                labels: filteredData.Select(category => category.Key).ToList(),
                data: filteredData.Select(category => category.Value).ToList()
                );
        }

        // 月份支出類別資料。回傳({日期Label}、{每日支出}、{累加支出})
        protected List<List<int>> handleDailyExpenseLineAndBar(int Year, int Month)
        {
            List<int> dayList = Enumerable.Range(1, 31).ToList();
            List<int> expenseList = Enumerable.Repeat(0, 31).ToList();
            List<int> accumulateExpenseList = Enumerable.Repeat(0, 31).ToList();

            SqlDataSource monthDetail = getDetailByMonthAndYear(Year, Month);
            DataView dataView = (DataView)monthDetail.Select(DataSourceSelectArguments.Empty);

            foreach (DataRowView rowView in dataView)
            {
                if (rowView["type"].ToString() != "e") continue;

                int inx = Convert.ToInt32(rowView["day"]) - 1;
                int amount = Convert.ToInt32(rowView["amount"]);
                expenseList[inx] += amount;
            }
            for (int i = 0;i < 31;++i)
            {
                int singleDayExpense = expenseList[i];
                accumulateExpenseList[i] += singleDayExpense + (i == 0 ? 0 : accumulateExpenseList[i-1]);
                
            }
            return new List<List<int>> { dayList, expenseList, accumulateExpenseList };
        }

        /* ===================== 處理 QuickChart 的 Config 字串 ===================== */

        // 處理年度總覽折線圖 Config 字串
        private string proccessYearChartConfig(int year)
        {
            List<List<int>> yearLine = handleYearLine(year);
            List<int> yearLineLabel = yearLine[0];
            List<int> yearLineData = yearLine[1];
            string yearLineLabelJSON = JsonSerializer.Serialize(yearLineLabel.Select(m => m.ToString()));
            string yearLineDataJSON = JsonSerializer.Serialize(yearLineData);

            String Config = $@"{{
                type: 'line',
                data: {{
                    labels: {yearLineLabelJSON},
                    datasets: [
                        {{
                            label: '月結餘', 
                            backgroundColor: '#9b58ed', 
                            borderColor: '#9b58ed',
                            data: {yearLineDataJSON},
                            fill: false,
                            lineTension: 0.2,
                        }}
                    ],
                }},
                options: {{
                    responsive: false,
                    title: {{
                        display: true,
                        text: '{year} 年度總覽',
                        fontColor: '#ffffff',
                        fontStyle: 'bold',
                        fontSize: 20,
                    }},
                    scales: {{
                        yAxes: [
                            {{
                                ticks: {{ 
                                    suggestedMin: 10, 
                                    suggestedMax: 50,
                                    fontColor: '#ffffff',
                                    fontSize: 16
                                }},
                                gridLines: {{ 
                                    color: '#ffffff88',
                                    zeroLineColor: '#ffffff88'
                                }},
                            }},
                        ],
                        xAxes: [
                            {{
                                gridLines: {{
                                    display: true,
                                    drawOnChartArea: false,
                                    zeroLineColor: '#ffffff88',
                                    color: '#ffffff88',
                                }},
                                ticks: {{
                                    fontColor: '#ffffff',
                                    fontSize: 16
                                }},
                            }},
                        ],
                    }},
                    legend: {{
                        labels: {{
                            fontColor: '#ffffff',
                            fontSize: 16
                        }}
                    }}
                }},
            }}";

            return Config;
        }

        // 處理月份結餘甜甜圈圖表 Config 字串
        private string proccessBalanceChartConfig(int year, int month)
        {
            List<int> monthBalance = handleBalanceDoughnut(year,month);
            int expense = monthBalance[0];
            int income = monthBalance[1];
            int balance = monthBalance[2];


            String Config = $@"{{
                type: 'doughnut',
                data: {{
                  datasets: [
                    {{
                      data: [{income}, {expense}],
                      backgroundColor: [
                        '#007800',
                        '#ff0000',
                      ],
                      label: 'Dataset1',
                      borderWidth: 5,
                      borderColor: '#1e293b'
                    }},
                  ],
                  labels: ['收入', '支出'],
                }},
                options: {{
                  legend: {{
                    display: true,
                    labels: {{ 
                        fontColor: '#ffffff', 
                        fontSize: 16 
                    }}
                  }},
                  title: {{
                    display: true,
                    text: '{year} 年 {month} 月結餘',
                    fontColor: '#ffffff',
                    fontStyle: 'bold',
                    fontSize: '30',
                  }},
                  plugins: {{
                    doughnutlabel: {{
                      labels: [
                        {{
                          text: '{balance}', font: {{ size: 20  }}, color:'#ffffff'
                        }}, {{
                          text: '結餘', font: {{ size: 15  }}, color:'#ffffff55'
                        }}]
                    }},
                    datalabels: {{
                      display:true, font: {{ size: 20 ,style: 'bold' }}, color:'#ffffff'
                    }},
                  }}
                }},
              }}";
            return Config;
        }

        // 處理月份支出類別水平長條圖 Config 字串
        private string proccessExpenseCategoryChartConfig(int year, int month)
        {
            (List<string> labels, List<int> data) = handleExpenseCategoryBar(year,month);
            string ExpenseCategoryLabelJSON = JsonSerializer.Serialize(labels);
            string ExpenseCategoryLineDataJSON = JsonSerializer.Serialize(data);

            int maxVal = data.Count > 0 ? data.Max() : 0;
            String Config = $@"{{
                type: 'horizontalBar',
                data: {{
                    labels: {ExpenseCategoryLabelJSON},
                    datasets: [
                        {{
                            label: '金額',
                            data: {ExpenseCategoryLineDataJSON},
                            backgroundColor: '#9b58ed',
                        }},
                    ],
                }},
                options: {{
                    title: {{
                        display: true,
                        text: '{month} 月支出類別排行',
                        fontColor: '#ffffff',
                        fontStyle: 'bold',
                        fontSize: '30',
                    }},
                    scales: {{
                        xAxes: [
                            {{
                                gridLines: {{
                                    display: true,
                                    drawOnChartArea: false,
                                    zeroLineColor: '#ffffff88',
                                    color: '#ffffff88',
                                }},
                                ticks: {{
                                    fontColor: '#ffffff',
                                    fontSize: 16,
                                    suggestedMax: {maxVal + 20}
                                }},
                            }},
                        ],
                        yAxes: [
                            {{
                                display: true,
                                position: 'left',
                                gridLines: {{
                                    display: true,
                                    drawOnChartArea: false,
                                    tickMarkLength: 8,
                                    color: '#ffffff88',
                                }},
                                ticks: {{
                                    fontColor: '#ffffff',
                                    fontSize: 16
                                }},
                            }},
                        ],
                    }},
                    legend: {{
                        display: false,
                    }},
                    plugins: {{
                        datalabels: {{
                            anchor: 'end',
                            align: 'end',
                            color: '#ffffff',
                            font: {{
                                size: 12,
                                style: 'bold',
                            }},
                        }},
                    }}
                }},
            }}";
            return Config;
        }

        // 處理(月份每日支出長條圖)與(支出趨勢折線圖) Config 字串
        private string proccessDailyExpenseChartConfig(int year, int month)
        {
            List<List<int>> dailyExpense = handleDailyExpenseLineAndBar(year, month);

            List<int> dayLabel = dailyExpense[0];
            List<int> dailyExpenseData = dailyExpense[1];
            List<int> accumulateExpenseData = dailyExpense[2];

            string dayLabelJSON = JsonSerializer.Serialize(dayLabel.Select(m => m.ToString()));
            string dailyExpenseDataJSON = JsonSerializer.Serialize(dailyExpenseData);
            string accumulateExpenseDataJSON = JsonSerializer.Serialize(accumulateExpenseData);

            int maxDaily = dailyExpenseData.Count > 0 ? dailyExpenseData.Max() : 0;
            int maxAccumulate = accumulateExpenseData.Count > 0 ? accumulateExpenseData.Max() : 0;

            if (maxDaily == 0) maxDaily = 20;
            if (maxAccumulate == 0) maxAccumulate = 20;

            string Config = $@"{{
                type: 'bar',
                data: {{
                    labels: {dayLabelJSON},
                    datasets: [{{
                        type: 'line', 
                        label: '總支出', 
                        yAxisID: 'right-y-axis', 
                        order: 1,
                        borderColor: '#ff9100', 
                        borderWidth: 4, 
                        fill: false,
                        lineTension: 0,
                        data: {accumulateExpenseDataJSON}
                    }}, {{
                        type: 'bar', 
                        label: '每日支出', 
                        yAxisID: 'left-y-axis', 
                        order: 2,
                        backgroundColor: '#9b58ed', 
                        borderWidth: 0,
                        data: {dailyExpenseDataJSON}
                    }}]
                }},
                options: {{
                    title: {{ 
                        display: true, 
                        text: '{month}月 每日支出趨勢圖', 
                        fontColor: '#ffffff', 
                        fontSize: 20 
                    }},
                    tooltips: {{ 
                        mode: 'index', 
                        intersect: true 
                    }},
                    scales: {{
                        xAxes: [{{
                            gridLines: {{ 
                                display: true, 
                                drawOnChartArea: false, 
                                color: '#ffffff55' 
                            }},
                            ticks: {{ 
                                fontColor: '#ffffff', 
                                fontSize: 16 
                            }}
                        }}],
                        yAxes: [{{
                            id: 'left-y-axis', 
                            position: 'left', 
                            display: true,
                            gridLines: {{
                                display: true,
                                color: '#ffffff55', 
                                zeroLineColor: '#ffffff' 
                            }},
                            ticks: {{ 
                                fontColor: '#ffffff', 
                                fontSize: 16, 
                                beginAtZero: true, 
                                max: {maxDaily + 20}
                            }}
                        }}, {{
                            id: 'right-y-axis',
                            position: 'right',
                            display: true,
                            gridLines: {{ 
                                display: false, 
                            }},
                            ticks: {{ 
                                fontColor: '#ffffff', 
                                fontSize: 16, 
                                beginAtZero: true,
                                max: {maxAccumulate + 20}
                            }}
                        }}]
                    }},
                    legend: {{
                        display: true,
                        labels: {{ 
                            fontColor: '#ffffff', 
                            fontSize: 16 
                        }}
                    }},
                    plugins: {{
                        datalabels: {{
                            anchor: 'end', 
                            align: 'end', 
                            color: '#9b58ed', 
                            font: {{ size: 18, style: 'bold' }},
                            display: function (context) {{ return context.dataset.type === 'bar'; }}
                        }}
                    }}
                }}
            }}";
            return Config;
        }

        /* ============== 根據 chart_type，依序利用 QuickChart 生成圖表 ============== */
        protected void handleChart(int year, int month)
        {
            foreach (string type in chart_type)
            {
                Chart switchChart = new Chart();
                switchChart.Version = "2.9.4";
                switchChart.BackgroundColor = "#1e293b";

                switch (type)
                {
                    case "year_line":
                        switchChart.Width = 1024;
                        switchChart.Height = 320;
                        switchChart.Config = proccessYearChartConfig(year);
                        year_chart.ImageUrl = switchChart.GetUrl();
                        break;
                    case "balance_doughnut":
                        switchChart.Width = 512;
                        switchChart.Height = 512;
                        switchChart.Config = proccessBalanceChartConfig(year, month);
                        balance_chart.ImageUrl = switchChart.GetUrl();
                        break;
                    case "expense_pie":
                        switchChart.Width = 512;
                        switchChart.Height = 512;
                        switchChart.Config = proccessExpenseCategoryChartConfig(year, month);
                        expense_chart.ImageUrl = switchChart.GetUrl();
                        break;
                    case "daily_expense":
                        switchChart.Width = 1024;
                        switchChart.Height = 512;
                        switchChart.Config = proccessDailyExpenseChartConfig(year, month);
                        daily_expense_chart.ImageUrl = switchChart.GetUrl();
                        break;
                }
            }
        }

        /* ================================ 載入事件 ================================ */
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
                handleChart(currentYear, currentMonth);

            }
        }

    }
    }


// method
/*
 private (List<string> labels, List<int> data) GetCategoryTotalsMonthly(int Year, int Month)
        {

            Dictionary<string, int> categoryTotals = AllCategories.ToDictionary(key => key, value => 0);


            SqlDataSource monthDetail = getDetailByMonthAndYear(Year, Month);


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
                        categoryTotals[category] += amount;
                    }
                    else if (type == "i")
                    {

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
        private (List<string> labels, List<int> data) GetCategoryTotalsYearly(int Year)
        {
            Dictionary<string, int> categoryTotals = AllCategories.ToDictionary(key => key, value => 0);

            SqlDataSource yearDetail = getDetailByYear(Year);
            DataView dataView = (DataView)yearDetail.Select(DataSourceSelectArguments.Empty);

            foreach (DataRowView rowView in dataView)
            {
                string category = rowView["Category"].ToString();
                string type = rowView["Type"].ToString().ToLower();
                int amount = Convert.ToInt32(rowView["Amount"]);
                if (categoryTotals.ContainsKey(category) && type == "e")
                {
                    categoryTotals[category] += amount;
                }
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

        protected List<List<int>> handleMonthLine(int Year, int Month)
        {
            var date = Enumerable.Range(1, 31).ToList();
            var balance = Enumerable.Repeat(0, 31).ToList();

            SqlDataSource monthDetail = getDetailByMonthAndYear(Year, Month);
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

        protected List<List<int>> handledailyLine(int Year, int Month)
        {
            var date = Enumerable.Range(1, 31).ToList();
            var balance = Enumerable.Repeat(0, 31).ToList();

            SqlDataSource monthDetail = getDetailByMonthAndYear(Year, Month);
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
                        balance[inx] += amount;
                        break;
                    case "i":

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
 */
// code
/**
 * 

            List<List<int>> qwq1 = handleMonthLine(currentYear, currentMonth);
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
                    type: 'bar',
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

            var result = GetCategoryTotalsYearly(currentYear);
            var resultdaily = GetCategoryTotalsMonthly(currentYear, currentMonth);


            string labelsJson = JsonSerializer.Serialize(month.Select(x => x.ToString()));
            string dataJson = JsonSerializer.Serialize(balance);

            string categoryLabelsJson = JsonSerializer.Serialize(result.labels);
            string categoryDataJson = JsonSerializer.Serialize(result.data);

            string dailyLabelsJson = JsonSerializer.Serialize(resultdaily.labels);
            string dailyDataJson = JsonSerializer.Serialize(resultdaily.data);

            List<List<int>> qwq2 = handledailyLine(currentYear, currentMonth);
            List<int> date2 = qwq2[0];
            List<int> balance2 = qwq2[1];
            string dailylabelsJson = JsonSerializer.Serialize(date2.Select(x => x.ToString()));
            string dailydataJson = JsonSerializer.Serialize(balance2);

            Chart qc3 = new Chart();
            qc3.Width = 500;
            qc3.Height = 300;
            qc3.Version = "2.9.4";

            // 2. 注意這裡多了 '$' 符號，變成 $@"..."
            qc3.Config = $@"{{
                    type: 'line',
                    data: {{
                        labels: {dailylabelsJson},
                        datasets: [{{
                            label: '支出',
                            data: {dailydataJson},
                            fill: false,
                            tension: 0.4,
                            beginAtZero: true
                        }}]
                    }}
                }}";
            qc3.BackgroundColor = "#1e293b";

            //daliy_expense_chart.ImageUrl = qc3.GetUrl();

            Chart qc2 = new Chart();
            qc2.Width = 500;
            qc2.Height = 300;
            qc2.Version = "2.9.4";

            // 2. 注意這裡多了 '$' 符號，變成 $@"..."
            qc2.Config = $@"{{
                    type: 'bar',
                    data: {{
                        labels: {dailyLabelsJson},
                        datasets: [{{
                            label: '支出',
                            data: {dailyDataJson},
                            fill: false,
                        }}]
                    }}
                }}";
            qc2.BackgroundColor = "#1e293b";

            expense_chart.ImageUrl = qc2.GetUrl();
 */