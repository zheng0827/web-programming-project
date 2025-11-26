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
            testBindData();
        }

        private void testBindData()
        {
            // 假設這是您從資料庫撈出來的 DataTable 或 List
            // 這裡我們模擬剛剛初始化的那 5 筆資料
            var dataFromDb = new[]
            {
                new { category = "測試一", description = "test1@example.com", date = DateTime.Now ,amount = 0},
                new { category = "測試二", description = "test2@example.com", date = DateTime.Now ,amount = 1},
                new { category = "測試三", description = "test3@example.com", date = DateTime.Now ,amount = 2},
                new { category = "測試四", description = "test4@example.com", date = DateTime.Now ,amount = 3},
                new { category = "測試五", description = "test5@example.com", date = DateTime.Now ,amount = 4}
            };

            Repeater1.DataSource = dataFromDb;
            Repeater1.DataBind();
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}