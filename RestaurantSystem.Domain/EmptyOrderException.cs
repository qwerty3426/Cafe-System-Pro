using System;

namespace RestaurantSystem.Domain
{
    public class EmptyOrderException : RestaurantException
    {
        public int OrderNumber { get; private set; }

        public EmptyOrderException(int orderNumber)
            : base($"Помилка обробки: Замовлення №{orderNumber} не містить жодної страви! Немає чого готувати.")
        {
            OrderNumber = orderNumber;
        }
    }
}
