using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUHTC.Model
{
    public class SettingsModel
    {
        public int Id { get; set; }

        public string MapName { get; set; }

        public byte[] MapImage { get; set; }

        public string Offset1X { get; set; }

        public string Offset1Y { get; set; }

        public string Offset2X { get; set; }

        public string Offset2Y { get; set; }

    }
}
