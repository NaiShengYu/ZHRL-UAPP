using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using AepApp.Models;
using SQLite;



namespace Todo

{

    public class TodoItemDatabase

    {

        readonly SQLiteAsyncConnection database;
        public TodoItemDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<TodoItem>().Wait();
            database.CreateTableAsync<UploadEmergencyModel>().Wait();
            database.CreateTableAsync<LoginModel>().Wait();
            Console.WriteLine("表创建成功");
        }

        public Task<List<TodoItem>> GetItemsAsync()
        {
            return database.Table<TodoItem>().ToListAsync();
        }
        public Task<List<UploadEmergencyModel>> GetEmergencyAsync()
        {
            return database.Table<UploadEmergencyModel>().ToListAsync();
        }
        public Task<List<LoginModel>> GetUserModelAsync()
        {
            return database.Table<LoginModel>().ToListAsync();
        }


        public Task<List<TodoItem>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }



        public Task<TodoItem> GetItemAsync(int id)      
        {
            return database.Table<TodoItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }
        public Task<UploadEmergencyModel> GetEmergencyAsync(int id)      
        {
            return database.Table<UploadEmergencyModel>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(TodoItem item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }
        public Task<int> SaveEmergencyAsync(UploadEmergencyModel item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> SaveUserModelAsync(LoginModel item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }


        public Task<int> DeleteItemAsync(TodoItem item)
        {
            return database.DeleteAsync(item);
        }
        public Task<int> DeleteEmergencyAsync(UploadEmergencyModel item)
        {
            return database.DeleteAsync(item);
        }

        public Task<int> DeleteUserModelAsync(LoginModel item)
        {
            return database.DeleteAsync(item);
        }
    }

}