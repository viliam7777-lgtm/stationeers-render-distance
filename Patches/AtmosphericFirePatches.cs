using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects;

namespace StationeersRenderDistance;

internal static class AtmosphericFirePatches
{
    internal static void Postfix(AtmosphericFire __instance, ref bool __result)
    {
        if (__result || !ModConfig.EffectsOn || __instance == null)
        {
            return;
        }

        if (!__instance.IsValid)
        {
            return;
        }

        var atmosphere = __instance.Atmosphere;
        if (atmosphere == null || atmosphere.Mode != AtmosphereHelper.AtmosphereMode.World)
        {
            return;
        }

        if (atmosphere.SquareDistanceToPlayer > ModConfig.GetParticleDistanceSquared())
        {
            return;
        }

        if (atmosphere.FuelBurnedRatio <= AtmosphericFire.MinCombustionForFlameFX)
        {
            return;
        }

        __result = true;
    }
}
