using System;
using BarRaider.SdTools;
using vJoyInterfaceWrap;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace VJoyControl
{
    abstract class VJoyPluginBase : PluginBase
    {
        protected static bool[,] VJoyDeviceButtonStates { get; } = new bool[16, 128];

        protected PluginSettings Settings { get; set; }

        protected vJoy VJoy { get; set; }

        protected uint JoystickId { get; set; } = 1;

        protected uint ButtonId { get; set; } = 1;

        protected bool Toggle { get; set; } = false;

        protected Brush OnBrush { get; set; } = Brushes.Green;

        protected Brush OffBrush { get; set; } = Brushes.Black;

        protected VJoyPluginBase(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload == null || payload.Settings.Count == 0)
            {
                Settings = PluginSettings.CreateDefault();
            }
            else
            {
                Settings = payload.Settings.ToObject<PluginSettings>();
            }
            ParseSettings(Settings, this);
            ConnectClient();
        }

        private static void ParseSettings(PluginSettings settings, VJoyPluginBase @base)
        {
            if (uint.TryParse(settings.JoystickId, out var jid))
            {
                @base.JoystickId = jid;
            }
            if (uint.TryParse(settings.ButtonId, out var bid))
            {
                @base.ButtonId = bid;
            }
            if (uint.TryParse(settings.Toggle, out var t))
            {
                @base.Toggle = t == 1;
            }
            try
            {
                @base.OffBrush = new SolidBrush(ColorTranslator.FromHtml(settings.OffColor));
                @base.OnBrush = new SolidBrush(ColorTranslator.FromHtml(settings.OnColor));
            }
            catch (Exception ex)
            {
                @base.Connection.LogSDMessage($"invalid color code | {ex.Message}");
                Logger.Instance.LogMessage(TracingLevel.WARN, $"invalid color code | {ex.Message}");
            }
        }

        private void ConnectClient()
        {
            try
            {
                VJoy = new vJoy();
                if (!VJoy.vJoyEnabled())
                {
                    Connection.LogSDMessage("vJoy driver not enabled: Failed Getting vJoy attributes.");
                    Logger.Instance.LogMessage(TracingLevel.ERROR, "vJoy driver not enabled: Failed Getting vJoy attributes.");
                }
                else
                {
                    Connection.LogSDMessage($"Vendor: {VJoy.GetvJoyManufacturerString()}\nProduct :{VJoy.GetvJoyProductString()}\nVersion Number:{VJoy.GetvJoySerialNumberString()}\n");
                    Logger.Instance.LogMessage(TracingLevel.INFO, $"Vendor: {VJoy.GetvJoyManufacturerString()}\nProduct :{VJoy.GetvJoyProductString()}\nVersion Number:{VJoy.GetvJoySerialNumberString()}\n");
                }
                if (!VJoy.AcquireVJD(JoystickId))
                {
                    Connection.ShowAlert().Start();
                }
                else
                {
                    VJoy.ResetAll();
                }
            }
            catch (ArgumentException ex)
            {
                Connection.LogSDMessage($"{ex.Message}");
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"{ex.Message}");
            }
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(Settings, payload.Settings);
            Connection.SetSettingsAsync(JObject.FromObject(Settings));
            ParseSettings(Settings, this);
            ConnectClient();
        }
    }
}