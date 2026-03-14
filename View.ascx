<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="GIBS.Modules.GIBS_TideCharts.View" %>
<%-- Leaflet.js CSS for the map (LOCAL) --%>
<link rel="stylesheet" href="<%= ControlPath %>css/leaflet.css" />

<asp:Literal ID="litMessage" runat="server"></asp:Literal>

<div id="divTideChart" runat="server" visible="false">
    
    <div class="station-details-container" style="display: flex; flex-wrap: wrap; align-items: flex-start; margin-bottom: 20px;">
        <div class="station-info" style="flex: 1; min-width: 300px; padding-right: 20px;">
            <h3 id="h3StationName" runat="server"></h3>
            <p>
                <strong>Station ID:</strong> <asp:Literal ID="litStationID" runat="server"></asp:Literal><br />
                <strong>Coordinates:</strong> <asp:Literal ID="litCoordinates" runat="server"></asp:Literal><br />
                <strong>Time Zone:</strong> <asp:Literal ID="litTimeZone" runat="server"></asp:Literal>
            </p>

 <h4 style="text-align:center;">Weekly Tide Predictions<br />(<asp:Literal ID="litDateRange" runat="server"></asp:Literal>)</h4>

        </div>
        <%-- Map Container (Ensure you've applied the ModuleId to the ID for uniqueness) --%>
        <div id="tideMap<%= ModuleId %>" style="flex: 1; min-width: 300px; height: 300px; border: 1px solid #ccc; background-color: #f0f0f0;"></div>
    </div>

   

    <div id="lineChartContainerDiv" runat="server" class="chart-container" style="height:440px; width:100%; margin-bottom: 40px; margin-top:20px;">
        <h3>Tide Levels</h3>
        <canvas id="tideChartCanvas<%= ModuleId %>" style="height:400px; width:100%;"></canvas>
    </div>

    <div id="barChartContainerDiv" runat="server" class="chart-container" style="height:440px; width:100%; margin-bottom: 40px; margin-top:20px;">
        <h3>Tide Levels</h3>
        <canvas id="tideBarChartCanvas<%= ModuleId %>" style="height:400px; width:100%;"></canvas>
    </div>
    <div>&nbsp;</div>
  <div class="card" id="divGridViewTidePredictions" runat="server">
    <div class="card-header" id="headingTidePredictions<%= ModuleId %>">
        <h4 class="mb-0">
            <button type="button" class="btn btn-lg btn-link text-decoration-none text-start w-100 p-0" data-bs-toggle="collapse" data-bs-target="#collapseTidePredictions<%= ModuleId %>" aria-expanded="false" aria-controls="collapseTidePredictions<%= ModuleId %>">
                <span class="arrow">&#9660;</span> <%= StationName %>: Tide Prediction Details
            </button>
        </h4>
    </div>
    <div id="collapseTidePredictions<%= ModuleId %>" class="collapse" role="tabpanel" aria-labelledby="headingTidePredictions<%= ModuleId %>">
        <div class="card-body">
            <asp:GridView ID="gvTidePredictions" runat="server"
                AutoGenerateColumns="false"
                CssClass="dnnGrid table table-striped table-bordered" 
                HeaderStyle-CssClass="dnnGridHeader"
                RowStyle-CssClass="dnnGridItem"
                AlternatingRowStyle-CssClass="dnnGridAltItem"
                EmptyDataText="No tide predictions available for this period."
                Visible="false" 
                HorizontalAlign="Center">
                <Columns>
                    <asp:TemplateField HeaderText="Tide">
                        <ItemTemplate>
                            <%# Eval("type").ToString() == "H" ? "High Tide" : (Eval("type").ToString() == "L" ? "Low Tide" : "") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PredictedDateTime" HeaderText="Date/Time (LST/LDT)" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" />
                    <asp:BoundField DataField="v" HeaderText="Prediction (Feet)" DataFormatString="{0:F2}" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="type" HeaderText="Type" ItemStyle-HorizontalAlign="Center" Visible="false" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>

<div class="tcCredits"><cite>Tide data provided by <a href="https://tidesandcurrents.noaa.gov/" target="_blank">NOAA</a></cite></div>

</div>

<%-- Leaflet.js JavaScript for the map (LOCAL, place at bottom for better performance) --%>
<script src="<%= ControlPath %>js/leaflet.js"></script>