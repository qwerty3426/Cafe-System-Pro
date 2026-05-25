using System;
using System.Collections.Generic;

namespace RestaurantSystem.Domain
{
    public class Order : EventArgs, IDisposable
    {
        private static int _globalOrderCounter = 0;

        public Guid Id { get; private set; }
        public int OrderNumber { get; private set; }
        public List<MenuItem> Items { get; private set; }
        public Staff AssignedWaiter { get; private set; }
        public string Status { get; set; }

        public Order(Staff waiter)
        {
            Id = Guid.NewGuid();
            Items = new List<MenuItem>();
            Status = "Нове";
            AssignedWaiter = waiter ?? throw new ArgumentNullException(nameof(waiter));
            
            _globalOrderCounter++;
            OrderNumber = _globalOrderCounter;
        }

        public void AddItem(MenuItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Items.Add(item);
        }

        public decimal GetTotalSum()
        {
            decimal sum = 0;
            foreach (var item in Items)
            {
                sum += item.GetPrice();
            }
            return sum;
        }

        public static Order operator +(Order order, MenuItem item)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            order.AddItem(item);
            return order;
        }

        public static Order operator -(Order order, MenuItem item)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (item == null) throw new ArgumentNullException(nameof(item));
            order.Items.Remove(item);
            return order;
        }

        public void Dispose()
        {
            Items.Clear();
        }
    }
}
