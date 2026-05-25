using System;
using System.Threading.Tasks;

namespace RestaurantSystem.Domain
{
    public static class RetryPolicy
    {
        // Універсальний метод повторення операцій (використовує функціональний делегати Func<T> та Action для логування)
        public static T ExecuteWithRetry<T>(Func<T> operation, int maxAttempts, Action<string> logger)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    // Намагаємося виконати бізнес-операцію
                    return operation();
                }
                catch (Exception ex) when (ex is not EmptyOrderException) // Помилки валідації не ретраїм
                {
                    if (attempt >= maxAttempts)
                    {
                        logger($"[RETRY POLICY] Досягнуто ліміту спроб ({maxAttempts}). Операція завершилась аварійно.");
                        throw;
                    }

                    // Обчислення експоненційної затримки: 2^attempt * 100 мілісекунд (наприклад: 200мс, 400мс, 800мс...)
                    int delay = (int)Math.Pow(2, attempt) * 100;
                    logger($"[RETRY POLICY] Спроба №{attempt} провалилася через: {ex.Message}. Повтор через {delay}мс...");
                    Task.Delay(delay).Wait();
                }
            }
        }
    }
}
