using UnityEngine;

namespace StationeersRenderDistance;

internal static class ParticleCullingPatches
{
    internal static void PlayPostfix(ParticleSystem __instance)
    {
        if (!ModConfig.EffectsOn || __instance == null)
        {
            return;
        }

        if (!ModConfig.AllParticles && !IsFireLikeName(__instance))
        {
            return;
        }

        var main = __instance.main;
        if (main.cullingMode != ParticleSystemCullingMode.AlwaysSimulate)
        {
            main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
        }
    }

    private static bool IsFireLikeName(ParticleSystem system)
    {
        var name = system.gameObject != null ? system.gameObject.name : system.name;
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return name.IndexOf("Fire", System.StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("Flame", System.StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("Smoke", System.StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("Steam", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
