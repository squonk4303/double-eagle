using Godot;
using System;

public partial class OptionsPanel : Control
{
    // Exported nodes for easy access and customization
    [Export] private HSlider sensitivitySlider;
    [Export] private Label sensitivityValueLabel;
    [Export] private Button backButton;

    // Current mouse sensitivity value
    private float mouseSensitivity;

    // Signal to notify sensitivity change
    [Signal]
    public delegate void SensitivityChangedEventHandler(float value);

    public override void _Ready()
    {
        sensitivitySlider.MinValue = 0.1f;
        sensitivitySlider.MaxValue = 1.0f;
        sensitivitySlider.Step = 0.05f;

        LoadSettings();
        sensitivitySlider.Value = mouseSensitivity;
        UpdateSensitivityLabel(mouseSensitivity);
        
        sensitivitySlider.ValueChanged += OnSensitivityChanged;
        backButton.Pressed += OnBackPressed;
    }

    private void OnSensitivityChanged(double value)
    {
        mouseSensitivity = (float)value;
        UpdateSensitivityLabel(mouseSensitivity);
        EmitSignal(SignalName.SensitivityChanged, mouseSensitivity);
    }

    private void UpdateSensitivityLabel(float value)
    {
        if (sensitivityValueLabel != null)
            sensitivityValueLabel.Text = value.ToString("F2");
    }

    private void OnBackPressed()
    {
        SaveSettings();
        QueueFree();
    }

    // Load settings from ConfigFile (persistent storage)
    private void LoadSettings()
    {
        var config = new ConfigFile();
        if (config.Load("user://settings.cfg") == Error.Ok)
            mouseSensitivity = (float)config.GetValue("controls", "sensitivity", 0.5f);
    }

    // Save settings to config file (persistent storage)
    private void SaveSettings()
    {
        var config = new ConfigFile();
        config.SetValue("controls", "sensitivity", mouseSensitivity);
        config.Save("user://settings.cfg");
    }
}