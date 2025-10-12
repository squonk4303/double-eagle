using Godot;
using System;

public partial class Health : Node
{
    [Signal]
    public delegate void HealthChangedEventHandler(float current, float max);

    [Signal]
    public delegate void DiedEventHandler();

    [Export]
    public float MaxHealth { get; set; } = 100f;

    private float _currentHealth;

    public float CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);

            if (_currentHealth <= 0)
            {
                EmitSignal(SignalName.Died);
            }
        }
    }

    public override void _Ready()
    {
        _currentHealth = MaxHealth;
        EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }

    public void ModifyHealth(float amount)
    {
        CurrentHealth += amount;
    }
}