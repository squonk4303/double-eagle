
using Godot;
using System;

public partial class HPBar : Control
{
    private ProgressBar _hpBar;

    public override void _Ready()
    {
        _hpBar = GetNode<ProgressBar>("ProgressBar");
    }

    public void UpdateHealth(float current, float max)
    {
        _hpBar.MaxValue = max;
        _hpBar.Value = current;

        float healthPercent = current / max;

        Color barColor;
        if (healthPercent > 0.5f)
        {
            float t = (1.0f - healthPercent) * 2.0f;
            barColor = new Color(t, 1.0f, 0.0f);
        }
        else
        {
            float t = healthPercent * 2.0f;
            barColor = new Color(1.0f, t, 0.0f);
        }

        _hpBar.Modulate = barColor;
    }
}