using Xunit;
using RestaurantSystem.Domain;
using System;
using System.Collections.Generic;

namespace RestaurantSystem.Tests
{
    public class OrderTests
    {
        private readonly Staff _defaultWaiter = new Staff("Микола", "Старший офіціант");

        #region 1. ТЕСТИ СТРУКТУРИ ТА ОПЕРАТОРІВ (AAA)

        [Fact]
        public void Order_OperatorPlus_ShouldAddMenuItemCorrectly()
        {
            // Arrange
            var order = new Order(_defaultWaiter);
            var item = new MenuItem("Борщ", 120.00m);

            // Act
            order += item;

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(120.00m, order.GetTotalSum());
        }

        [Fact]
        public void Order_OperatorMinus_ShouldRemoveItemIfExists()
        {
            // Arrange
            var order = new Order(_defaultWaiter);
            var item = new MenuItem("Борщ", 120.00m);
            order += item;

            // Act
            order -= item;

            // Assert
            Assert.Empty(order.Items);
            Assert.Equal(0m, order.GetTotalSum());
        }

        [Fact]
        public void Order_GetTotalSum_ShouldAggregateAllItems()
        {
            // Arrange
            var order = new Order(_defaultWaiter);
            order += new MenuItem("Борщ", 120.00m);
            order += new MenuItem("Сік", 50.00m);

            // Act
            decimal total = order.GetTotalSum();

            // Assert
            Assert.Equal(170.00m, total);
        }

        #endregion

        #region 2. ТЕСТИ ВАЛІДАЦІЇ ТА CUSTOM EXCEPTIONS

        [Fact]
        public void Processor_ProcessOrder_ShouldThrowEmptyOrderException_WhenOrderIsEmpty()
        {
            // Arrange
            var processor = new RestaurantOrderProcessor();
            var emptyOrder = new Order(_defaultWaiter);

            // Act & Assert
            Assert.Throws<EmptyOrderException>(() => processor.ProcessOrder(emptyOrder));
        }

        [Fact]
        public void Processor_ProcessOrder_ShouldThrowArgumentNullException_WhenOrderIsNull()
        {
            // Arrange
            var processor = new RestaurantOrderProcessor();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => processor.ProcessOrder(null!));
        }

        #endregion

        #region 3. ТЕСТИ ПАТЕРНУ DECORATOR (СТРУКТУРНИЙ)

        [Fact]
        public void ExtraCheeseDecorator_ShouldWrapNameAndAddPrice()
        {
            // Arrange
            var pizza = new MenuItem("Піца Чотири Сири", 210.00m);

            // Act
            var decorated = new ExtraCheeseDecorator(pizza);

            // Assert
            Assert.Equal(245.00m, decorated.GetPrice());
            Assert.Equal("Піца Чотири Сири (+ Подвійний Сир)", decorated.Name);
        }

        [Fact]
        public void CoffeeSyrupDecorator_ShouldWrapNameAndAddPrice()
        {
            // Arrange
            var coffee = new MenuItem("Капучино", 55.00m);

            // Act
            var decorated = new CoffeeSyrupDecorator(coffee);

            // Assert
            Assert.Equal(70.00m, decorated.GetPrice());
            Assert.Equal("Капучино (+ Топінг Сироп)", decorated.Name);
        }

        #endregion

        #region 4. ТЕСТИ ПАТЕРНУ FACTORY METHOD (ПОРОДЖУВАЛЬНИЙ)

        [Fact]
        public void ProductFactory_ShouldCreateDrinkItem_ForColdDrinksCategory()
        {
            // Arrange & Act
            var product = ProductFactory.CreateProduct("Холодні напої", "Кола", 50.00m, 0.5);

            // Assert
            Assert.IsType<DrinkItem>(product);
            Assert.Equal("Холодні напої", product.Category);
        }

        [Fact]
        public void ProductFactory_ShouldApplyTax_WhenVolumeIsLarge()
        {
            // Arrange & Act
            var largeDrink = (DrinkItem)ProductFactory.CreateProduct("Холодні напої", "Кола", 50.00m, 0.5);
            var smallDrink = (DrinkItem)ProductFactory.CreateProduct("Холодні напої", "Еспресо", 40.00m, 0.06);

            // Assert
            Assert.Equal(15.00m, largeDrink.BottleTax);
            Assert.Equal(10.00m, smallDrink.BottleTax);
        }

        #endregion

        #region 5. ТЕСТИ SINGLETON ТА ФІНАНСІВ

        [Fact]
        public void FinancialMonitor_ShouldBeSingleton_AndMaintainSameInstance()
        {
            // Arrange & Act
            var instance1 = FinancialMonitor.Instance;
            var instance2 = FinancialMonitor.Instance;

            // Assert
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void FinancialMonitor_RegisterPayment_ShouldIncreaseTotalRevenue()
        {
            // Arrange
            var monitor = FinancialMonitor.Instance;
            decimal initialRevenue = monitor.TotalRevenue;

            // Act
            monitor.RegisterPayment(100.00m);

            // Assert
            Assert.Equal(initialRevenue + 100.00m, monitor.TotalRevenue);
        }

        #endregion

        #region 6. ТЕСТИ RETRY POLICY ТА ДЕЛЕГАТІВ

        [Fact]
        public void RetryPolicy_ShouldReturnResultImmediately_IfNoExceptionOccurs()
        {
            // Arrange
            int executionCount = 0;
            Func<string> operation = () => { executionCount++; return "Success"; };
            List<string> logs = new List<string>();

            // Act
            string result = RetryPolicy.ExecuteWithRetry(operation, maxAttempts: 3, msg => logs.Add(msg));

            // Assert
            Assert.Equal("Success", result);
            Assert.Equal(1, executionCount);
            Assert.Empty(logs);
        }

        [Fact]
        public void RetryPolicy_ShouldRetryAndFail_WhenMaxAttemptsReached()
        {
            // Arrange
            Func<bool> failingOperation = () => throw new Exception("Hardware Error");
            List<string> logs = new List<string>();

            // Act & Assert
            Assert.Throws<Exception>(() =>
                RetryPolicy.ExecuteWithRetry(failingOperation, maxAttempts: 3, msg => logs.Add(msg))
            );
            Assert.Equal(3, logs.Count); // 3 лог-повідомлення: під час кожної невдалої спроби і після досягнення ліміту
        }

        #endregion

        #region 7. ТЕСТИ ЖИТТЄВОГО ЦИКЛУ ТА ЗБЕРЕЖЕННЯ (JSON / DTO)

        [Fact]
        public void Order_Dispose_ShouldClearItemsCollection()
        {
            // Arrange
            var order = new Order(_defaultWaiter);
            order += new MenuItem("Суп", 80.00m);

            // Act
            order.Dispose();

            // Assert
            Assert.Empty(order.Items);
        }

        [Fact]
        public void StorageService_SaveAndLoad_ShouldPreserveDataIntegrity()
        {
            // Arrange
            var waiter = new Staff("Микола", "Офіціант");
            var order = new Order(waiter);
            order += new MenuItem("Тестова страва", 99.99m);
            var logs = new List<string> { "Тест запису" };

            // Act
            StorageService.SaveState(500.00m, logs, order);
            var loaded = StorageService.LoadState();

            // Assert
            Assert.NotNull(loaded);
            Assert.Equal(500.00m, loaded.TotalRevenue);
            Assert.Contains("Тест запису", loaded.SystemLogs);
            Assert.Single(loaded.CurrentDraftItems);
            Assert.Equal("Тестова страва", loaded.CurrentDraftItems[0].Name);
        }

        #endregion
    }
}
