using Assets.Scripts;
using Assets.Scripts.GridSystem;
using BepInEx.Configuration;
using UnityEngine;

namespace StationeersRenderDistance;

internal sealed class SettingsMenu
{
    private const int WindowId = 0x52444D31;
    private bool _visible;
    private Rect _window = new(80f, 80f, 440f, 390f);
    private CursorLockMode _savedLock;
    private bool _savedCursorVisible;
    private bool _cursorOverridden;

    internal void Update()
    {
        if (!IsWorldReady())
        {
            if (_visible)
            {
                Close();
            }

            return;
        }

        if (ModConfig.MenuHotkey.Value.IsDown())
        {
            Toggle();
        }
    }

    internal void OnGUI()
    {
        if (!_visible)
        {
            return;
        }

        _window = GUI.Window(WindowId, _window, DrawWindow, "Render Distance");
    }

    private void Toggle()
    {
        if (_visible)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Open()
    {
        _visible = true;
        _savedLock = Cursor.lockState;
        _savedCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _cursorOverridden = true;
    }

    private void Close()
    {
        _visible = false;
        if (_cursorOverridden)
        {
            Cursor.lockState = _savedLock;
            Cursor.visible = _savedCursorVisible;
            _cursorOverridden = false;
        }
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Distances (meters). 0 = vanilla for that category.");

        DrawSlider("Buildings", ModConfig.StructureRenderDistance);
        DrawSlider("Small things", ModConfig.SmallThingRenderDistance);
        DrawSlider("Particles / fire", ModConfig.ParticleRenderDistance);

        GUILayout.Space(8f);
        GUILayout.Label("Affected objects");
        ModConfig.BuildingsEnabled.Value = GUILayout.Toggle(ModConfig.BuildingsEnabled.Value, " Buildings");
        ModConfig.EffectsEnabled.Value = GUILayout.Toggle(ModConfig.EffectsEnabled.Value, " Effects");
        ModConfig.AffectAllParticles.Value = GUILayout.Toggle(
            ModConfig.AffectAllParticles.Value,
            " All particles (off = fire-like names only)");

        GUILayout.Space(8f);
        GUILayout.Label("Many distant fires is expensive. Keep particle range lower than buildings if FPS drops.");
        GUILayout.Label($"Hotkey: {ModConfig.MenuHotkey.Value}");

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
    }

    private static void DrawSlider(string label, ConfigEntry<float> entry)
    {
        var value = entry.Value;
        GUILayout.Label($"{label}: {value:0} m");
        var next = GUILayout.HorizontalSlider(value, ModConfig.DistanceMin, ModConfig.DistanceMax);
        next = ModConfig.ClampDistance(Mathf.Round(next));
        if (!Mathf.Approximately(next, value))
        {
            entry.Value = next;
        }
    }

    private static bool IsWorldReady()
    {
        try
        {
            var state = GameManager.GameState;
            return state == GameState.Running || state == GameState.Paused;
        }
        catch
        {
            return false;
        }
    }
}
