using System;

namespace Weapon
{
    class Weapon
    {
        private int _bullets;

        public int Damage { get; }
        public bool IsEmpty => _bullets <= 0;

        public void Fire()
        {
            if (IsEmpty)
                Reload();
            else
                _bullets--;
        }

        private void Reload()
        {
            Console.WriteLine("Оружие перезаряжается.");
            Console.WriteLine("Оружие перезаряжено.");
            Fire();
        }
    }

    class Player
    {
        private int _health;

        public bool IsKilled => _health <= 0;

        public event Action Dead;

        public void TakeDamage(int damage)
        {
            _health -= damage;

            if (IsKilled)
                Dead?.Invoke();
        }
    }

    class Bot
    {
        private Weapon _weapon;

        public void OnSeePlayer(Player player)
        {
            _weapon.Fire();
            player.TakeDamage(_weapon.Damage);
        }
    }
}
