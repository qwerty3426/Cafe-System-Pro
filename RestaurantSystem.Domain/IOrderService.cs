using System;

namespace RestaurantSystem.Domain
{
    public interface IOrderService
    {
        void ProcessOrder(Order order);
        void CancelOrder(Order order, string reason);
    }
}