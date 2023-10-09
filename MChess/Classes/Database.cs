using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MChess.Classes
{
    public class Database
    {
        static SQLiteAsyncConnection _database;
        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<User>();
        }
        public static Task<List<User>> GetUserAsync(string username, string password)
        {
            return _database.QueryAsync<User>("SELECT * FROM User WHERE Username=? AND Password=?", username, password);
        }
        public static Task<List<User>> CheckUser(string username)
        {
            return _database.QueryAsync<User>("SELECT * FROM User WHERE Username=?", username);
        }
        public Task<int> AddUserAsync(User user)
        {
            return _database.InsertAsync(user);
        }
        public Task<List<User>> UpdateUser(string username, double newElo)
        {
            return _database.QueryAsync<User>("UPDATE User SET Elo=? WHERE Username=?", newElo, username);
        }
    }
}
