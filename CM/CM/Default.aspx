<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CM_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>IT合約管理系統</h1>
        <a href="#">維護廠商資料</a>
        <div runat="server" id="areaEdit" visible="false">
            <table>
                <tr>
                    <td>合約對象</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlVENDOR_ID"></asp:DropDownList>
                        
                    </td>
                </tr>
                <tr>
                    <td>合約名稱</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbTITLE"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>SAP付款憑單單號</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbSAP_NBR"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>BPM簽呈編號</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbBPM_NBR"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>合約類型流水號 (關聯ZFCF_CM_CONTRACT_TYPE)</td>
                    <td>
                        <asp:RadioButtonList runat="server" id="rblCONTRACT_TYPE_ID" RepeatDirection="Horizontal" RepeatColumns="10"></asp:RadioButtonList>
                        <asp:TextBox runat="server" ID="tbCONTRACT_TYPE_ID"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>效期 (月)</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbTERM"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>合約起始時間</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbSTART_DATE"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>合約結束時間</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbEND_DATE"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>金額</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbAMOUNT"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>期數(月)</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbINSTALLMENTS"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>備註</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbNOTE" TextMode="MultiLine" Rows="6"></asp:TextBox></td>
                </tr>               
            </table>
            <asp:HiddenField ID="hid" runat="server" />
            <asp:HiddenField ID="hiMode" runat="server" />
            <asp:Button ID="btnSave" Text="存檔" runat="server" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" Text="取消" runat="server" OnClick="btnCancel_Click" />
        </div>
        <div id="areaList" runat="server">

            <div>
                BPM關鍵字 <asp:TextBox ID="tbBPM_Keyword" runat="server" placeHolder="請輸入關鍵字"></asp:TextBox>
                SAP關鍵字 <asp:TextBox ID="tbSAP_Keyword" runat="server" placeHolder="請輸入關鍵字"></asp:TextBox>
                SAP+BPM關鍵字 <asp:TextBox ID="tbSAP_BPM_Keyword" runat="server" placeHolder="請輸入關鍵字"></asp:TextBox><br/>
                金額<asp:TextBox ID="tbAMOUNT_START" runat="server" placeHolder=""></asp:TextBox>~<asp:TextBox ID="tbAMOUNT_END" runat="server" placeHolder=""></asp:TextBox>
                廠商名稱<asp:TextBox ID="tbVENDER_NAME_Keyword" runat="server" placeHolder="請輸入關鍵字"></asp:TextBox>
                <br />
                <asp:Button Text="搜尋" ID="btnSearch" OnClick="btnSearch_Click" runat="server" />
                <asp:Button Text="清除" ID="btnClear" OnClick="btnClear_Click" runat="server" />
            </div>
            <br />
            <div>
                <asp:Button ID="btnAdd" Text="新增" runat="server" OnClick="btnAdd_Click" /><br />
                每頁<asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                    <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                    <asp:ListItem Text="50" Value="50" ></asp:ListItem>
                    <asp:ListItem Text="100" Value="100" ></asp:ListItem>
                </asp:DropDownList>筆，查詢結果共[<asp:Label runat="server" ForeColor="Blue" ID="lbTotalCount"></asp:Label>]筆資料
                <asp:GridView  ID="gvList" runat="server" AutoGenerateColumns="false" OnRowCommand="gvList_RowCommand"
                    AllowPaging="true" AllowCustomPaging="true" OnPageIndexChanging="gvList_PageIndexChanging" >
                    <Columns>
                        <asp:TemplateField HeaderText="管理">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="DataEdit" Text="編輯" CommandArgument='<%# Eval("CONTRACT_ID") %>' ></asp:LinkButton>
                                <asp:LinkButton ID="btnDel" runat="server" CommandName="DataDel" Text="刪除" OnClientClick="return confirm('是否刪除?');"  CommandArgument='<%# Eval("CONTRACT_ID") %>' ></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>                        
                        <asp:BoundField DataField="TITLE" HeaderText="合約名稱" />
                        <asp:BoundField DataField="CONTRACT_CATEGORY" HeaderText="類型" />
                        <asp:BoundField DataField="VENDOR_SHORT" HeaderText="廠商" />
                        <asp:BoundField DataField="INSTALLMENTS" HeaderText="付款期數(月)" />                        
                        <asp:BoundField DataField="SAP_NBR" HeaderText="SAP" />
                        <asp:BoundField DataField="BPM_NBR" HeaderText="BPM" />
                        <asp:BoundField DataField="TERM" HeaderText="期數" />
                        <asp:BoundField DataField="START_DATE" DataFormatString="{0:yyyy年MM月dd日}" HeaderText="起" />
                        <asp:BoundField DataField="END_DATE"  DataFormatString="{0:yyyy年MM月dd日}" HeaderText="迄" />
                        
                        <asp:BoundField DataField="AMOUNT" HeaderText="金額" />
                        
                        
                    </Columns>

                </asp:GridView>                
            </div>
        </div>
    </form>
</body>
</html>
