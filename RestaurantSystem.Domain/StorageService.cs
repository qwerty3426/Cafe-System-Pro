using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantSystem.Domain
{
    public static class StorageService
    {
        private static readonly string FilePath = "restaurant_data.json";

        // Метод збереження стану
        public static void SaveState(decimal revenue, List<string> logs, Order currentDraft)
        {
            try
            {
                var state = new RestaurantStateDto
                {
                    TotalRevenue = revenue,
                    SystemLogs = logs,
                    CurrentDraftItems = currentDraft.Items.Select(i => new OrderItemDto
                    {
                        Name = i.Name,
                        Price = i.GetPrice()
                    }).ToList()
                };

                // Опції для красивого форматування JSON файлу
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(state, options);
                File.WriteAllText(FilePath, jsonString);
            }
            catch (Exception)
            {
                // Якщо не вдалося зберегти — система не повинна падати
            }
        }

        // Метод завантаження стану
        public static RestaurantStateDto? LoadState()
        {
            try
            {
                if (!File.Exists(FilePath)) return null;

                string jsonString = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<RestaurantStateDto>(jsonString);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
