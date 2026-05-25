using System;

namespace RestaurantSystem.Domain
{
    public class HotKitchenStation : KitchenStation
    {
        public HotKitchenStation() : base("Гарячий Цех") { }

        public override void PrepareItem(MenuItem item)
        {
            Console.WriteLine($"[ГАРЯЧИЙ ЦЕХ] Готується гаряча страва: {item.Name}. Розігрів плити, термообробка...");
        }
    }
}