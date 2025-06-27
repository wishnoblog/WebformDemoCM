using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CM_ZFCF_CM_VENDOR : System.Web.UI.Page
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

    private void BindData()
    {
        //分頁大小
        int pageSize = gv.PageSize;
        //第幾頁
        int pageIndex = gv.PageIndex;
        //計算跳過幾筆
        //Page1 01-10 ,pageIndex = 0 * 10 = skip 0
        //Page2 11-20 ,pageIndex = 1 * 10 = skip 10
        int offset = pageIndex * pageSize;
        //總筆數
        int totalCount = 0;
        #region 組合查詢字串
        string whereSql = "";
        //有關鍵字才查詢
        
       
        //搜尋廠商名稱
        if (!string.IsNullOrWhiteSpace(tbVENDER_NAME_Keyword.Text))
        {
            whereSql += "AND ([VENDOR_SHORT] like @vender_name  or [VENDOR_NAME_NOMALIZE] like @vender_name)";
        }
        #endregion 組合查詢字串
        string sql = @"
            SELECT
                *
            FROM [ZFCF_CM_VENDOR]

            WHERE 1 = 1 " + whereSql + @"
            ORDER BY [VENDOR_NAME]
            OFFSET @offset ROWS  --跳過幾行
            FETCH NEXT @pageSize ROWS ONLY; --抓接下來幾行
";

        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            DataTable dt = new DataTable();
            using (var cmd = new SqlCommand(sql, conn))
            {
                //cmd.Parameters.AddRange(parameters);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@vender_name", $"%{tbVENDER_NAME_Keyword.Text?.Trim().ToUpper()}%");
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            //讀取資料筆數 這邊共用之前的conn連線
            string sqlCount = @"
                SELECT COUNT(1) 
                FROM [ZFCF_CM_VENDOR]  AS a                   
                WHERE 1 = 1 " + whereSql;
            using (var cmd = new SqlCommand(sqlCount, conn))
            {
                //cmd.Parameters.AddRange(parameters);

                cmd.Parameters.AddWithValue("@vender_name", $"%{tbVENDER_NAME_Keyword.Text?.Trim().ToUpper()}%");
                //ExecuteScalar = 回傳第一列的第一格資料
                totalCount = Convert.ToInt32(cmd.ExecuteScalar());
                lbTotalCount.Text = totalCount.ToString();
            }
            gv.VirtualItemCount = totalCount;
            //gvList.AllowCustomPaging = true;
            gv.DataSource = dt;
            gv.DataBind();
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
        tbVENDOR_SHORT.Text = string.Empty;
        tbVENDOR_NAME.Text = string.Empty;      
    }

    /// <summary>
    /// 存檔
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {

        if (hiMode.Value == "add")
        {
            //新增
            //2. 準備SQL
            string sql = @"
            INSERT INTO [ZFCF_CM_VENDOR]  (
                    [VENDOR_NAME]
                  ,[VENDOR_SHORT]
                  ,[VENDOR_NAME_NOMALIZE]
                  ,[DISABLE]
            ) VALUES (
                    @VENDOR_NAME,
                    @VENDOR_SHORT,
                    @VENDOR_NAME_NOMALIZE,
                    @DISABLE
            )";
            //3. 執行寫入

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    //加上參數
                    cmd.Parameters.AddWithValue("@VENDOR_ID", hid.Value);
                    cmd.Parameters.AddWithValue("@VENDOR_NAME", tbVENDOR_NAME.Text?.Trim());
                    cmd.Parameters.AddWithValue("@VENDOR_SHORT", tbVENDOR_SHORT.Text?.Trim());
                    cmd.Parameters.AddWithValue("@VENDOR_NAME_NOMALIZE", tbVENDOR_NAME.Text?.Trim().ToUpper());
                    cmd.Parameters.AddWithValue("@DISABLE", cbDisable.Checked);
                    //執行寫入
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        else
        {
            const string sql = @"
                UPDATE [ZFCF_CM_VENDOR]
                SET
                     [VENDOR_NAME] = @VENDOR_NAME
                    ,[VENDOR_SHORT] = @VENDOR_SHORT
                    ,[VENDOR_NAME_NOMALIZE] = @VENDOR_NAME_NOMALIZE    
                    ,[DISABLE] = @DISABLE
                WHERE [VENDOR_ID] = @VENDOR_ID
";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    //加上參數
                    cmd.Parameters.AddWithValue("@VENDOR_ID", hid.Value);
                    cmd.Parameters.AddWithValue("@VENDOR_NAME", tbVENDOR_NAME.Text?.Trim());
                    cmd.Parameters.AddWithValue("@VENDOR_SHORT", tbVENDOR_SHORT.Text?.Trim());
                    cmd.Parameters.AddWithValue("@VENDOR_NAME_NOMALIZE", tbVENDOR_NAME.Text?.Trim().ToUpper());
                    cmd.Parameters.AddWithValue("@DISABLE", cbDisable.Checked);
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
            FROM [ZFCF_CM_VENDOR]
            WHERE [VENDOR_ID] = @id
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
                        if (dt.Rows.Count > 0)
                        {
                            // 先取得 DataRow
                            var row = dt.Rows[0];
                            //資料綁定
                            hid.Value = id;
                            tbVENDOR_NAME.Text = row.Field<string>("VENDOR_NAME");
                            tbVENDOR_SHORT.Text = row.Field<string>("VENDOR_SHORT");
                            cbDisable.Checked = row.Field<bool>("DISABLE");
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
            DELETE [ZFCF_CM_VENDOR] 
            WHERE [VENDOR_ID] = @id
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
        gv.PageIndex = page;
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
        gv.PageSize = int.Parse(ddlPageSize.SelectedValue);
        gv.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
        //修改頁面大小後跑回第一頁
        gv.PageIndex = 0;
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
        tbVENDER_NAME_Keyword.Text = string.Empty;
        BindData();
    }

}