using System;

namespace RestaurantSystem.Domain
{
    // Абстрактний декоратор, який сам є MenuItem, але загортає в себе інший MenuItem
    public abstract class ProductDecorator : MenuItem
    {
        protected readonly MenuItem _baseProduct;

        protected ProductDecorator(MenuItem product, string name, decimal price)
            : base(product?.Name ?? "Страва", product?.GetPrice() ?? 0)
        {
            _baseProduct = product ?? throw new ArgumentNullException(nameof(product));
            // Копіюємо категорію оригінального продукту
            this.Category = _baseProduct.Category;
        }
    }

    // КОНКРЕТНИЙ ДЕКОРАТОР 1: Додатковий Сир (для Піци / Гарячих страв)
    public class ExtraCheeseDecorator : ProductDecorator
    {
        public ExtraCheeseDecorator(MenuItem product) : base(product, "Додатковий сир", 35.00m) { }

        public override string Name => $"{_baseProduct.Name} (+ Подвійний Сир)";

        public override decimal GetPrice()
        {
            // Ціна базової страви + 35 грн за сир
            return _baseProduct.GetPrice() + 35.00m;
        }
    }

    // КОНКРЕТНИЙ ДЕКОРАТОР 2: Солодкий Сироп (для Кави)
    public class CoffeeSyrupDecorator : ProductDecorator
    {
        public CoffeeSyrupDecorator(MenuItem product) : base(product, "Сироп", 15.00m) { }

        public override string Name => $"{_baseProduct.Name} (+ Топінг Сироп)";

        public override decimal GetPrice()
        {
            // Ціна кави + 15 грн за сироп
            return _baseProduct.GetPrice() + 15.00m;
        }
    }
}
