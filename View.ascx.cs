/*
' Copyright (c) 2025 gibs.com
' All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Net.Http;
using System.Collections.Generic;
using System.Globalization; // For DateTime parsing and double.Parse
using System.Linq; // For LINQ operations
using System.Web.Script.Serialization; // For JavaScriptSerializer
using System.Text; // For StringBuilder

namespace GIBS.Modules.GIBS_TideCharts
{
    // Define classes to match the NOAA API JSON structure for predictions
    public class Prediction
    {
        public string t { get; set; } // Time string from API (e.g., "2025-05-26 12:00")
        public string v { get; set; } // Value string from API (e.g., "5.213")
        public string type { get; set; } // H for High, L for Low

        // Helper properties to convert string data to actual types for Chart.js
        public DateTime PredictedDateTime => DateTime.ParseExact(t, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        public double PredictedValue => double.Parse(v, CultureInfo.InvariantCulture);
    }

    public class PredictionsResponse
    {
        public List<Prediction> predictions { get; set; }
    }

    public partial class View : GIBS_TideChartsModuleSettingsBase // Inherit from your settings base class
    {
        private const string NOAA_API_BASE_URL = "https://api.tidesandcurrents.noaa.gov/api/prod/datagetter";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "ChartJS", "https://cdn.jsdelivr.net/npm/chart.js");
            // Leaflet CSS and JS are now handled directly in View.ascx markup, so no need to register them here.
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (string.IsNullOrEmpty(this.StationID))
                    {
                        litMessage.Text = "<p class='dnnFormMessage dnnFormValidationSummary'>Please configure the Tide Chart module settings (Station ID is missing).</p>";
                        divTideChart.Visible = false; // Hide chart area
                    }
                    else
                    {
                        litMessage.Text = ""; // Clear any previous messages
                        LoadTideChartData();
                    }
                }


            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                litMessage.Text = "<p class='dnnFormMessage dnnFormValidationSummary'>An error occurred while loading tide data. Please try again later.</p>";
                divTideChart.Visible = false;
            }
        }

        private void LoadTideChartData()
        {
            litMessage.Text = "<p>Loading tide chart data...</p>";
            divTideChart.Visible = false; // Keep hidden until data is ready

            try
            {
                // Define date range for a week
                DateTime startDate = DateTime.Today;
                DateTime endDate = startDate.AddDays(this.NumberOfTideDays); // Get From Settings

                // Format dates asYYYYMMdd HH:mm (NOAA API expects this)
                string beginDateFormatted = startDate.ToString("yyyyMMdd HH:mm", CultureInfo.InvariantCulture);
                string endDateFormatted = endDate.ToString("yyyyMMdd HH:mm", CultureInfo.InvariantCulture);

                string apiUrl = $"{NOAA_API_BASE_URL}?" +
                                $"product=predictions&" +
                                $"datum=MLLW&" +
                                $"station={this.StationID}&" +
                                $"begin_date={beginDateFormatted}&" +
                                $"end_date={endDateFormatted}&" +
                                $"interval=hilo&" +
                                $"time_zone=lst&" +
                                $"units=english&" +
                                $"application=GIBS&" +
                                $"format=json";

                using (HttpClient client = new HttpClient())
                {
                    string jsonResponse = client.GetStringAsync(apiUrl).Result;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    PredictionsResponse response = serializer.Deserialize<PredictionsResponse>(jsonResponse);

                    if (response?.predictions != null && response.predictions.Any())
                    {
                        // Populate basic station info
                        h3StationName.InnerText = !string.IsNullOrEmpty(this.StationName) ? this.StationName + ", " + this.StationState : "Tide Station";
                        litStationID.Text = this.StationID;

                        // Parse coordinates to double before passing to GenerateMapScript
                        double latitude = 0;
                        double longitude = 0;

                        if (!string.IsNullOrEmpty(this.StationLatitude) && !string.IsNullOrEmpty(this.StationLongitude))
                        {
                            if (double.TryParse(this.StationLatitude, NumberStyles.Any, CultureInfo.InvariantCulture, out latitude) &&
                                double.TryParse(this.StationLongitude, NumberStyles.Any, CultureInfo.InvariantCulture, out longitude))
                            {
                                litCoordinates.Text = $"{latitude}, {longitude}"; // Display parsed values
                            }
                            else
                            {
                                litCoordinates.Text = "Invalid Coordinates";
                                Exceptions.LogException(new FormatException("Failed to parse StationLatitude or StationLongitude to double."));
                            }
                        }
                        else
                        {
                            litCoordinates.Text = "Coordinates not set";
                        }

                        litTimeZone.Text = "LST/LDT";

                        // Populate date range for display
                        litDateRange.Text = $"{startDate.ToString("MM/dd/yyyy")} - {endDate.ToString("MM/dd/yyyy")}";

                        if(this.ShowGridView)
                        {
                            gvTidePredictions.DataSource = response.predictions;
                            gvTidePredictions.DataBind();
                            gvTidePredictions.Visible = true; // Ensure GridView is visible
                            divGridViewTidePredictions.Visible = true;
                        }
                        else
                        {
                            divGridViewTidePredictions.Visible = false;
                            gvTidePredictions.Visible = false;
                        }

                        // Generate Chart.js data for BOTH charts
                        GenerateTideChartScript(response.predictions, this.ModuleId);

                        // Generate Map Script - NOW PASSING DOUBLES
                        GenerateMapScript(latitude, longitude, this.ModuleId);

                        litMessage.Text = string.Empty; // Clear loading message
                        divTideChart.Visible = true; // Show the chart area
                    }
                    else
                    {
                        litMessage.Text = $"<p class='dnnFormMessage dnnFormValidationSummary'>No tide predictions found for Station ID: {this.StationID} for the specified period. The station might not provide 'hilo' predictions, or there was no data.</p>";
                        divTideChart.Visible = false;
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                litMessage.Text = $"<p class='dnnFormMessage dnnFormValidationSummary'>Error connecting to NOAA Tide API: {httpEx.Message}. Please check your internet connection or try again later.</p>";
                Exceptions.LogException(httpEx);
            }
            catch (Exception ex)
            {
                litMessage.Text = $"<p class='dnnFormMessage dnnFormValidationSummary'>An unexpected error occurred while fetching or parsing tide data: {ex.Message}.</p>";
                Exceptions.LogException(ex);
            }
        }

        private void GenerateTideChartScript(List<Prediction> predictions, int moduleId)
        {
            // Only generate script if at least one chart type is enabled
            if (!this.ShowLineChart && !this.ShowBarChart)
            {
                return; // No charts to show, exit
            }

            var labels = new StringBuilder();
            var data = new StringBuilder();
            var pointColors = new StringBuilder();
            // New StringBuilder for full dates for the tooltip
            var fullDatesForTooltip = new StringBuilder();

            foreach (var p in predictions.OrderBy(p => p.PredictedDateTime))
            {
                // Labels for the X-axis (shorter format)
                labels.Append($"'{p.PredictedDateTime.ToString("ddd hh:mm tt")}',");
                // Data values for the Y-axis
                data.Append($"{p.PredictedValue},");

                // Full date/time for the tooltip (e.g., "Tues 5/27 12:05 PM")
                fullDatesForTooltip.Append($"'{p.PredictedDateTime.ToString("ddd M/dd hh:mm tt")}',");

                // Determine point/bar color based on tide type
                if (p.type == "H")
                {
                    pointColors.Append("'rgba(255, 99, 132, 1)',"); // Red for High
                }
                else if (p.type == "L")
                {
                    pointColors.Append("'rgba(54, 162, 235, 1)',"); // Blue for Low
                }
                else
                {
                    pointColors.Append("'rgba(75, 192, 192, 1)',"); // Default color
                }
            }

            // Remove trailing commas
            if (labels.Length > 0) labels.Length--;
            if (data.Length > 0) data.Length--;
            if (pointColors.Length > 0) pointColors.Length--;
            if (fullDatesForTooltip.Length > 0) fullDatesForTooltip.Length--;

            string uniqueLineCanvasId = $"tideChartCanvas{moduleId}";
            string uniqueBarCanvasId = $"tideBarChartCanvas{moduleId}";

            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("<script type='text/javascript'>");
            scriptBuilder.AppendLine("    document.addEventListener('DOMContentLoaded', function () {");

            // --- Line Chart Configuration (Conditional) ---
            if (this.ShowLineChart)
            {
                scriptBuilder.AppendLine($"        var lineCtx = document.getElementById('{uniqueLineCanvasId}');");
                scriptBuilder.AppendLine("        if (lineCtx) {");
                scriptBuilder.AppendLine("            var tideLineChart = new Chart(lineCtx, {");
                scriptBuilder.AppendLine("                type: 'line',");
                scriptBuilder.AppendLine("                data: {");
                scriptBuilder.AppendLine($"                    labels: [{labels}],");
                scriptBuilder.AppendLine("                    datasets: [{");
                scriptBuilder.AppendLine("                        label: 'Tide Prediction',");
                scriptBuilder.AppendLine($"                        data: [{data}],");
                scriptBuilder.AppendLine("                        borderColor: 'rgba(75, 192, 192, 1)',");
                scriptBuilder.AppendLine("                        backgroundColor: 'rgba(75, 192, 192, 0.2)',");
                scriptBuilder.AppendLine("                        borderWidth: 1,");
                scriptBuilder.AppendLine("                        pointRadius: 5,");
                scriptBuilder.AppendLine($"                        pointBackgroundColor: [{pointColors}],");
                scriptBuilder.AppendLine("                        pointBorderColor: '#fff',");
                scriptBuilder.AppendLine("                        pointHoverRadius: 7,");
                scriptBuilder.AppendLine("                        fill: false");
                scriptBuilder.AppendLine("                    }]");
                scriptBuilder.AppendLine("                },");
                scriptBuilder.AppendLine("                options: {");
                scriptBuilder.AppendLine("                    responsive: true,");
                scriptBuilder.AppendLine("                    maintainAspectRatio: false,");
                scriptBuilder.AppendLine("                    scales: {");
                scriptBuilder.AppendLine("                        x: {");
                scriptBuilder.AppendLine("                            title: {");
                scriptBuilder.AppendLine("                                display: true,");
                scriptBuilder.AppendLine("                                text: 'Date/Time'");
                scriptBuilder.AppendLine("                            },");
                scriptBuilder.AppendLine("                            ticks: {");
                scriptBuilder.AppendLine("                                autoSkip: true,");
                scriptBuilder.AppendLine("                                maxTicksLimit: 10");
                scriptBuilder.AppendLine("                            }");
                scriptBuilder.AppendLine("                        },");
                scriptBuilder.AppendLine("                        y: {");
                scriptBuilder.AppendLine("                            beginAtZero: false,");
                scriptBuilder.AppendLine("                            title: {");
                scriptBuilder.AppendLine("                                display: true,");
                scriptBuilder.AppendLine("                                text: 'Water Level'");
                scriptBuilder.AppendLine("                            }");
                scriptBuilder.AppendLine("                        }");
                scriptBuilder.AppendLine("                    },");
                scriptBuilder.AppendLine("                    plugins: {");
                scriptBuilder.AppendLine("                        tooltip: {");
                scriptBuilder.AppendLine("                            callbacks: {");
                scriptBuilder.AppendLine("                                label: function(context) {");
                scriptBuilder.AppendLine("                                    var value = context.parsed.y.toFixed(2);");
                scriptBuilder.AppendLine("                                    var index = context.dataIndex;");
                scriptBuilder.AppendLine($"                                    var dates = [{fullDatesForTooltip}];"); // Use full dates here
                scriptBuilder.AppendLine($"                                    var types = {new JavaScriptSerializer().Serialize(predictions.Select(p => p.type).ToArray())};");
                scriptBuilder.AppendLine("                                    var type = types[index];");
                scriptBuilder.AppendLine("                                    var tideTypeLabel = '';");
                scriptBuilder.AppendLine("                                    if (type === 'H') {");
                scriptBuilder.AppendLine("                                        tideTypeLabel = 'High Tide';");
                scriptBuilder.AppendLine("                                    } else if (type === 'L') {");
                scriptBuilder.AppendLine("                                        tideTypeLabel = 'Low Tide';");
                scriptBuilder.AppendLine("                                    } else {");
                scriptBuilder.AppendLine("                                        tideTypeLabel = 'Tide'; ");
                scriptBuilder.AppendLine("                                    }");
                scriptBuilder.AppendLine("                                    return [dates[index], tideTypeLabel + ': ' + value + ' ft'];"); // Return an array for multi-line tooltip
                scriptBuilder.AppendLine("                                }");
                scriptBuilder.AppendLine("                            }");
                scriptBuilder.AppendLine("                        }");
                scriptBuilder.AppendLine("                    }");
                scriptBuilder.AppendLine("                }");
                scriptBuilder.AppendLine("            });");
                scriptBuilder.AppendLine("        }");
            }
            else
            {
                lineChartContainerDiv.Visible = false;
            }

            // --- Bar Chart Configuration (Conditional) ---
            if (this.ShowBarChart)
            {
                scriptBuilder.AppendLine($"        var barCtx = document.getElementById('{uniqueBarCanvasId}');");
                scriptBuilder.AppendLine("        if (barCtx) {");
                scriptBuilder.AppendLine("            var tideBarChart = new Chart(barCtx, {");
                scriptBuilder.AppendLine("                type: 'bar',");
                scriptBuilder.AppendLine("                data: {");
                scriptBuilder.AppendLine($"                    labels: [{labels}],");
                scriptBuilder.AppendLine("                    datasets: [{");
                scriptBuilder.AppendLine("                        label: 'Tide Prediction',");
                scriptBuilder.AppendLine($"                        data: [{data}],");
                scriptBuilder.AppendLine($"                        backgroundColor: [{pointColors}],");
                scriptBuilder.AppendLine($"                        borderColor: [{pointColors}],");
                scriptBuilder.AppendLine("                        borderWidth: 1");
                scriptBuilder.AppendLine("                    }]");
                scriptBuilder.AppendLine("                },");
                scriptBuilder.AppendLine("                options: {");
                scriptBuilder.AppendLine("                    responsive: true,");
                scriptBuilder.AppendLine("                    maintainAspectRatio: false,");
                scriptBuilder.AppendLine("                    scales: {");
                scriptBuilder.AppendLine("                        x: {");
                scriptBuilder.AppendLine("                            title: {");
                scriptBuilder.AppendLine("                                display: true,");
                scriptBuilder.AppendLine("                                text: 'Date/Time'");
                scriptBuilder.AppendLine("                            },");
                scriptBuilder.AppendLine("                            ticks: {");
                scriptBuilder.AppendLine("                                autoSkip: true,");
                scriptBuilder.AppendLine("                                maxTicksLimit: 10");
                scriptBuilder.AppendLine("                            }");
                scriptBuilder.AppendLine("                        },");
                scriptBuilder.AppendLine("                        y: {");
                scriptBuilder.AppendLine("                            beginAtZero: false,");
                scriptBuilder.AppendLine("                            title: {");
                scriptBuilder.AppendLine("                                display: true,");
                scriptBuilder.AppendLine("                                text: 'Water Level'");
                scriptBuilder.AppendLine("                            }");
                scriptBuilder.AppendLine("                        }");
                scriptBuilder.AppendLine("                    },");
                scriptBuilder.AppendLine("                    plugins: {");
                scriptBuilder.AppendLine("                        tooltip: {");
                scriptBuilder.AppendLine("                            callbacks: {");
                scriptBuilder.AppendLine("                                label: function(context) {");
                scriptBuilder.AppendLine("                                    var value = context.parsed.y.toFixed(2);");
                scriptBuilder.AppendLine("                                    var index = context.dataIndex;");
                scriptBuilder.AppendLine($"                                    var dates = [{fullDatesForTooltip}];"); // Use full dates here
                scriptBuilder.AppendLine($"                                    var types = {new JavaScriptSerializer().Serialize(predictions.Select(p => p.type).ToArray())};");
                scriptBuilder.AppendLine("                                    var type = types[index];");
                scriptBuilder.AppendLine("                                    var tideTypeLabel = '';");
                scriptBuilder.AppendLine("                                    if (type === 'H') {");
                scriptBuilder.AppendLine("                                        tideTypeLabel = 'High Tide';");
                scriptBuilder.AppendLine("                                    } else if (type === 'L') {");
                scriptBuilder.AppendLine("                                        tideTypeLabel = 'Low Tide';");
                scriptBuilder.AppendLine("                                    } else {");
                scriptBuilder.AppendLine("                                        tideTypeLabel = 'Tide'; ");
                scriptBuilder.AppendLine("                                    }");
                scriptBuilder.AppendLine("                                    return [dates[index], tideTypeLabel + ': ' + value + ' ft'];"); // Return an array for multi-line tooltip
                scriptBuilder.AppendLine("                                }");
                scriptBuilder.AppendLine("                            }");
                scriptBuilder.AppendLine("                        }");
                scriptBuilder.AppendLine("                    }");
                scriptBuilder.AppendLine("                }");
                scriptBuilder.AppendLine("            });");
                scriptBuilder.AppendLine("        }");
            }
            else
            {
                barChartContainerDiv.Visible = false;
            }

            scriptBuilder.AppendLine("    });");
            scriptBuilder.AppendLine("</script>");

            Page.ClientScript.RegisterStartupScript(this.GetType(), $"TideChartsScript_{moduleId}", scriptBuilder.ToString(), false);
        }

        private void GenerateMapScript(double latitude, double longitude, int moduleId)
        {
            string mapDivId = $"tideMap{moduleId}"; // The unique ID of the map container div

            // Resolve the correct client-side URL for Leaflet's images folder
            string leafletImagePath = ResolveUrl($"{ControlPath}images/");

            string mapScript = $@"
                <script type='text/javascript'>
                    document.addEventListener('DOMContentLoaded', function () {{
                        var mapElement = document.getElementById('{mapDivId}');
                        if (mapElement) {{
                            // Override Leaflet's default icon paths to point to your local images folder
                            L.Icon.Default.imagePath = '{leafletImagePath}';

                            // Initialize map centered on the station coordinates with a zoom level from settings
                            var map = L.map('{mapDivId}').setView([{latitude}, {longitude}], {this.MapZoom}); 

                            // Add OpenStreetMap tiles
                            L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                                maxZoom: 19,
                                attribution: '&copy; <a href=""http://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors'
                            }}).addTo(map);

                            // Add a marker at the station location
                            L.marker([{latitude}, {longitude}]).addTo(map)
                                .bindPopup('Tide Station: {h3StationName.InnerText}') // Use the station name as the popup text
                                .openPopup(); // Automatically open the popup on load
                        }}
                    }});
                </script>
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), $"TideMapScript_{moduleId}", mapScript, false);
        }
    }
}