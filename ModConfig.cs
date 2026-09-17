using System;
using BepInEx.Configuration;
using UnityEngine;

namespace StationeersRenderDistance;

internal static class ModConfig
{
    internal const float VanillaFireDistanceMeters = 50f;
    internal const float DistanceMin = 0f;
    internal const float DistanceMax = 2000f;

    internal static ConfigEntry<bool> BuildingsEnabled = null!;
    internal static ConfigEntry<float> StructureRenderDistance = null!;
    internal static ConfigEntry<float> SmallThingRenderDistance = null!;
    internal static ConfigEntry<bool> EffectsEnabled = null!;
    internal static ConfigEntry<float> ParticleRenderDistance = null!;
    internal static ConfigEntry<bool> AffectAllParticles = null!;
    internal static ConfigEntry<bool> LogPatches = null!;
    internal static ConfigEntry<KeyboardShortcut> MenuHotkey = null!;

    internal static bool BuildingsOn => BuildingsEnabled.Value;
    internal static bool EffectsOn => EffectsEnabled.Value;
    internal static bool AllParticles => AffectAllParticles.Value;
    internal static bool Verbose => LogPatches.Value;
    internal static float StructureMeters => StructureRenderDistance.Value;
    internal static float SmallThingMeters => SmallThingRenderDistance.Value;
    internal static float ParticleMeters => ParticleRenderDistance.Value;

    internal static void Bind(ConfigFile config)
    {
        BuildingsEnabled = config.Bind(
            "Buildings",
            "Enabled",
            true,
            "Raise structure / building occlusion distance on this client.");

        StructureRenderDistance = config.Bind(
            "Buildings",
            "StructureRenderDistance",
            500f,
            new ConfigDescription(
                "Minimum draw distance in meters for buildings (frames, walls, devices, cladding). Vanilla is used when it is already farther. 0 keeps vanilla.",
                new AcceptableValueRange<float>(0f, 2000f)));

        SmallThingRenderDistance = config.Bind(
            "Buildings",
            "SmallThingRenderDistance",
            0f,
            new ConfigDescription(
                "Minimum draw distance in meters for items, crates, and other draggable things. 0 keeps vanilla.",
                new AcceptableValueRange<float>(0f, 2000f)));

        EffectsEnabled = config.Bind(
            "Effects",
            "Enabled",
            true,
            "Raise fire / particle effect cull distance on this client.");

        ParticleRenderDistance = config.Bind(
            "Effects",
            "ParticleRenderDistance",
            300f,
            new ConfigDescription(
                "Minimum draw distance in meters for fire and (optionally) other particles. Vanilla fire is 50 m. Many distant fires is expensive.",
                new AcceptableValueRange<float>(0f, 2000f)));

        AffectAllParticles = config.Bind(
            "Effects",
            "AffectAllParticles",
            true,
            "When true, also relax Unity particle culling for smoke, steam, and other particle systems. When false, only fire-like effects are changed.");

        LogPatches = config.Bind(
            "General",
            "LogPatches",
            false,
            "Log each Harmony patch as it is applied. Useful if a game update breaks a method.");

        MenuHotkey = config.Bind(
            "General",
            "MenuHotkey",
            new KeyboardShortcut(KeyCode.F10),
            "Toggle the in-game settings popup. Change this if another mod already uses F10.");

        StructureRenderDistance.SettingChanged += OnDistanceSettingChanged;
        SmallThingRenderDistance.SettingChanged += OnDistanceSettingChanged;
        BuildingsEnabled.SettingChanged += OnDistanceSettingChanged;
    }

    internal static float ClampDistance(float meters)
    {
        if (meters < DistanceMin)
        {
            return DistanceMin;
        }

        if (meters > DistanceMax)
        {
            return DistanceMax;
        }

        return meters;
    }

    internal static float StructureDistanceSquared()
    {
        return ToSquared(StructureMeters);
    }

    internal static float SmallThingDistanceSquared()
    {
        return ToSquared(SmallThingMeters);
    }

    internal static float GetParticleDistanceSquared()
    {
        if (!EffectsOn)
        {
            return VanillaFireDistanceMeters * VanillaFireDistanceMeters;
        }

        var meters = Math.Max(VanillaFireDistanceMeters, ParticleMeters);
        return meters * meters;
    }

    internal static float ToSquared(float meters)
    {
        if (meters <= 0f)
        {
            return 0f;
        }

        return meters * meters;
    }

    private static void OnDistanceSettingChanged(object sender, EventArgs e)
    {
        try
        {
            var manager = Assets.Scripts.OcclusionManager.Instance;
            if (manager == null)
            {
                return;
            }

            manager.UpdateRenderDistanceMultiplier();
        }
        catch (Exception ex)
        {
            Plugin.Log.LogWarning($"Could not refresh occlusion after config change: {ex.Message}");
        }
    }
}
