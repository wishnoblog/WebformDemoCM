<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CM_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>IT合約管理系統</h1>
        <div runat="server" id="areaEdit" visible="false">
            <table>
                <tr>
                    <td>對象流水號ID (關聯ZFCF_CM_VENDOR)</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbVENDOR_ID"></asp:TextBox></td>
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
                <asp:Button ID="btnAdd" Text="新增" runat="server" OnClick="btnAdd_Click" />
                <asp:GridView  ID="gvList" runat="server" AutoGenerateColumns="false" OnRowCommand="gvList_RowCommand"
                    AllowPaging="true" OnPageIndexChanging="gvList_PageIndexChanging" >
                    <Columns>
                        <asp:TemplateField HeaderText="管理">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="DataEdit" Text="編輯" CommandArgument='<%# Eval("CONTRACT_ID") %>' ></asp:LinkButton>
                                <asp:LinkButton ID="btnDel" runat="server" CommandName="DataDel" Text="刪除" OnClientClick="return confirm('是否刪除?');"  CommandArgument='<%# Eval("CONTRACT_ID") %>' ></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CONTRACT_ID" HeaderText="ID" />
                        <asp:BoundField DataField="INSTALLMENTS" HeaderText="付款期數(月)" />
                        <asp:BoundField DataField="TITLE" HeaderText="合約名稱" />
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
