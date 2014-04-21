using AUHTC.Model;
using AUHTC.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUHTC.ViewModel
{
    public class EntityViewModel
    {
        private SerialDataRepository entity;

        public EntityViewModel()
        {
            entity = new SerialDataRepository();
            entity.Database.CreateIfNotExists();
        }

        internal void AddDataToDB(ProcessedDataModel data)
        {
            entity.SerialData.Add(data);
        }

        internal void RemoveDataFromDB(ProcessedDataModel data)
        {
            entity.SerialData.Remove(data);
        }

        internal List<ProcessedDataModel> GetDataListByDate(DateTime date)
        {
            List<ProcessedDataModel> dataList = new List<ProcessedDataModel>();
            var query = from data in entity.SerialData
                        where data.RecordDate == date
                        select data;

            foreach (var data in query)
            {
                dataList.Add(data);
            }

            return dataList;
        }

        internal List<ProcessedDataModel> GetAllSerialData()
        {
            return entity.SerialData.ToList();
        }

        internal string GetSettings()
        {
            return entity.ProgramSettings.ToString();
        }
    }
}
