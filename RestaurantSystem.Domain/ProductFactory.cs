using System;

namespace RestaurantSystem.Domain
{
    public static class ProductFactory
    {
        // Фабричний метод для створення позицій меню
        public static MenuItem CreateProduct(string category, string name, decimal price, double volume = 0)
        {
            switch (category)
            {
                case "Гарячі страви":
                    return new MenuItem(name, price) { Category = category };

                case "Холодні напої":
                    // Для напоїв автоматично вираховуємо податок на тару (10 грн), якщо об'єм більше 0.3л
                    decimal tax = volume > 0.3 ? 15.00m : 10.00m;
                    return new DrinkItem(name, price, volume, tax) { Category = category };

                case "Кава":
                    // Кава — це теж напій, але зі своїми специфічними параметрами (наприклад, менший об'єм)
                    return new DrinkItem(name, price, volume, 0.0m) { Category = category };

                default:
                    // Якщо категорія невідома, повертаємо базовий MenuItem
                    return new MenuItem(name, price) { Category = "Інше" };
            }
        }
    }
}
