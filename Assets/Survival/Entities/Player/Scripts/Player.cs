using System;

public class Player : Entity, IDamagable
{
    private Stat _health = new(100, 100);
    private Stat _stamina = new(100, 100);
    private Stat _sanity = new(100, 100);

    public IObservableStat Health => _health;
    public IObservableStat Stamina => _stamina;
    public IObservableStat Sanity => _sanity;

    public void Initialize(PlayerStats playerStats)
    {
        _health = new(playerStats.Health, playerStats.MaxHealth);
        _stamina = new(playerStats.Stamina, playerStats.MaxStamina);
        _sanity = new(playerStats.Sanity, playerStats.MaxSanity);
    }

    public void TakeDamage(float damage)
    {
        _health.Value -= damage;
    }
}
