using System;

namespace RestaurantSystem.Domain
{
    public class DrinkItem : MenuItem
    {
        private double _volume;

        public double Volume
        {
            get => _volume;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Об'єм напою має бути більшим за нуль!");
                _volume = value;
            }
        }

        public decimal BottleTax { get; set; }

        public DrinkItem() : base()
        {
            Volume = 0.5;
            BottleTax = 0.0m;
        }

        public DrinkItem(string name, decimal basePrice, double volume, decimal bottleTax) 
            : base(name, basePrice)
        {
            Volume = volume;
            BottleTax = bottleTax;
        }

        public DrinkItem(DrinkItem other) : base(other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            Volume = other.Volume;
            BottleTax = other.BottleTax;
        }

        // Перевизначення віртуального методу
        public override decimal CalculateFinalPrice()
        {
            return BasePrice + BottleTax;
        }
    }
}