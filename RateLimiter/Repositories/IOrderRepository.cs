using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Repositories
{
    public interface IOrderRepository
    {
        Order GetById(Guid id);
        List<Order> GetAll();
    }
}
