using Microsoft.Ajax.Utilities;
using System;
using System.Activities.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CM_Default : System.Web.UI.Page
{
    /// <summary>
    /// 連線字串
    /// </summary>
    private string connectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        if (!IsPostBack)
        {
            BindData();
            BindData_CONTRACT_TYPE();
            BindData_ddlVENDOR();
        }        
    }

    private void BindData_ddlVENDOR()
    {
        string sql = @"SELECT  
                            [VENDOR_ID]                          
                          ,[VENDOR_SHORT]                          
                      FROM [ZFCF_CM_VENDOR]
                      where DISABLE = 0
        ";
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand(sql, conn))
            {

                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                ddlVENDOR_ID.DataSource = dt;
                ddlVENDOR_ID.DataTextField = "VENDOR_SHORT"; //指定選項文字是資料表的哪個欄位
                ddlVENDOR_ID.DataValueField = "VENDOR_ID"; //指定選項值 是資料表的哪個欄位
                ddlVENDOR_ID.DataBind();
            }
        }
    }

    private void BindData_CONTRACT_TYPE()
    {
        string sql = @"
            SELECT
                [CONTRACT_TYPE_ID],
                [CONTRACT_CATEGORY]
            FROM [ZFCF_CM_CONTRACT_TYPE]";
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new SqlCommand(sql, conn))
            {
                
                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                rblCONTRACT_TYPE_ID.DataSource = dt;
                rblCONTRACT_TYPE_ID.DataTextField = "CONTRACT_CATEGORY"; //顯示的文字
                rblCONTRACT_TYPE_ID.DataValueField = "CONTRACT_TYPE_ID"; //值
                rblCONTRACT_TYPE_ID.DataBind();                
            }
        }
    }

    /// <summary>
    /// Grid View 讀取資料表
    /// </summary>
    private void BindData()
    {
        //分頁大小
        int pageSize = gvList.PageSize;
        //第幾頁
        int pageIndex = gvList.PageIndex;
        //計算跳過幾筆
        //Page1 01-10 ,pageIndex = 0 * 10 = skip 0
        //Page2 11-20 ,pageIndex = 1 * 10 = skip 10
        int offset = pageIndex * pageSize;        
        //總筆數
        int totalCount = 0;
        #region 組合查詢字串
        string whereSql = "";
        //有關鍵字才查詢
        if (!string.IsNullOrWhiteSpace(tbBPM_Keyword.Text))
        {
            whereSql += " AND [BPM_NBR] like @bpm_nbr ";//小細節 前後加空白
        }
        if (!string.IsNullOrWhiteSpace(tbSAP_Keyword.Text))
        {
            whereSql += " AND [SAP_NBR] like @sap_nbr ";
        }
        //複合欄位搜尋 同時查SAP_NBR 和 BPM_NBR
        if (!string.IsNullOrWhiteSpace(tbSAP_BPM_Keyword.Text))
        {
            whereSql += @" AND (
                            ([SAP_NBR] like @sap_bpm_nbr) or 
                            ([BPM_NBR] like @sap_bpm_nbr) 
                        )";
        }
        //搜尋金額的起迄範圍
        decimal amountStart = 0;
        if (!string.IsNullOrWhiteSpace(tbAMOUNT_START.Text) && 
            decimal.TryParse(tbAMOUNT_START.Text,out amountStart)) {
            whereSql += "AND AMOUNT >= @amount_start ";
        }
        decimal amountEnd = 0;
        if (!string.IsNullOrWhiteSpace(tbAMOUNT_END.Text) &&
            decimal.TryParse(tbAMOUNT_END.Text, out amountEnd)
            )
        {
            whereSql += "AND AMOUNT <= @amount_end ";
        }
        //搜尋廠商名稱
        if (!string.IsNullOrWhiteSpace(tbVENDER_NAME_Keyword.Text))
        {
            whereSql += "AND ([VENDOR_SHORT] like @vender_name  or [VENDOR_NAME_NOMALIZE] like @vender_name)";
        }
        #endregion 組合查詢字串
        string sql = @"
            SELECT
                [CONTRACT_ID]
                  ,[VENDOR_SHORT] --廠商名稱
                  ,[TITLE]
                  ,[SAP_NBR]
                  ,[BPM_NBR]
                  --,[CONTRACT_TYPE_ID]
                  ,b.CONTRACT_CATEGORY --合約類型名稱
                  ,[TERM]
                  ,[START_DATE]
                  ,[END_DATE]
                  ,[AMOUNT]
                  ,[INSTALLMENTS]
            FROM [ZFCF_CM_CONTRACT] AS a

            JOIN [ZFCF_CM_CONTRACT_TYPE] AS b -- 組合合約類型
            ON a.CONTRACT_TYPE_ID = b.CONTRACT_TYPE_ID

            JOIN [ZFCF_CM_VENDOR] as c --組合廠商資料
            on a.VENDOR_ID = c.VENDOR_ID

            WHERE 1 = 1 " + whereSql  + @"
            ORDER BY [CONTRACT_ID] DESC --ASC 升冪排序 DESC 降冪排序
            OFFSET @offset ROWS  --跳過幾行
            FETCH NEXT @pageSize ROWS ONLY; --抓接下來幾行
";
        //參數化查詢 整理成一個陣列
        //使用 SqlParameter[] 陣列來存放參數
        //SqlParameter[] parameters = new SqlParameter[]
        //{
        //    new SqlParameter("@pageSize", pageSize),
        //    new SqlParameter("@offset", offset),
        //    new SqlParameter("@bpm_nbr", "%" + tbBPM_Keyword.Text?.Trim().ToUpper() + "%"),
        //    new SqlParameter("@sap_nbr", "%" + tbSAP_Keyword.Text?.Trim().ToUpper() + "%"),
        //    new SqlParameter("@sap_bpm_nbr", "%" + tbSAP_BPM_Keyword.Text?.Trim().ToUpper() + "%"),
        //    new SqlParameter("@amount_start", "%" + tbSAP_BPM_Keyword.Text?.Trim().ToUpper() + "%")
        //};
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            DataTable dt = new DataTable();
            using (var cmd = new SqlCommand(sql, conn))
            {
                //cmd.Parameters.AddRange(parameters);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@bpm_nbr", "%" + tbBPM_Keyword.Text?.Trim().ToUpper() + "%");
                cmd.Parameters.AddWithValue("@sap_nbr", "%" + tbSAP_Keyword.Text?.Trim().ToUpper() + "%");
                cmd.Parameters.AddWithValue("@sap_bpm_nbr", "%" + tbSAP_BPM_Keyword.Text?.Trim().ToUpper() + "%");
                cmd.Parameters.AddWithValue("@amount_start", amountStart);
                cmd.Parameters.AddWithValue("@amount_end", amountEnd);
                cmd.Parameters.AddWithValue("@vender_name", $"%{tbVENDER_NAME_Keyword.Text?.Trim().ToUpper()}%");
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            //讀取資料筆數 這邊共用之前的conn連線
            string sqlCount = @"
                SELECT COUNT(1) 
                FROM [ZFCF_CM_CONTRACT]  AS a
                    
                JOIN [ZFCF_CM_CONTRACT_TYPE] AS b -- 組合合約類型
                ON a.CONTRACT_TYPE_ID = b.CONTRACT_TYPE_ID

                JOIN [ZFCF_CM_VENDOR] as c --組合廠商資料
                on a.VENDOR_ID = c.VENDOR_ID

                WHERE 1 = 1 " + whereSql;
            using(var cmd = new SqlCommand(sqlCount, conn)) {
                //cmd.Parameters.AddRange(parameters);
                cmd.Parameters.AddWithValue("@bpm_nbr", "%" + tbBPM_Keyword.Text?.Trim().ToUpper() + "%");
                cmd.Parameters.AddWithValue("@sap_nbr", "%" + tbSAP_Keyword.Text?.Trim().ToUpper() + "%");
                cmd.Parameters.AddWithValue("@sap_bpm_nbr", "%" + tbSAP_BPM_Keyword.Text?.Trim().ToUpper() + "%");
                cmd.Parameters.AddWithValue("@amount_start", amountStart);
                cmd.Parameters.AddWithValue("@amount_end", amountEnd);
                cmd.Parameters.AddWithValue("@vender_name", $"%{tbVENDER_NAME_Keyword.Text?.Trim().ToUpper()}%");
                //ExecuteScalar = 回傳第一列的第一格資料
                totalCount = Convert.ToInt32(cmd.ExecuteScalar());
                lbTotalCount.Text = totalCount.ToString();
            }
            gvList.VirtualItemCount = totalCount;
            //gvList.AllowCustomPaging = true;
            gvList.DataSource = dt;
            gvList.DataBind();
        }      
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        clearForm();
        areaEdit.Visible = true;
        areaList.Visible = false;
        hiMode.Value = "add";
    }

    /// <summary>
    /// 清除表單內容
    /// </summary>
    private void clearForm()
    {
        //清除 rblCONTRACT_TYPE_ID
        rblCONTRACT_TYPE_ID.ClearSelection();
        tbTITLE.Text = string.Empty;
        tbSAP_NBR.Text = string.Empty;
        tbBPM_NBR.Text = string.Empty;
        tbSTART_DATE.Text = string.Empty;
        tbEND_DATE.Text = string.Empty;
        //tbVENDOR_ID.Text = string.Empty;
        ddlVENDOR_ID.ClearSelection();
        tbCONTRACT_TYPE_ID.Text = string.Empty;
        tbTERM.Text = string.Empty;
        tbAMOUNT.Text = string.Empty;
        tbINSTALLMENTS.Text = string.Empty;
        tbNOTE.Text = string.Empty;
    }

    /// <summary>
    /// 存檔
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {


        #region 讀取表單內容     
        //1. 讀資料+邏輯驗證
        //合約名稱
        string TITLE = tbTITLE.Text?.Trim(); 
        //SAP付款憑單單號
        string SAP_NBR = tbSAP_NBR.Text?.Trim().ToUpper(); 
        //BPM簽呈編號
        string BPM_NBR = tbBPM_NBR.Text?.Trim().ToUpper();
        //合約起始時間
        string START_DATE = tbSTART_DATE.Text;
        //合約結束時間
        string END_DATE = tbEND_DATE.Text;
        //備註
        string NOTE = tbNOTE.Text?.Trim();
        DateTime CREATE_TIME = DateTime.Now;
        DateTime UPDATE_TIME = DateTime.Now;
        string CREATE_UID = "TEST";
        string UPDATE_UID = "TEST";
        //int的另外處理
        int VENDOR_ID = 0;
        int.TryParse(ddlVENDOR_ID.SelectedValue, out VENDOR_ID);        
        int CONTRACT_TYPE_ID = 0;
        int.TryParse(tbCONTRACT_TYPE_ID.Text, out CONTRACT_TYPE_ID);
        int TERM = 0;
        int.TryParse(tbTERM.Text, out TERM);
        int INSTALLMENTS = 0;
        int.TryParse(tbINSTALLMENTS.Text, out INSTALLMENTS);
        //decimal
        //金額
        decimal AMOUNT = 0;
        if(!decimal.TryParse(tbAMOUNT.Text, out AMOUNT))
        {
            //轉換失敗 提示錯誤訊息
        }
        #endregion
        
        if (hiMode.Value=="add")
        {
            //新增
            //2. 準備SQL
            string sql = @"
INSERT INTO [ZFCF_CM_CONTRACT]  (
        [VENDOR_ID]
      ,[TITLE]
      ,[SAP_NBR]
      ,[BPM_NBR]
      ,[CONTRACT_TYPE_ID]
      ,[TERM]
      ,[START_DATE]
      ,[END_DATE]
      ,[AMOUNT]
      ,[INSTALLMENTS]
      ,[CREATE_TIME]
      ,[UPDATE_TIME]
      ,[CREATE_UID]
      ,[UPDATE_UID]
      ,[NOTE]
) VALUES (
       @VENDOR_ID
      ,@TITLE
      ,@SAP_NBR
      ,@BPM_NBR
      ,@CONTRACT_TYPE_ID
      ,@TERM
      ,@START_DATE
      ,@END_DATE
      ,@AMOUNT
      ,@INSTALLMENTS
      ,@CREATE_TIME
      ,@UPDATE_TIME
      ,@CREATE_UID
      ,@UPDATE_UID
      ,@NOTE)";
            //3. 執行寫入

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    //加上參數
                    cmd.Parameters.AddWithValue("@VENDOR_ID", VENDOR_ID);
                    cmd.Parameters.AddWithValue("@TITLE", TITLE);
                    cmd.Parameters.AddWithValue("@SAP_NBR", SAP_NBR);
                    cmd.Parameters.AddWithValue("@BPM_NBR", BPM_NBR);
                    cmd.Parameters.AddWithValue("@CONTRACT_TYPE_ID", CONTRACT_TYPE_ID);
                    cmd.Parameters.AddWithValue("@TERM", TERM);
                    cmd.Parameters.AddWithValue("@START_DATE", START_DATE);
                    cmd.Parameters.AddWithValue("@END_DATE", END_DATE);
                    cmd.Parameters.AddWithValue("@AMOUNT", AMOUNT);
                    cmd.Parameters.AddWithValue("@INSTALLMENTS", INSTALLMENTS);
                    cmd.Parameters.AddWithValue("@CREATE_TIME", CREATE_TIME);
                    cmd.Parameters.AddWithValue("@UPDATE_TIME", UPDATE_TIME);
                    cmd.Parameters.AddWithValue("@CREATE_UID", CREATE_UID);
                    cmd.Parameters.AddWithValue("@UPDATE_UID", UPDATE_UID);
                    cmd.Parameters.AddWithValue("@NOTE", NOTE);
                    //執行寫入
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        else
        {
            const string sql = @"
                UPDATE [ZFCF_CM_CONTRACT]
                SET
                     [VENDOR_ID] = @VENDOR_ID
                      ,[TITLE] = @TITLE
                      ,[SAP_NBR] = @SAP_NBR
                      ,[BPM_NBR] = @BPM_NBR
                      ,[CONTRACT_TYPE_ID] = @CONTRACT_TYPE_ID
                      ,[TERM] = @TERM
                      ,[START_DATE] = @START_DATE
                      ,[END_DATE] = @END_DATE
                      ,[AMOUNT] = @AMOUNT
                      ,[INSTALLMENTS] = @INSTALLMENTS                      
                      ,[UPDATE_TIME] = @UPDATE_TIME                      
                      ,[UPDATE_UID] = @UPDATE_UID
                      ,[NOTE] = @NOTE
                WHERE [CONTRACT_ID] = @id
";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    //加上參數
                    cmd.Parameters.AddWithValue("@VENDOR_ID", VENDOR_ID);
                    cmd.Parameters.AddWithValue("@TITLE", TITLE);
                    cmd.Parameters.AddWithValue("@SAP_NBR", SAP_NBR);
                    cmd.Parameters.AddWithValue("@BPM_NBR", BPM_NBR);
                    cmd.Parameters.AddWithValue("@CONTRACT_TYPE_ID", CONTRACT_TYPE_ID);
                    cmd.Parameters.AddWithValue("@TERM", TERM);
                    cmd.Parameters.AddWithValue("@START_DATE", START_DATE);
                    cmd.Parameters.AddWithValue("@END_DATE", END_DATE);
                    cmd.Parameters.AddWithValue("@AMOUNT", AMOUNT);
                    cmd.Parameters.AddWithValue("@INSTALLMENTS", INSTALLMENTS);
                    cmd.Parameters.AddWithValue("@CREATE_TIME", CREATE_TIME);
                    cmd.Parameters.AddWithValue("@UPDATE_TIME", UPDATE_TIME);
                    cmd.Parameters.AddWithValue("@CREATE_UID", CREATE_UID);
                    cmd.Parameters.AddWithValue("@UPDATE_UID", UPDATE_UID);
                    cmd.Parameters.AddWithValue("@NOTE", NOTE);
                    cmd.Parameters.AddWithValue("@id", hid.Value);
                    //執行寫入
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            //編輯
        }

        areaEdit.Visible = false;
        areaList.Visible = true;
        BindData();
    }

    /// <summary>
    /// 取消不存檔
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        areaEdit.Visible = false;
        areaList.Visible = true;
    }

    /// <summary>
    /// 編輯+刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();
        //編輯
        if (e.CommandName == "DataEdit")
        {            
            //TODO 觸發編輯
            clearForm();
            //TODO 讀取資料
            string sql = @"
            SELECT Top 1  *
            FROM [ZFCF_CM_CONTRACT] 
            WHERE [CONTRACT_ID] = @id
";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //方法二 使用DataTable
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                        if(dt.Rows.Count > 0)
                        {
                            // 先取得 DataRow
                            var row = dt.Rows[0];
                            //資料綁定
                            hid.Value = id;
                            tbTITLE.Text = row["TITLE"].ToString();
                            ddlVENDOR_ID.SelectedValue = row["VENDOR_ID"].ToString();
                            tbSAP_NBR.Text = row["SAP_NBR"].ToString();
                            tbBPM_NBR.Text = row["BPM_NBR"].ToString();
                            tbCONTRACT_TYPE_ID.Text = row["CONTRACT_TYPE_ID"].ToString();
                            tbTERM.Text = row["TERM"].ToString();
                            tbAMOUNT.Text = row["AMOUNT"].ToString();
                            tbINSTALLMENTS.Text = row["INSTALLMENTS"].ToString();
                            tbNOTE.Text = row["NOTE"].ToString();
                           

                            // 修正 START_DATE
                            if (!row.IsNull("START_DATE"))
                            {
                                var start = row.Field<DateTime>("START_DATE");     //轉型                        
                                tbSTART_DATE.Text = start.ToString("yyyy/MM/dd");  // or start.ToShortDateString()
                                //yyyy 西元年 MM 月 dd 日 HH小時 mm分鐘 ss秒 fff毫秒
                                
                                //如果遇到要轉民國年
                                //yyyy-1911
                                var rocYear = start.Year - 1911; //轉民國年
                            }           

                            // 同樣修正 END_DATE
                            if (!row.IsNull("END_DATE"))
                            {
                                var end = row.Field<DateTime>("END_DATE");
                                tbEND_DATE.Text = end.ToString("yyyy/MM/dd");
                            }

                            areaEdit.Visible = true;
                            areaList.Visible = false;
                            hiMode.Value = "edit";
                        }
                    }
                }
            }
        }

        //刪除
        if (e.CommandName == "DataDel")
        {
            const string sql = @"
            DELETE [ZFCF_CM_CONTRACT] 
            WHERE [CONTRACT_ID] = @id
";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    //加上參數
                    cmd.Parameters.AddWithValue("@id", id);
                    //執行寫入
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            BindData();
        }
    }

    /// <summary>
    /// 頁碼改變
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        var page = e.NewPageIndex;
        gvList.PageIndex = page;
        BindData();
    }

    /// <summary>
    /// 分頁大小改變觸發
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        //修改頁面大小
        gvList.PageSize = int.Parse(ddlPageSize.SelectedValue);
        gvList.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
        //修改頁面大小後跑回第一頁
        gvList.PageIndex = 0;
        //重新綁定資料
        BindData();
    }

    /// <summary>
    /// 搜尋
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindData();
    }

    /// <summary>
    /// 清除搜尋
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        tbBPM_Keyword.Text = string.Empty;
        tbSAP_Keyword.Text = string.Empty;
        tbSAP_BPM_Keyword.Text = string.Empty;
        tbAMOUNT_START.Text = string.Empty;
        tbAMOUNT_END.Text = string.Empty;
        tbVENDER_NAME_Keyword.Text = string.Empty;
        BindData();
    }
}