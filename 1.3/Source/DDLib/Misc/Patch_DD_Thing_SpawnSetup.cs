using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    [HarmonyPatch(typeof(Thing), "SpawnSetup")]
    public static class DD_Thing_SpawnSetup
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> raw, ILGenerator generator)
        {
            FieldInfo stackCountField = AccessTools.Field(typeof(Thing), "stackCount");
            FieldInfo thingDefField = AccessTools.Field(typeof(Thing), "def");
            FieldInfo stackLimitField = AccessTools.Field(typeof(ThingDef), "stackLimit");
            MethodInfo hasModDef = AccessTools.Method(typeof(ThingDef), "HasModExtension", null, new Type[] { typeof(LegacyModExtension) });
            MethodInfo getModDef = AccessTools.Method(typeof(ThingDef), "GetModExtension", null, new Type[] { typeof(LegacyModExtension) });
            FieldInfo allowStackExceedField = AccessTools.Field(typeof(LegacyModExtension), "allowStackLimitExceed");

            Label? skipBranch = null;
            List<Predicate<CodeInstruction>> conds = new List<Predicate<CodeInstruction>>() {
                    inst => inst.IsLdarg(0),
                    inst => inst.LoadsField(stackCountField),
                    inst => inst.IsLdarg(0),
                    inst => inst.LoadsField(thingDefField),
                    inst => inst.LoadsField(stackLimitField),
                    inst => inst.Branches(out skipBranch)
                };

            List<CodeInstruction> ops = raw.ToList();

            for (int instI = 0; instI < ops.Count; instI++)
            {
                int match = 0;

                //Look forward.
                for (int condI = 0; condI < conds.Count && (instI + condI) < ops.Count; condI++)
                {
                    if (!conds[condI](ops[instI + condI]))
                    {
                        break;
                    }
                    match++;
                }

                //Look forward fully matched.
                if (match >= conds.Count && skipBranch.HasValue)
                {
                    Label truncateBranch = generator.DefineLabel();

                    List<CodeInstruction> patch = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, thingDefField),
                            new CodeInstruction(OpCodes.Callvirt, hasModDef),
                            new CodeInstruction(OpCodes.Brfalse_S, truncateBranch),
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, thingDefField),
                            new CodeInstruction(OpCodes.Callvirt, getModDef),
                            new CodeInstruction(OpCodes.Ldfld, allowStackExceedField),
                            new CodeInstruction(OpCodes.Brtrue_S, skipBranch)
                        };

                    while (!ops[instI].OperandIs(skipBranch.Value))
                    {
                        instI++;
                        if (instI >= ops.Count)
                        {
                            //Failed.
                            Log.Error("Patch failed.");
                            return raw;
                        }
                    }

                    ops[instI + 1].labels.Add(truncateBranch);
                    ops.InsertRange(instI + 1, patch);
                }
            }

            return ops;
        }
    }
}
