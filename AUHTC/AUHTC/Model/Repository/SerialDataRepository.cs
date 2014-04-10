using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUHTC.Model.Repository
{
    public class SerialDataRepository : DbContext
    {
        public DbSet<SerialDataModel> SerialData { get; set; }
    }
}
