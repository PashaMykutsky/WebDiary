using Organizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Business
{
    public class UnitOfWork
    {
        private ModelContext mc;
        private UsersRepository usersRepository;
        private RecordsRepository recordsRepository;
        public UnitOfWork(ModelContext context)
        {
            mc = context;
        }

        public UsersRepository Users
        {
            get
            {
                if (usersRepository == null)
                    usersRepository = new UsersRepository(mc);
                return usersRepository;
            }
        }

        public RecordsRepository Records
        {
            get
            {
                if (recordsRepository == null)
                    recordsRepository = new RecordsRepository(mc);
                return recordsRepository;
            }
        }

        public void SaveChanges()
        {
            mc.SaveChanges();
        }
    }
}
