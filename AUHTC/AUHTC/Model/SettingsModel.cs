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

        public string MapLocation { get; set; }

        public int Offset1X { get; set; }

        public int Offset1Y { get; set; }

        public int Offset2X { get; set; }

        public int Offset2Y { get; set; }

    }
}
