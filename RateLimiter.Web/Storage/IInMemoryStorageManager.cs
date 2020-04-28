using RateLimiter.Data;
using System;
using System.Collections.Generic;

namespace RateLimiter.Storage
{
    public interface IInMemoryStorageManager
    {
        void Add(Access access);
        int Count(Func<Access, bool> func);
        IList<Access> GetAll();
        Access GetById(int id);
        long GetLastId();
        void Delete(Access access);
        void Delete(int id);

    }
}