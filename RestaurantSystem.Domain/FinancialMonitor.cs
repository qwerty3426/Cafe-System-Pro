using System;

namespace RestaurantSystem.Domain
{
    public class FinancialMonitor
    {
        // 1. Єдиний приватний статичний екземпляр класу (Singleton)
        private static FinancialMonitor? _instance;
        private static readonly object _lock = new object();

        // Спільний ресурс каси — загальний прибуток ресторану
        public decimal TotalRevenue { get; private set; }

        // 2. Приватний конструктор (забороняє створення через new ззовні)
        private FinancialMonitor()
        {
            TotalRevenue = 0.0m;
            Console.WriteLine("[SINGLETON LOG] Головну касу ресторану успішно ініціалізовано.");
        }

        // 3. Глобальна точка доступу до каси
        public static FinancialMonitor Instance
        {
            get
            {
                // Потокобезпечна ініціалізація (Thread-safe Double-Check Locking)
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FinancialMonitor();
                        }
                    }
                }
                return _instance;
            }
        }

        // Статичний метод або звичайний метод для бізнес-логіки
        public void RegisterPayment(decimal amount)
        {
            if (amount <= 0)
            {
                // Замість тихого ігнорування викидаємо контрольовану помилку
                throw new InvalidPaymentException(amount);
            }
            
            TotalRevenue += amount;
            Console.WriteLine($"[FINANCE] Каса: Зафіксовано оплату на суму +{amount} грн. Загальна виручка закладу: {TotalRevenue} грн.");
        }
    }
}