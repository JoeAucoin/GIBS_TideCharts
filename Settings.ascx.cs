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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace GIBS.Modules.GIBS_TideCharts
{
    // Helper classes to deserialize JSON
    public class NOAAStation
    {
        public string id { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string state { get; set; }
        public int timezonecorr { get; set; }

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(state))
                {
                    return $"{name}, {state}";
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
                else if (!string.IsNullOrEmpty(id))
                {
                    return id;
                }
                return "[Unknown Station]";
            }
        }
    }

    public class NOAAStationsResponse
    {
        public List<NOAAStation> stations { get; set; }
    }

    public partial class Settings : GIBS_TideChartsModuleSettingsBase
    {
        private const string NOAA_STATIONS_API_URL = "https://api.tidesandcurrents.noaa.gov/mdapi/prod/webapi/stations.json";

        #region Base Method Implementations

        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    // If a StationId is saved, load its details into the textboxes.
                    if (!string.IsNullOrEmpty(this.StationID))
                    {
                        txtStationID.Text = this.StationID; // Load StationId into its textbox
                        txtStationName.Text = this.StationName;
                        txtState.Text = this.StationState;
                        txtTimezonecorr.Text = this.StationTimezoneCorrection.ToString();
                        txtLatitude.Text = this.StationLatitude.ToString();
                        txtLongitude.Text = this.StationLongitude.ToString();
                        chkShowBarChart.Checked = this.ShowBarChart;
                        chkShowLineChart.Checked = this.ShowLineChart;
                        chkShowGridView.Checked = this.ShowGridView;
                        divLocationSelect.Visible = false; // Keep dropdown hidden
                        ddlMapZoom.SelectedValue = this.MapZoom.ToString();
                        ddlNumberOfTideDays.SelectedValue = this.NumberOfTideDays.ToString();
                        
                        ShowMessage.Text = $"Search for a new station if needed.";
                    }
                    else
                    {
                        // No station saved, clear fields, hide dropdown and prompt for search.
                        ClearStationFields(); // Ensure fields are empty, including txtStationID
                        divLocationSelect.Visible = false;
                        ShowMessage.Text = "Enter a search term and click 'Lookup Station' to find a tide station.";
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public override void UpdateSettings()
        {
            try
            {
                var modules = new ModuleController();

                // Save values directly from all textboxes.
                modules.UpdateModuleSetting(ModuleId, "StationID", txtStationID.Text); // Save from txtStationID
                modules.UpdateModuleSetting(ModuleId, "StationName", txtStationName.Text);
                modules.UpdateModuleSetting(ModuleId, "StationState", txtState.Text);
                modules.UpdateModuleSetting(ModuleId, "StationTimezoneCorrection", txtTimezonecorr.Text);
                modules.UpdateModuleSetting(ModuleId, "StationLatitude", txtLatitude.Text);
                modules.UpdateModuleSetting(ModuleId, "StationLongitude", txtLongitude.Text);
                modules.UpdateModuleSetting(ModuleId, "ShowLineChart", chkShowLineChart.Checked.ToString());
                modules.UpdateModuleSetting(ModuleId, "ShowBarChart", chkShowBarChart.Checked.ToString());
                modules.UpdateModuleSetting(ModuleId, "ShowGridView", chkShowGridView.Checked.ToString());
                modules.UpdateModuleSetting(ModuleId, "MapZoom", ddlMapZoom.SelectedValue);
                modules.UpdateModuleSetting(ModuleId, "NumberOfTideDays", ddlNumberOfTideDays.SelectedValue);
                //  ShowMessage.Text = "Station settings saved.";
                //   this.ModuleConfiguration.ClearCache();
            }
            catch (Exception exc)
            {
                ShowMessage.Text = $"An error occurred while saving settings: {exc.Message}";
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Event Handlers

        protected async void LinkButtonLookupStation_Click(object sender, EventArgs e)
        {
            ShowMessage.Text = string.Empty;
            CorrectLocation.Items.Clear(); // Clear dropdown from previous searches/loads
            divLocationSelect.Visible = false; // Hide dropdown initially
            ClearStationFields(); // Clear all fields including txtStationID

            string searchName = txtSearchName.Text.Trim();

            if (string.IsNullOrEmpty(searchName))
            {
                ShowMessage.Text = "Please enter a search term for the station name.";
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    ShowMessage.Text = "Searching for stations...";
                    string json = await client.GetStringAsync(NOAA_STATIONS_API_URL);

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    NOAAStationsResponse response = serializer.Deserialize<NOAAStationsResponse>(json);

                    if (response != null && response.stations != null)
                    {
                        var filteredStations = response.stations
                            .Where(s => s.name.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) >= 0)
                            .OrderBy(s => s.name)
                            .ToList();

                        if (filteredStations.Count == 1)
                        {
                            // Only one station found, auto-populate and hide dropdown
                            NOAAStation singleStation = filteredStations.First();
                            PopulateStationFields(singleStation); // Fills all textboxes including txtStationID
                            ShowMessage.Text = $"Found '{singleStation.DisplayName}' and auto-filled details.";

                            divLocationSelect.Visible = false; // Ensure dropdown remains hidden

                        }
                        else if (filteredStations.Any())
                        {
                            // Multiple stations found, show dropdown
                            CorrectLocation.DataSource = filteredStations;
                            CorrectLocation.DataTextField = "DisplayName";
                            CorrectLocation.DataValueField = "id";
                            CorrectLocation.DataBind();
                            CorrectLocation.Items.Insert(0, new ListItem("-- Select a Station --", string.Empty));

                            divLocationSelect.Visible = true; // Show the dropdown
                            ShowMessage.Text = $"{filteredStations.Count} stations found. Please select one from the dropdown.";
                        }
                        else
                        {
                            ShowMessage.Text = "No stations found matching your search. Please try a different name.";
                            divLocationSelect.Visible = false;
                        }
                    }
                    else
                    {
                        ShowMessage.Text = "Failed to retrieve station data from NOAA API or no stations found in response.";
                        divLocationSelect.Visible = false;
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                ShowMessage.Text = $"Error connecting to NOAA API: {httpEx.Message}";
                Exceptions.ProcessModuleLoadException(this, httpEx);
            }
            catch (Exception ex)
            {
                ShowMessage.Text = $"An unexpected error occurred during station lookup: {ex.Message}";
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected async void CorrectLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowMessage.Text = string.Empty;

            string selectedStationId = CorrectLocation.SelectedValue;

            if (string.IsNullOrEmpty(selectedStationId))
            {
                ClearStationFields(); // Clears all fields including txtStationID
                ShowMessage.Text = "Please select a station from the list.";
                return;
            }

            string stationDetailUrl = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    stationDetailUrl = $"https://api.tidesandcurrents.noaa.gov/mdapi/prod/webapi/stations/{selectedStationId}.json";
                    ShowMessage.Text = $"Fetching details for '{selectedStationId}'...";

                    string json = await client.GetStringAsync(stationDetailUrl);

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    NOAAStationsResponse singleStationResponse = serializer.Deserialize<NOAAStationsResponse>(json);
                    NOAAStation selectedStation = null;

                    if (singleStationResponse != null && singleStationResponse.stations != null && singleStationResponse.stations.Any())
                    {
                        selectedStation = singleStationResponse.stations.First();
                    }

                    if (selectedStation != null)
                    {
                        PopulateStationFields(selectedStation); // Fills all textboxes including txtStationID
                        ShowMessage.Text = $"Station '{selectedStation.DisplayName}' details loaded successfully.";
                    }
                    else
                    {
                        ClearStationFields(); // Clears all fields including txtStationID
                        ShowMessage.Text = "Could not retrieve details for the selected station. Please try again.";
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                ClearStationFields(); // Clears all fields including txtStationID
                ShowMessage.Text = $"Error connecting to NOAA API for details: {httpEx.Message}. URL: {stationDetailUrl}";
                Exceptions.ProcessModuleLoadException(this, httpEx);
            }
            catch (Exception ex)
            {
                ClearStationFields(); // Clears all fields including txtStationID
                ShowMessage.Text = $"An error occurred while loading station details: {ex.Message}";
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// Populates the textboxes with the properties of a given NOAAStation object.
        /// </summary>
        /// <param name="station"></param>
        private void PopulateStationFields(NOAAStation station)
        {
            txtStationID.Text = station.id; // Populate the new txtStationID
            txtStationName.Text = station.name;
            txtState.Text = station.state;
            txtTimezonecorr.Text = station.timezonecorr.ToString();
            txtLatitude.Text = station.lat.ToString();
            txtLongitude.Text = station.lng.ToString();
        }

        /// <summary>
        /// Clears all station-related textboxes.
        /// </summary>
        private void ClearStationFields()
        {
            txtStationID.Text = string.Empty; // Clear the new txtStationID
            txtStationName.Text = string.Empty;
            txtState.Text = string.Empty;
            txtTimezonecorr.Text = string.Empty;
            txtLatitude.Text = string.Empty;
            txtLongitude.Text = string.Empty;
        }

        /// <summary>
        /// Attempts to populate the dropdown and select the saved station.
        /// This method is now only implicitly used for binding when the dropdown is to be displayed (i.e., after a search with multiple results).
        /// It's no longer called on initial LoadSettings if a station is already saved.
        /// </summary>
        /// <param name="stationIdToSelect"></param>
        /// <returns></returns>
      

        #endregion
    }
}