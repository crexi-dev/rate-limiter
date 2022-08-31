using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _orders = new();
        private Random _rnd = new Random();
        public OrderRepository()
        {
            InitializeProductStore();
        }

        public List<Order> GetAll()
        {
            return _orders.Values.ToList();
        }

        public Order GetById(Guid id)
        {
            return _orders[id];
        }

        private void InitializeProductStore()
        {
            //Creating 5 random sample products
            for (var i = 0; i < 5; i++)
            {
                var id = Guid.NewGuid();
                _orders.Add(id, new Order(id, $"Sample Product {i + 1}", _rnd.Next(30, 70), _rnd.Next(1, 5)));
            }
        }
    }
}
