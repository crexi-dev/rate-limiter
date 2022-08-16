using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Models
{
    public sealed class History<T>
    {
        private LinkedList<T> items = new LinkedList<T>();

        public List<T> Items => items.ToList();

        public int capacity { get; }

        public History(int capacity)
        {
            this.capacity = capacity;
        }

        public void Add(T item)
        {
            if (items.Count == capacity)
            {
                items.RemoveFirst();
                items.AddLast(item);
            }
            else
            {
                items.AddLast(new LinkedListNode<T>(item));
            }
        }
    }
}
