using System;

namespace RestaurantSystem.Domain
{
    // Базовий клас для всіх помилок нашої системи ресторану
    public class RestaurantException : Exception
    {
        public RestaurantException() : base("Сталася непередбачена помилка в системі ресторану.") { }

        public RestaurantException(string message) : base(message) { }

        public RestaurantException(string message, Exception innerException) : base(message, innerException) { }
    }
}
