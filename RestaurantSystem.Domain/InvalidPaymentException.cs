using System;

namespace RestaurantSystem.Domain
{
    public class InvalidPaymentException : RestaurantException
    {
        public decimal AttemptedAmount { get; private set; }

        public InvalidPaymentException(decimal amount)
            : base($"Фінансова помилка: Спроба провести оплату на некоректну суму ({amount} грн).")
        {
            AttemptedAmount = amount;
        }
    }
}
