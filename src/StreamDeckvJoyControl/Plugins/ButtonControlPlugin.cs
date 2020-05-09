using System;
using System.Drawing;
using BarRaider.SdTools;

namespace VJoyControl
{
    [PluginActionId("com.randombehavior.vjoycontrol.buttoncontrol.streamdeckplugin")]
    class ButtonControlPlugin : VJoyPluginBase
    {
        public ButtonControlPlugin(SDConnection connection, InitialPayload payload) : base(connection, payload) { }

        public override void KeyReleased(KeyPayload payload)
        {
            if (!Toggle)
            {
                CurrentState = false;
                VJoy.SetBtn(CurrentState, JoystickId, ButtonId);
                DrawState(CurrentState);
            }
        }

        private bool CurrentState
        {
            get
            {
                return VJoyDeviceButtonStates[JoystickId - 1, ButtonId - 1];
            }
            set
            {
                VJoyDeviceButtonStates[JoystickId - 1, ButtonId - 1] = value;
            }
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (!Toggle)
            {
                CurrentState = true;
            }
            else
            {
                CurrentState = !CurrentState;
            }
            VJoy.SetBtn(CurrentState, JoystickId, ButtonId);
            DrawState(CurrentState);
        }

        private static Image img = Image.FromFile("Images/base_t@2x.png");
        private async void DrawState(bool state)
        {
            try
            {
                Bitmap bmp = Tools.GenerateGenericKeyImage(out var graphics);
                graphics.FillRectangle(state ? OnBrush : OffBrush, 0, 0, bmp.Width, bmp.Height);
                graphics.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
                string imgBase64 = Tools.ImageToBase64(bmp, true);
                await Connection.SetImageAsync(imgBase64);
                graphics.Dispose();
            }
            catch (Exception ex)
            {
                await Connection.LogSDMessage($"DrawSymbolData Exception: {ex}");
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"DrawSymbolData Exception: {ex}");
            }
        }

        public override void Dispose() { }

        public override void OnTick() { DrawState(CurrentState); }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }
    }
}