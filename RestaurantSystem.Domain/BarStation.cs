using System;

namespace RestaurantSystem.Domain
{
    public class BarStation : KitchenStation
    {
        public BarStation() : base("Бар / Напої") { }

        public override void PrepareItem(MenuItem item)
        {
            Console.WriteLine($"[БАР] Наливається або змішується напій: {item.Name}. Охолодження, підготовка келиха...");
        }
    }
}