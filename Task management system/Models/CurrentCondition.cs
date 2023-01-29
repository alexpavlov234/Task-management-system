using Newtonsoft.Json.Linq;
using Syncfusion.Blazor.Maps;
using System.Xml.Linq;

namespace Task_management_system.Models
{



    public class CurrentCondition
    {


        public DateTime LocalObservationDateTime { get; set; }
        public int EpochTime { get; set; }
        public string WeatherText { get; set; }
        public int WeatherIcon { get; set; }
        public bool HasPrecipitation { get; set; }
        public object PrecipitationType { get; set; }
        public bool IsDayTime { get; set; }
        public Temperature Temperature { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }




}
