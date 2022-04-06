namespace Weapon
{
    class Weapon
    {
        private int _bullets;

        public int Damage { get; }

        public void Fire()
        {
            _bullets--;
        }
    }

    class Player
    {
        private int _health;

        public void TakeDamage(int damage)
        {
            _health -= damage;
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
