using System;
using System.Collections.Generic;

namespace RestaurantSystem.Domain
{
    public class RestaurantOrderProcessor : IOrderService
    {
        private readonly KitchenStation _hotKitchen = new HotKitchenStation();
        private readonly KitchenStation _bar = new BarStation();
        private readonly Queue<Order> _ordersQueue = new Queue<Order>();
        private readonly Dictionary<Guid, Order> _completedOrdersArchive = new Dictionary<Guid, Order>();
        private readonly Random _random = new Random();

        // ПАТЕРН OBSERVER: Оголошуємо подію готовності замовлення
        public event EventHandler<Order>? OrderReady;

        public int PendingOrdersCount => _ordersQueue.Count;

        public void ProcessOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.Items.Count == 0) throw new EmptyOrderException(order.OrderNumber);

            order.Status = "В черзі на приготування";
            _ordersQueue.Enqueue(order);
        }

        public void CookNextOrderInQueue(Action<string> webLogger)
        {
            if (_ordersQueue.Count == 0)
            {
                webLogger("[QUEUE] Черга ресторану порожня. Немає замовлень.");
                return;
            }

            try
            {
                RetryPolicy.ExecuteWithRetry<bool>(() =>
                {
                    if (_random.Next(1, 10) <= 3)
                    {
                        throw new InvalidOperationException("Кухонний термінал друку замовлень тимчасово недоступний (Помилка шини даних).");
                    }

                    Order currentOrder = _ordersQueue.Peek();
                    webLogger($"[PROCESSOR] Почалась робота над замовленням №{currentOrder.OrderNumber}...");

                    foreach (var item in currentOrder.Items)
                    {
                        if (item is DrinkItem)
                            _bar.PrepareItem(item);
                        else
                            _hotKitchen.PrepareItem(item);
                    }

                    _ordersQueue.Dequeue();
                    currentOrder.Status = "Готово до видачі";

                    decimal totalSum = currentOrder.GetTotalSum();
                    FinancialMonitor.Instance.RegisterPayment(totalSum);
                    _completedOrdersArchive.Add(currentOrder.Id, currentOrder);

                    webLogger($"[КУХНЯ]: Замовлення №{currentOrder.OrderNumber} успішно видано. Каса отримала +{totalSum:F2} грн.");

                    // ПАТЕРН OBSERVER: Повідомляємо підписників, що замовлення готове
                    OnOrderReady(currentOrder);
                    return true;
                }, maxAttempts: 3, logger: webLogger);
            }
            catch (Exception ex)
            {
                webLogsBackup(webLogger, $"[КРИТИЧНИЙ ЗБІЙ]: Не вдалося обслужити замовлення після 3 спроб. Причина: {ex.Message}");
            }
        }

        private void webLogsBackup(Action<string> logger, string msg) => logger(msg);

        // Захищений метод для безпечного виклику події
        protected virtual void OnOrderReady(Order order)
        {
            OrderReady?.Invoke(this, order);
        }

        public Order? FindOrderInArchive(Guid id)
        {
            if (_completedOrdersArchive.TryGetValue(id, out Order? foundOrder)) return foundOrder;
            return null;
        }

        public void CancelOrder(Order order, string reason)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            order.Status = "Скасовано";
        }
    }
}
