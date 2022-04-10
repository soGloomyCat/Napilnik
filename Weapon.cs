using System;

namespace Weapon
{
    class Weapon
    {
        private int _bullets;

        public int Damage { get; }

        public void TryShoot()
        {
            if (CheckAmmo())
                Fire();
            else
                Reload();
        }

        private void Fire()
        {
            _bullets--;
        }

        private bool CheckAmmo()
        {
            if (_bullets > 0)
                return true;
            else
                return false;
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

        private bool _isKilled => CheckDead();

        public event Action IsDead;

        public void TakeDamage(int damage)
        {
            _health -= damage;

            if (_isKilled)
                IsDead?.Invoke();
        }

        private bool CheckDead()
        {
            if (_health <= 0)
                return true;
            else
                return false;
        }
    }

    class Bot
    {
        private Weapon _weapon;

        public void OnSeePlayer(Player player)
        {
            _weapon.TryShoot();
            player.TakeDamage(_weapon.Damage);
        }
    }
}
