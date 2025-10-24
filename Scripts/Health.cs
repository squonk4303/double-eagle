using Godot;
using System;

public partial class Health : Node
{
    // Signal to notify when health changes
    [Signal]
    public delegate void HealthChangedEventHandler(float current, float max);

    // Signal to notify when the entity dies
    [Signal]
    public delegate void DiedEventHandler();

    // Maximum health value
    [Export]
    public float MaxHealth { get; set; } = 30.0f;

    // Backing field for current health
    private float _currentHealth;

    // Current health value with clamping and signal emission
    public float CurrentHealth
    {
        // Getter and private setter
        get => _currentHealth;
        private set
        {
            // Clamp the health value between 0 and MaxHealth
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            // Emit health changed signal
            EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);

            if (_currentHealth <= 0)
            {
                // Emit died signal
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
}
