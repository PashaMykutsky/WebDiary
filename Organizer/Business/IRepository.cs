using Microsoft.EntityFrameworkCore;
using Organizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Business
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Create(T item);
        void Update(T item);
        void Delete(int id);
    }
    public class UsersRepository : IRepository<User>
    {
        private ModelContext db;
        public UsersRepository(ModelContext context)
        {
            db = context;
        }
        public void Create(User item)
        {
            item.Password = CryptoService.HashingPassword(item.Password);
            item.IsAdmin = false;
            item.IsBanned = false;
            item.LastActive = DateTime.Now;
            db.Users.Add(item);
        }

        public void Delete(int id)
        {
            User user = db.Users.Find(id);
            if (user != null)
                db.Users.Remove(user);
        }

        public User Get(int id)
        {
            return db.Users.Find(id);
        }

        public User GetByEmail(string email)
        {
            return db.Users.FirstOrDefault(u => u.Email.Equals(email));
        }

        public IEnumerable<User> GetAll()
        {
            return db.Users;
        }

        public void Update(User item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
        public User UserVerification(string login, string password)
        {
            User user = this.GetByEmail(login);
            if (user != null)
            {
                if (CryptoService.VerificationPassword(password, user.Password))
                {
                    return user;
                }
            }
            return null;
        }
    }

    public class RecordsRepository : IRepository<Record>
    {
        private ModelContext db;
        public RecordsRepository(ModelContext context)
        {
            db = context;
        }
        public void Create(Record item)
        {
            db.Records.Add(item);
        }

        public void Delete(int id)
        {
            Record record = db.Records.Find(id);
            if (record != null)
                db.Records.Remove(record);
        }

        public Record Get(int id)
        {
            return db.Records.Find(id);
        }

        public IEnumerable<Record> GetAll()
        {
            return db.Records;
        }

        public bool CheckRecordsUser(int Id_User)
        {
            if (db.Records.First(r => r.User.Id == Id_User) != null)
                return true;
            return false;
        }

        public IEnumerable<Record> GetAllByUser(int id)
        {
            return db.Records.Where( r => r.User.Id == id);
        }

        public void Update(Record item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}
