using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Data;

namespace RateLimiter.Storage
{
    public class InMemoryStorageManager : IInMemoryStorageManager
    {

        public IList<Access> Accesses { get; private set; }

        public InMemoryStorageManager()
        {
            Accesses = new List<Access>();
        }

        public InMemoryStorageManager(List<Access> accesses)
        {
            Accesses = accesses;
        }

        public void Add(Access access)
        {
            if (access == null) return;

            Accesses.Add(access);
        }

        public int Count(Func<Access, bool> func) {
            return Accesses.Count(func);
        }

        public IList<Access> GetAll()
        {
            return Accesses;
        }

        public Access GetById(int id)
        {
            return Accesses.FirstOrDefault(a => a.ID == id);
        }

        public long GetLastId()
        {
            return Accesses.Any() ? Accesses.Max(x => x.ID) : 0;
        }

        public void Delete(Access access)
        {
            if (access == null) return;
            Accesses.Remove(access);
        }

        public void Delete(int id)
        {
            Delete(Accesses.FirstOrDefault(a => a.ID == id));
        }
    }
}
