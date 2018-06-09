
using AepApp.Models;
using SQLite;

using System.Collections.Generic;

using System.Threading.Tasks;

namespace Todo
{
    public class EmergencyAccidentLocalDB
    {
        readonly SQLiteAsyncConnection emergencyDB;
        public  EmergencyAccidentLocalDB(string dbPath)

        {
            emergencyDB = new SQLiteAsyncConnection(dbPath);
            emergencyDB.CreateTableAsync<UploadEmergencyModel>().Wait();
          var a = emergencyDB.CreateTableAsync<UploadEmergencyModel>();
        }



        public Task<List<UploadEmergencyModel>> GetItemsAsync()
        {
            return emergencyDB.Table<UploadEmergencyModel>().ToListAsync();
        }


        public Task<List<UploadEmergencyModel>> GetItemsNotDoneAsync()
        {
            return emergencyDB.QueryAsync<UploadEmergencyModel>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }



        public Task<UploadEmergencyModel> GetItemAsync(int id)
        {
            return emergencyDB.Table<UploadEmergencyModel>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }



        public Task<int> SaveItemAsync(UploadEmergencyModel item)

        {

            if (item.ID != 0)

            {

                return emergencyDB.UpdateAsync(item);

            }

            else
            {

                return emergencyDB.InsertAsync(item);

            }

        }



        public Task<int> DeleteItemAsync(UploadEmergencyModel item)

        {

            return emergencyDB.DeleteAsync(item);

        }

    }
}
