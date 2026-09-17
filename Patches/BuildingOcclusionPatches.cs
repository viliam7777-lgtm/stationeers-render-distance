using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts;
using Assets.Scripts.Objects;
using HarmonyLib;
using UnityEngine;

namespace StationeersRenderDistance;

internal static class BuildingOcclusionPatches
{
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        var thingType = typeof(Thing);
        return AccessTools.GetTypesFromAssembly(thingType.Assembly)
            .Where(thingType.IsAssignableFrom)
            .Select(type => AccessTools.DeclaredMethod(type, nameof(Thing.GetRenderMaxDistanceSquared)))
            .Where(method => method != null)!;
    }

    internal static void Postfix(Thing __instance, ref float __result)
    {
        if (!ModConfig.BuildingsOn || __instance == null)
        {
            return;
        }

        var minimumSquared = GetMinimumSquared(__instance);
        if (minimumSquared <= 0f)
        {
            return;
        }

        __result = Mathf.Max(__result, minimumSquared);
    }

    private static float GetMinimumSquared(Thing thing)
    {
        if (thing is Structure)
        {
            return ModConfig.StructureDistanceSquared();
        }

        if (thing is Item || thing is DraggableThing)
        {
            return ModConfig.SmallThingDistanceSquared();
        }

        return 0f;
    }
}
