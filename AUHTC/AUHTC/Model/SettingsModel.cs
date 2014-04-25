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

        public float Offset1X { get; set; }

        public float Offset1Y { get; set; }

        public float Offset2X { get; set; }

        public float Offset2Y { get; set; }

    }
}
