
using Godot;
using System;

public partial class HPBar : Control
{
    // Reference to the ProgressBar node
    private ProgressBar _hpBar;

    public override void _Ready()
    {
        // Get the ProgressBar node
        _hpBar = GetNode<ProgressBar>("ProgressBar");
    }

    public void UpdateHealth(float current, float max)
    {
        _hpBar.MaxValue = max;
        _hpBar.Value = current;

        float healthPercent = current / max;

        Color barColor;
        // Interpolate color from green to yellow to red based on health percentage
        if (healthPercent > 0.5f)
        {
            // From green to yellow
            float t = (1.0f - healthPercent) * 2.0f;
            barColor = new Color(t, 1.0f, 0.0f);
        }
        else
        {
            // From yellow to red
            float t = healthPercent * 2.0f;
            barColor = new Color(1.0f, t, 0.0f);
        }

        // Apply the color to the ProgressBar
        _hpBar.Modulate = barColor;
    }
}