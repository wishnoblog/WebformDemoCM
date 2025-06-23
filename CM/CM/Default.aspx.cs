using System;
using System.Activities.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        }        
    }

    /// <summary>
    /// Grid View 讀取資料表
    /// </summary>
    private void BindData()
    {
        string sql = @"
            SELECT
                [CONTRACT_ID]
                  ,[VENDOR_ID]
                  ,[TITLE]
                  ,[SAP_NBR]
                  ,[BPM_NBR]
                  ,[CONTRACT_TYPE_ID]
                  ,[TERM]
                  ,[START_DATE]
                  ,[END_DATE]
                  ,[AMOUNT]
                  ,[INSTALLMENTS]
            FROM [ZFCF_CM_CONTRACT]";
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            
            using(var cmd = new SqlCommand(sql, conn))
            {
                //方法一 使用SQL Data Reader
                //var reader = cmd.ExecuteReader();
                //gvList.DataSource = reader;
                //gvList.DataBind();

                //方法二 使用DataTable
                DataTable dt = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                gvList.DataSource = dt;
                gvList.DataBind();

                //方法三 DataSet 用在一次讀取很多個表
                //DataSet ds = new DataSet();
                //using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                //{
                //    adapter.Fill(ds);
                //}
                //gvList.DataSource = ds;
                //gvList.DataBind();
            }
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        clearForm();
        areaEdit.Visible = true;
        areaList.Visible = false;
        hiMode.Value = "add";
    }

    private void clearForm()
    {
        tbTITLE.Text = string.Empty;
        tbSAP_NBR.Text = string.Empty;
        tbBPM_NBR.Text = string.Empty;
        tbSTART_DATE.Text = string.Empty;
        tbEND_DATE.Text = string.Empty;
        tbVENDOR_ID.Text = string.Empty;
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
        int.TryParse(tbVENDOR_ID.Text, out VENDOR_ID);
        
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
                            tbVENDOR_ID.Text = row["VENDOR_ID"].ToString();
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

    protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        var page = e.NewPageIndex;
        gvList.PageIndex = page;
        BindData();
    }
}