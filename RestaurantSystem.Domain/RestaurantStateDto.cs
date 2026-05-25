using System;
using System.Collections.Generic;

namespace RestaurantSystem.Domain
{
    // DTO для збереження окремої позиції в логах чи замовленні
    public class OrderItemDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    // Головний DTO-контейнер для збереження повного стану системи
    public class RestaurantStateDto
    {
        public decimal TotalRevenue { get; set; }
        public List<string> SystemLogs { get; set; } = new List<string>();
        public List<OrderItemDto> CurrentDraftItems { get; set; } = new List<OrderItemDto>();
    }
}
