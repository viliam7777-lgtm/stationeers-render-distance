using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace StationeersRenderDistance;

internal static class ThingFirePatches
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var replacement = AccessTools.Method(typeof(ModConfig), nameof(ModConfig.GetParticleDistanceSquared));
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float value && value == 2500f)
            {
                yield return new CodeInstruction(OpCodes.Call, replacement).WithLabels(instruction.labels).WithBlocks(instruction.blocks);
                continue;
            }

            yield return instruction;
        }
    }
}
