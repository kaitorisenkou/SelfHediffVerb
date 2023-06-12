using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace SelfHediffVerb {
    [StaticConstructorOnStartup]
    public class SelfHediffVerb {
        static SelfHediffVerb() {
            Log.Message("[SelfHediffVerb] Now active");
            var harmony = new Harmony("kaitorisenkou.SelfHediffVerb");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[SelfHediffVerb] Harmony patch complete!");
        }
    }
    [HarmonyPatch(typeof(Verb), nameof(Verb.EquipmentSource), MethodType.Getter)]
    [HarmonyAfter("kaitorisenkou.ModularWeapons")]
    public static class Patch_VerbEquipmentSource {
        [HarmonyPostfix]
        public static void Postfix(ref Verb __instance, ref ThingWithComps __result) {
            if (__result != null) return;
            var comp = __instance.DirectOwner as CompVerbWithCooltime;
            if (comp != null) __result = comp.parent;
        }
    }
}
