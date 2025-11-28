using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace web_programming_project
{
    public partial class home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
            var detailData = new[] {
                new {category = "飲食", description = "午餐便當", amount = 100},
                new { category = "交通", description = "捷運儲值", amount = 500 },
                new { category = "交通", description = "儲值", amount = 500 },
                new { category = "娛樂", description = "看電影", amount = 300 }
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