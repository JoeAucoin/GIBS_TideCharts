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

namespace GIBS.Modules.GIBS_TideCharts
{
    /// <summary>
    /// Base class for the settings of the GIBS Tide Charts Module.
    /// This class will hold the selected weather station's essential information.
    /// </summary>
    public class GIBS_TideChartsModuleSettingsBase : ModuleSettingsBase
    {
        /// <summary>
        /// Gets or sets the ID of the selected NOAA weather station.
        /// Corresponds to the 'id' field in the NOAA stations API.
        /// </summary>
       
       

        public string StationID
        {
            get
            {
                if (Settings.Contains("StationID"))
                    return Settings["StationID"].ToString();
                return "";
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "StationID", value.ToString());
            }
        }

        public string StationName
        {
            get
            {
                if (Settings.Contains("StationName"))
                    return Settings["StationName"].ToString();
                return "";
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "StationName", value.ToString());
            }
        }
        /// <summary>
        /// Gets or sets the latitude of the selected NOAA weather station.
        /// Corresponds to the 'lat' field in the NOAA stations API.
        /// </summary>

        public string StationLatitude
        {
            get
            {
                if (Settings.Contains("StationLatitude"))
                    return Settings["StationLatitude"].ToString();
                return "";
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "StationLatitude", value.ToString());
            }
        }
        /// <summary>
        /// Gets or sets the longitude of the selected NOAA weather station.
        /// Corresponds to the 'lng' field in the NOAA stations API.
        /// </summary>


        public string StationLongitude
        {
            get
            {
                if (Settings.Contains("StationLongitude"))
                    return Settings["StationLongitude"].ToString();
                return "";
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "StationLongitude", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected NOAA weather station.
        /// Corresponds to the 'state' field in the NOAA stations API.
        /// </summary>
       

        public string StationState
        {
            get
            {
                if (Settings.Contains("StationState"))
                    return Settings["StationState"].ToString();
                return "";
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "StationState", value.ToString());
            }
        }
        /// <summary>
        /// Gets or sets the timezone correction of the selected NOAA weather station.
        /// Corresponds to the 'timezonecorr' field in the NOAA stations API.
        /// </summary>
      

        public string StationTimezoneCorrection
        {
            get
            {
                if (Settings.Contains("StationTimezoneCorrection"))
                    return Settings["StationTimezoneCorrection"].ToString();
                return "";
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "StationTimezoneCorrection", value.ToString());
            }
        }

        public bool ShowLineChart
        {
            get
            {
                if (Settings.Contains("ShowLineChart"))
                    return Convert.ToBoolean(Settings["ShowLineChart"]);
                return false;
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "ShowLineChart", value.ToString());
            }
        }

        public bool ShowBarChart
        {
            get
            {
                if (Settings.Contains("ShowBarChart"))
                    return Convert.ToBoolean(Settings["ShowBarChart"]);
                return false;
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "ShowBarChart", value.ToString());
            }
        }


        public bool ShowGridView
        {
            get
            {
                if (Settings.Contains("ShowGridView"))
                    return Convert.ToBoolean(Settings["ShowGridView"]);
                return false;
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "ShowGridView", value.ToString());
            }
        }

        public int MapZoom
        {
            get
            {
                if (Settings.Contains("MapZoom"))
                    return Convert.ToInt16(Settings["MapZoom"]);
                return 12;
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "MapZoom", value.ToString());
            }
        }



        public int NumberOfTideDays
        {
            get
            {
                if (Settings.Contains("NumberOfTideDays"))
                    return Convert.ToInt16(Settings["NumberOfTideDays"]);
                return 7;
            }
            set
            {
                var mc = new ModuleController();
                mc.UpdateTabModuleSetting(TabModuleId, "NumberOfTideDays", value.ToString());
            }
        }

    }
}