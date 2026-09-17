using System;
using BepInEx.Logging;
using HarmonyLib;

namespace StationeersRenderDistance;

internal static class Patcher
{
    internal static void Apply(Harmony harmony, ManualLogSource log)
    {
        TryPatch(log, "building occlusion", () =>
        {
            var methods = BuildingOcclusionPatches.TargetMethods();
            var postfix = AccessTools.Method(typeof(BuildingOcclusionPatches), nameof(BuildingOcclusionPatches.Postfix));
            var count = 0;
            foreach (var method in methods)
            {
                harmony.Patch(method, postfix: new HarmonyMethod(postfix));
                count++;
            }

            if (count == 0)
            {
                throw new InvalidOperationException("No GetRenderMaxDistanceSquared methods found.");
            }

            log.LogInfo($"Patched {count} GetRenderMaxDistanceSquared overrides.");
        });

        TryPatch(log, "atmospheric fire distance", () =>
        {
            var method = AccessTools.Method(typeof(Assets.Scripts.Objects.AtmosphericFire), nameof(Assets.Scripts.Objects.AtmosphericFire.IsEmitting));
            if (method == null)
            {
                throw new MissingMethodException("AtmosphericFire.IsEmitting");
            }

            harmony.Patch(method, postfix: new HarmonyMethod(typeof(AtmosphericFirePatches), nameof(AtmosphericFirePatches.Postfix)));
        });

        TryPatch(log, "thing fire emit distance", () =>
        {
            var method = AccessTools.Method(typeof(Objects.ThingFire), nameof(Objects.ThingFire.EmitThingFireParticles));
            if (method == null)
            {
                throw new MissingMethodException("ThingFire.EmitThingFireParticles");
            }

            harmony.Patch(method, transpiler: new HarmonyMethod(typeof(ThingFirePatches), nameof(ThingFirePatches.Transpiler)));
        });

        TryPatch(log, "particle culling", () =>
        {
            var method = AccessTools.Method(typeof(UnityEngine.ParticleSystem), nameof(UnityEngine.ParticleSystem.Play), Type.EmptyTypes);
            if (method == null)
            {
                throw new MissingMethodException("ParticleSystem.Play");
            }

            harmony.Patch(method, postfix: new HarmonyMethod(typeof(ParticleCullingPatches), nameof(ParticleCullingPatches.PlayPostfix)));
        });
    }

    private static void TryPatch(ManualLogSource log, string name, Action apply)
    {
        try
        {
            apply();
            if (ModConfig.Verbose)
            {
                log.LogInfo($"Applied patch: {name}");
            }
        }
        catch (Exception ex)
        {
            log.LogWarning($"Skipped patch '{name}': {ex.Message}");
        }
    }
}
