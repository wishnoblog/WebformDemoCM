<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ZFCF_CM_VENDOR.aspx.cs" Inherits="CM_ZFCF_CM_VENDOR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div runat="server" id="areaEdit" visible="false">
            <table>
                <tr>
                    <td>廠商名稱</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbVENDOR_NAME"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>廠商名稱簡稱</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbVENDOR_SHORT"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>停用</td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbDisable"></asp:CheckBox>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hid" runat="server" />
            <asp:HiddenField ID="hiMode" runat="server" />
            <asp:Button ID="btnSave" Text="存檔" runat="server" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" Text="取消" runat="server" OnClick="btnCancel_Click" />
        </div>
        <div runat="server" id="areaList">
            <h1>廠商資料維護</h1>
            <div>
                廠商名稱<asp:TextBox ID="tbVENDER_NAME_Keyword" runat="server" placeHolder="請輸入關鍵字"></asp:TextBox>
                <br />
                <asp:Button Text="搜尋" ID="btnSearch" OnClick="btnSearch_Click" runat="server" />
                <asp:Button Text="清除" ID="btnClear" OnClick="btnClear_Click" runat="server" />
            </div>
            <a href="Default.aspx">返回</a>
            <asp:Button ID="btnAdd" Text="新增" runat="server" OnClick="btnAdd_Click" /><br />
            每頁<asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                <asp:ListItem Text="30" Value="30"></asp:ListItem>
                <asp:ListItem Text="50" Value="50"></asp:ListItem>
                <asp:ListItem Text="100" Value="100"></asp:ListItem>
            </asp:DropDownList>筆，查詢結果共[<asp:Label runat="server" ForeColor="Blue" ID="lbTotalCount"></asp:Label>]筆資料
            <asp:GridView ID="gv" runat="server"  AutoGenerateColumns="false" OnRowCommand="gvList_RowCommand"
                    AllowPaging="true" AllowCustomPaging="true" OnPageIndexChanging="gvList_PageIndexChanging" >
                <Columns>
                <asp:TemplateField HeaderText="管理">
                    <itemtemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="DataEdit" Text="編輯" CommandArgument='<%# Eval("VENDOR_ID") %>'></asp:LinkButton>
                        <asp:LinkButton ID="btnDel" runat="server" CommandName="DataDel" Text="刪除" OnClientClick="return confirm('是否刪除?');" CommandArgument='<%# Eval("VENDOR_ID") %>'></asp:LinkButton>
                    </itemtemplate>
                </asp:TemplateField>
                
                <asp:BoundField DataField="VENDOR_NAME" HeaderText="廠商名稱" />
                <asp:BoundField DataField="VENDOR_SHORT" HeaderText="簡稱" />
                    </Columns>
            </asp:GridView>
            
        </div>

    </form>
</body>
</html>
