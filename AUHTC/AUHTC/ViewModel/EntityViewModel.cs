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

        internal void AddDataToDB(SerialDataModel data)
        {
            entity.SerialData.Add(data);
        }

        internal void RemoveDataFromDB(SerialDataModel data)
        {
            entity.SerialData.Remove(data);
        }

        internal List<SerialDataModel> GetDataListByDate(DateTime date)
        {
            List<SerialDataModel> dataList = new List<SerialDataModel>();
            var query = from data in entity.SerialData
                        where data.RecordDate == date
                        select data;

            foreach (var data in query)
            {
                dataList.Add(data);
            }

            return dataList;
        }

        internal List<SerialDataModel> GetAll()
        {
            return entity.SerialData.ToList();
        }
    }
}
