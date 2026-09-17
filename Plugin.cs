using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StationeersMods.Interface;
using UnityEngine;

namespace StationeersRenderDistance;

[StationeersMod(PluginGuid, PluginName, PluginVersion)]
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "se.admin.RenderDistance";
    public const string PluginName = "StationeersRenderDistance";
    public const string PluginVersion = "0.2.0";

    internal static Plugin? Instance { get; private set; }
    internal static ManualLogSource Log { get; private set; } = null!;

    private Harmony? _harmony;
    private SettingsMenu? _menu;

    private void Awake()
    {
        Instance = this;
        Log = Logger;

        try
        {
            if (ClientGuard.IsDedicatedOrHeadless())
            {
                Log.LogInfo("Skipped: client-only");
                return;
            }

            ModConfig.Bind(Config);
            _harmony = new Harmony(PluginGuid);
            Patcher.Apply(_harmony, Log);
            _menu = new SettingsMenu();
            Log.LogInfo($"{PluginName} {PluginVersion} loaded (client). Press {ModConfig.MenuHotkey.Value} in-world for settings.");
        }
        catch (Exception ex)
        {
            Log.LogError($"Error during Awake: {ex}");
        }
    }

    private void Update()
    {
        _menu?.Update();
    }

    private void OnGUI()
    {
        _menu?.OnGUI();
    }
}

internal static class ClientGuard
{
    internal static bool IsDedicatedOrHeadless()
    {
        try
        {
            if (Application.isBatchMode)
            {
                return true;
            }
        }
        catch
        {
            // Unity Application may not be ready; keep checking game flags.
        }

        try
        {
            if (Assets.Scripts.GameManager.IsBatchMode)
            {
                return true;
            }
        }
        catch
        {
            // GameManager may not exist yet on a true client; do not skip.
        }

        return false;
    }
}
