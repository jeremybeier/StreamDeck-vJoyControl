using Newtonsoft.Json;

namespace VJoyControl
{
    internal class PluginSettings
    {
        internal static PluginSettings CreateDefault()
        {
            var instance = new PluginSettings();
            instance.JoystickId = "1";
            instance.ButtonId = "1";
            instance.Toggle = "0";
            instance.OnColor = "#FF0000";
            instance.OffColor = "#000000";
            return instance;
        }

        [JsonProperty(PropertyName = "JoystickId")]
        public string JoystickId { get; set; }

        [JsonProperty(PropertyName = "ButtonId")]
        public string ButtonId { get; set; }

        [JsonProperty(PropertyName = "Toggle")]
        public string Toggle { get; set; }

        [JsonProperty(PropertyName = "OnColor")]
        public string OnColor { get; set; }

        [JsonProperty(PropertyName = "OffColor")]
        public string OffColor { get; set; }
    }
}