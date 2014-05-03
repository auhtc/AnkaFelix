using System;

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

        public string Dump { get; set; } // Unuttuğumuz birşey var
        // Regex'e uymayan veriler buraya atılacak sanırım ancak $GRPMC dışında gelen gps verileri regexe uymayınca
        // burası boş yere şişecek ??
    }
}
