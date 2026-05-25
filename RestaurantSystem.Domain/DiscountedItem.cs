using System;

namespace RestaurantSystem.Domain
{
    public class DiscountedItem : MenuItem
    {
        private int _discountPercent;

        public int DiscountPercent
        {
            get => _discountPercent;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentException("Знижка повинна бути в діапазоні від 0 до 100%!");
                _discountPercent = value;
            }
        }

        public DiscountedItem() : base()
        {
            DiscountPercent = 0;
        }

        public DiscountedItem(string name, decimal basePrice, int discountPercent) 
            : base(name, basePrice)
        {
            DiscountPercent = discountPercent;
        }

        public DiscountedItem(DiscountedItem other) : base(other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            DiscountPercent = other.DiscountPercent;
        }

        // Перевизначення віртуального методу
        public override decimal CalculateFinalPrice()
        {
            decimal discountAmount = BasePrice * (_discountPercent / 100m);
            return BasePrice - discountAmount;
        }
    }
}