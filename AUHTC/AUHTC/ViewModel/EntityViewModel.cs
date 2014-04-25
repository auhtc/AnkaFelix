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

        internal void AddSerialDataToDB(ProcessedDataModel data)
        {
            entity.SerialData.Add(data);
        }

        internal void RemoveSerialDataFromDB(ProcessedDataModel data)
        {
            entity.SerialData.Remove(data);
        }

        internal List<ProcessedDataModel> GetSerialDataListByDate(DateTime date)
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

        internal List<ProcessedDataModel> GetSerialDataListByType(string dataType)
        {
            List<ProcessedDataModel> serialDataList = new List<ProcessedDataModel>();
            var query = from data in entity.SerialData
                        where data.Type == dataType
                        select data;

            foreach (var data in query)
            {
                serialDataList.Add(data);
            }

            return serialDataList;
        }

        internal List<ProcessedDataModel> GetAllSerialData()
        {
            return entity.SerialData.ToList();
        }

        internal void SaveSettingsToDB(SettingsModel settings)
        {
            entity.ProgramSettings.Add(settings);
        }

        internal void RemoveSettingsFromDB(SettingsModel settings)
        {
            entity.ProgramSettings.Remove(settings);
        }

        internal SettingsModel GetSettingsByMapName(string mapName)
        {
            var settings = new SettingsModel();/*(from data in entity.ProgramSettings
                            where data.MapName == mapName
                            select data).SingleOrDefault();*/

            return settings;
        }
    }
}
