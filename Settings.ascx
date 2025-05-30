<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="GIBS.Modules.GIBS_TideCharts.Settings" %>

<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="#" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
         
		<div class="dnnFormItem">
            <dnn:label ID="lblSearchName" runat="server" />
            <asp:TextBox ID="txtSearchName" runat="server" />
        </div> 
         <div class="dnnFormItem">
            <dnn:label ID="lblStationLookup" runat="server" suffix=":" />
             <asp:LinkButton ID="LinkButtonLookupStation" runat="server" OnClick="LinkButtonLookupStation_Click">Lookup Station</asp:LinkButton>
             &nbsp;&nbsp;<asp:Label ID="ShowMessage" runat="server" Text="Click link to search for a station" CssClass="messageError"></asp:Label><br />&nbsp;
        </div>		 
		 
<div class="dnnFormItem" id="divLocationSelect" runat="server" visible="false">
        <dnn:label ID="lblLocationSelect" runat="server" Text="Select Location" />
        <asp:DropDownList ID="CorrectLocation" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="CorrectLocation_SelectedIndexChanged" Visible="true">
        </asp:DropDownList>
    </div>
<!--  AUTO FILL THE FOLLOWING FIELDS BASED ON SELECTION -->
    <div class="dnnFormItem">
        <dnn:label ID="lblStationID" runat="server" Text="Station ID" HelpText="NOAA Station ID (e.g., 1612340)" ControlName="txtStationID" />
        <asp:TextBox ID="txtStationID" runat="server" />
    </div>

        <div class="dnnFormItem">
            <dnn:label ID="lblStationName" runat="server" />
            <asp:TextBox ID="txtStationName" runat="server" />
        </div>
		
        <div class="dnnFormItem">
            <dnn:label ID="lblState" runat="server" />
            <asp:TextBox ID="txtState" runat="server" />
        </div>

         <div class="dnnFormItem">
            <dnn:label ID="lblTimezonecorr" runat="server" />
            <asp:TextBox ID="txtTimezonecorr" runat="server" />
        </div>

        <div class="dnnFormItem">
            <dnn:label ID="lblLatitude" runat="server" />
            <asp:TextBox ID="txtLatitude" runat="server" />
        </div>
		<div class="dnnFormItem">
            <dnn:label ID="lblLongitude" runat="server" />
            <asp:TextBox ID="txtLongitude" runat="server" />
        </div>
		
	<div class="dnnFormItem">
        <dnn:label ID="lblShowLineChart" runat="server" ControlName="chkShowLineChart">
        </dnn:label>
        <asp:CheckBox ID="chkShowLineChart" runat="server" />
    </div>
	
    <div class="dnnFormItem">
        <dnn:label ID="lblShowBarChart" runat="server" ControlName="chkShowBarChart">
        </dnn:label>
        <asp:CheckBox ID="chkShowBarChart" runat="server" />
    </div>	

    <div class="dnnFormItem">
        <dnn:label ID="lblShowGridView" runat="server" ControlName="chkShowGridView">
        </dnn:label>
        <asp:CheckBox ID="chkShowGridView" runat="server" />
    </div>	
        
    <div class="dnnFormItem">
        <dnn:label ID="lblMapZoom" runat="server" ControlName="ddlMapZoom">
        </dnn:label>
        <asp:DropDownList ID="ddlMapZoom" runat="server">
            <asp:ListItem Text="1" Value="1"></asp:ListItem>
            <asp:ListItem Text="2" Value="2"></asp:ListItem>
            <asp:ListItem Text="3" Value="3"></asp:ListItem>
            <asp:ListItem Text="4" Value="4"></asp:ListItem>
            <asp:ListItem Text="5" Value="5"></asp:ListItem>
            <asp:ListItem Text="6" Value="6"></asp:ListItem>
            <asp:ListItem Text="7" Value="7"></asp:ListItem>
            <asp:ListItem Text="8" Value="8"></asp:ListItem>
            <asp:ListItem Text="9" Value="9"></asp:ListItem>
            <asp:ListItem Text="10" Value="10"></asp:ListItem>
            <asp:ListItem Text="11" Value="11"></asp:ListItem>
            <asp:ListItem Text="12" Value="12"></asp:ListItem>
            <asp:ListItem Text="13" Value="13"></asp:ListItem>
            <asp:ListItem Text="14" Value="14"></asp:ListItem>
            <asp:ListItem Text="15" Value="15"></asp:ListItem>
            <asp:ListItem Text="16" Value="16"></asp:ListItem>
            <asp:ListItem Text="17" Value="17"></asp:ListItem>
            <asp:ListItem Text="18" Value="18"></asp:ListItem>
        </asp:DropDownList>
    </div>

    <div class="dnnFormItem">
        <dnn:label ID="lblNumberOfTideDays" runat="server" ControlName="ddlNumberOfTideDays">
        </dnn:label>
        <asp:DropDownList ID="ddlNumberOfTideDays" runat="server">
            <asp:ListItem Text="1" Value="1"></asp:ListItem>
            <asp:ListItem Text="2" Value="2"></asp:ListItem>
            <asp:ListItem Text="3" Value="3"></asp:ListItem>
            <asp:ListItem Text="4" Value="4"></asp:ListItem>
            <asp:ListItem Text="5" Value="5"></asp:ListItem>
            <asp:ListItem Text="6" Value="6"></asp:ListItem>
            <asp:ListItem Text="7" Value="7"></asp:ListItem>
            <asp:ListItem Text="8" Value="8"></asp:ListItem>
            <asp:ListItem Text="9" Value="9"></asp:ListItem>
            <asp:ListItem Text="10" Value="10"></asp:ListItem>
            <asp:ListItem Text="11" Value="11"></asp:ListItem>
            <asp:ListItem Text="12" Value="12"></asp:ListItem>
            <asp:ListItem Text="13" Value="13"></asp:ListItem>
            <asp:ListItem Text="14" Value="14"></asp:ListItem>
            <asp:ListItem Text="15" Value="15"></asp:ListItem>
            <asp:ListItem Text="16" Value="16"></asp:ListItem>
            <asp:ListItem Text="17" Value="17"></asp:ListItem>
            <asp:ListItem Text="18" Value="18"></asp:ListItem>
			<asp:ListItem Text="19" Value="19"></asp:ListItem>
			<asp:ListItem Text="20" Value="20"></asp:ListItem>
            <asp:ListItem Text="21" Value="21"></asp:ListItem>
            <asp:ListItem Text="22" Value="22"></asp:ListItem>
            <asp:ListItem Text="23" Value="23"></asp:ListItem>
            <asp:ListItem Text="24" Value="24"></asp:ListItem>
            <asp:ListItem Text="25" Value="25"></asp:ListItem>
            <asp:ListItem Text="26" Value="26"></asp:ListItem>
            <asp:ListItem Text="27" Value="27"></asp:ListItem>
            <asp:ListItem Text="28" Value="28"></asp:ListItem>
			<asp:ListItem Text="29" Value="29"></asp:ListItem>
			<asp:ListItem Text="30" Value="30"></asp:ListItem>			
        </asp:DropDownList>
    </div>	

    </fieldset>