using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUHTC.Model
{
    public class ProcessedDataModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }

        public int KoorX { get; set; }

        public int KoorY { get; set; }

        public DateTime RecordDate { get; set; }

        public string RaceName { get; set; }

        public string Type { get; set; }

        public string Dump { get; set; }
    }
}
