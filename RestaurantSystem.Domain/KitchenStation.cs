using System;

namespace RestaurantSystem.Domain
{
    public abstract class KitchenStation
    {
        public string StationName { get; protected set; }

        protected KitchenStation(string stationName)
        {
            StationName = stationName;
        }

        // Абстрактний метод: кожен цех реалізує його по-своєму
        public abstract void PrepareItem(MenuItem item);

        // Звичайний метод: спільна логіка для всіх цехів кухні
        public void DisplayStationStatus()
        {
            Console.WriteLine($"[KITCHEN] Станція '{StationName}' готова до роботи.");
        }
    }
}