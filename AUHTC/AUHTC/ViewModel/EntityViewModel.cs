using AUHTC.Model;
using AUHTC.Model.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AUHTC.ViewModel
{
    public class EntityViewModel
    {
        private SerialDataRepository entity;

        public EntityViewModel()
        {
            entity = new SerialDataRepository();
            //entity.Database.Delete();
            entity.Database.CreateIfNotExists();
        }

        internal void AddSerialDataToDB(ProcessedDataModel data)
        {
            entity.SerialData.Add(data);
            entity.SaveChanges();
        }

        internal void RemoveSerialDataFromDB(ProcessedDataModel data)
        {
            entity.SerialData.Remove(data);
            entity.SaveChanges();
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
            if (entity.ProgramSettings.Any(s => s.MapName == settings.MapName))
            {
                SettingsModel set = entity.ProgramSettings.FirstOrDefault(s => s.MapName == settings.MapName);
                set.MapImage = settings.MapImage;
                set.Offset1X = settings.Offset1X;
                set.Offset1Y = settings.Offset1Y;
                set.Offset2X = settings.Offset2X;
                set.Offset2Y = settings.Offset2Y;
            }
            else
            {
                entity.Entry(settings).State = EntityState.Added;
            }
            entity.SaveChanges();
        }

        internal void RemoveSettingsFromDB(SettingsModel settings)
        {
            entity.ProgramSettings.Remove(settings);
            entity.SaveChanges();
        }

        internal List<SettingsModel> GetAllSettings()
        {
            return entity.ProgramSettings.ToList();
        }

        internal SettingsModel GetSettingsByMapName(string p)
        {
            var query = from oData in entity.ProgramSettings
                        where oData.MapName == p
                        select oData;

            SettingsModel result = query.FirstOrDefault<SettingsModel>();

            return result;
        }

        internal SettingsModel GetSettingsById(int p)
        {
            var query = from oData in entity.ProgramSettings
                        where oData.Id == p
                        select oData;

            SettingsModel result = query.FirstOrDefault<SettingsModel>();

            return result;
        }
    }
}
