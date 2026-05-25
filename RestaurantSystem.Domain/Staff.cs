using System;

namespace RestaurantSystem.Domain
{
    public class Staff
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public Staff()
        {
            Id = Guid.NewGuid();
            Name = "Новий співробітник";
            Role = "Стажер";
            Console.WriteLine($"[CORE LOG] Співробітник {Id} створений за замовчуванням.");
        }

        public Staff(string name, string role)
        {
            Id = Guid.NewGuid();
            Name = name;
            Role = role;
            Console.WriteLine($"[CORE LOG] Співробітник {Id} ({Name}, {Role}) створений.");
        }

        public Staff(Staff other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Id = Guid.NewGuid();
            Name = other.Name;
            Role = other.Role;
            Console.WriteLine($"[CORE LOG] Скопійовано дані співробітника {Id}.");
        }

        ~Staff()
        {
            System.Diagnostics.Debug.WriteLine($"[GC LOG] Співробітник {Id} звільнив ресурси.");
        }
    }
}