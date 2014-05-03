using System.Windows;

namespace AUHTC.Model
{
    public class Constants
    {
        public double MapHeight { get; set; }

        public double MapWidth { get; set; }

        public double MapLeft { get; set; }

        public double MapTop { get; set; }

        public double RatioX { get; set; }

        public double RatioY { get; set; }

        public Point Offset { get; set; }

        public SettingsModel Setting { get; set; }
        public Constants()
        {
            Setting = App.Database.GetSettingsByMapName(App.CurrentMapName);
        }

        internal void Inıt(System.Windows.Controls.Image MapImage)
        {
            Offset = new Point((int)float.Parse(Setting.Offset1X), (int)float.Parse(Setting.Offset1Y));
            Point Offset2 = new Point((int)float.Parse(Setting.Offset2X), (int)float.Parse(Setting.Offset2Y));

            Point t_Offset = new Point((Offset2.X - Offset.X), (Offset2.Y - Offset.Y));

            MapHeight = MapImage.Source.Height;
            MapWidth = MapImage.Source.Width;
            MapLeft = MapImage.Margin.Left;
            MapTop = MapImage.Margin.Right;

            RatioX = MapHeight / t_Offset.X;
            RatioY = MapWidth / t_Offset.Y;
        }
    }
}
