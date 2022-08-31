using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Rating { get; set; }

        public Order(Guid id, string name, int price, int rating)
        {
            Id = id;
            Name = name;
            Price = price;
            Rating = rating;
        }
    }
}
