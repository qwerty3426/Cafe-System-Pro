using System;

namespace RestaurantSystem.Domain
{
    public class MenuItem
    {
        private string _name = "Unknown";
        private decimal _basePrice;

        public Guid Id { get; private set; }

        public virtual string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва страви не може бути пустою!");
                _name = value;
            }
        }

        public decimal BasePrice
        {
            get => _basePrice;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Ціна страви не може бути від'ємною!");
                _basePrice = value;
            }
        }

        public string Category { get; set; } = "Без категорії";

        
        public virtual decimal GetPrice()
        {
            return CalculateFinalPrice();
        }

        // Конструктор за замовчуванням
        public MenuItem()
        {
            Id = Guid.NewGuid();
            Name = "Стандартна страва";
            BasePrice = 0.0m;
        }

        // Конструктор з параметрами
        public MenuItem(string name, decimal basePrice)
        {
            Id = Guid.NewGuid();
            Name = name;
            BasePrice = basePrice;
        }

        // Копіювальний конструктор
        public MenuItem(MenuItem other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            Id = Guid.NewGuid();
            Name = other.Name;
            BasePrice = other.BasePrice;
        }

        // ВІРТУАЛЬНИЙ МЕТОД: може перевизначатися у похідних
        public virtual decimal CalculateFinalPrice()
        {
            return BasePrice;
        }
    }
}