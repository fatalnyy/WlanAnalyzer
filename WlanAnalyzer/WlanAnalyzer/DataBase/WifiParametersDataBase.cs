using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using WlanAnalyzer.Models;

namespace WlanAnalyzer.DataBase
{
    public class WifiParametersDataBase
    {
        readonly SQLiteAsyncConnection database;
        public WifiParametersDataBase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<WifiParameters>().Wait();
        }

        public Task<List<WifiParameters>> GetListOfWifiParametersAsync()
        {
            return database.Table<WifiParameters>().ToListAsync();
        }

        public Task<WifiParameters> GetParticularWifiParametersAsync(int id)
        {
            return database.Table<WifiParameters>().Where(i => i.WifiID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveWifiParametersAsync(WifiParameters wifiParameters)
        {
            return database.InsertAsync(wifiParameters);
        }
        public Task<int> UpdateWifiParametersAsync(WifiParameters wifiParameters)
        {
            return database.UpdateAsync(wifiParameters);
        }
        public Task SaveCollectionOfWifiParameters(ObservableCollection<WifiParameters> collectionOfWifiParameters)
        {
            return database.InsertAllAsync(collectionOfWifiParameters);
        }
        public Task<int> DeleteParticularWifiParameters(WifiParameters wifiParameters)
        { 
            return database.DeleteAsync(wifiParameters);
        }
        public void DeleteAllObjectsFromDatabase()
        {
            database.DropTableAsync<WifiParameters>().Wait();
            database.CreateTableAsync<WifiParameters>().Wait();
        }
    }
}
