using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using HarmonyLib;
using System.Reflection;

namespace ElectricPodLauncher
{
    [StaticConstructorOnStartup]
    static class ElectricPodLauncher_Patches
    {
        static ElectricPodLauncher_Patches()
        {
            DoPatches();
        }

        public static void DoPatches()
        {
            var harmony = new Harmony("ElectricPodLauncher");

            //prevents errors when Reuse Pods mod not present
            if (Verse.ModLister.HasActiveModWithName("Reuse Pods"))
            {
                //gets the methods to patch Building_PodFunnel
                var mOriginal = AccessTools.Method("Building_PodFunnel:getServiceCount");
                var mPostfix = typeof(ElectricPodLauncher_Patches).GetMethod("PodFunnelPostfix");
                
                if (mOriginal != null)
                {
                    //applies patch
                    var funnelPatch = new HarmonyMethod(mPostfix);
                    harmony.Patch(mOriginal, postfix: funnelPatch);
                }

                //gets the methods to patch ReusePods_Utils
                var fOriginal = AccessTools.Method("ReusePods_Utils:FindLaunchersWithinRadius");
                var fPostfix = typeof(ElectricPodLauncher_Patches).GetMethod("FindLaunchersPostfix");

                if (fOriginal != null)
                {
                    //applies patch
                    var findPatch = new HarmonyMethod(fPostfix);
                    harmony.Patch(fOriginal, postfix: findPatch);
                }
            }
        }

        [HarmonyPostfix]
        public static void PodFunnelPostfix(ref int __result, ref Building __instance)
        {
            //includes electric pod launchers in the pod funnel description
            int count = __result;
            foreach (Building item in __instance.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("PBL_ElectricPodLauncher")))
            {
                if (item.Position.DistanceTo(__instance.Position) <= 20f)
                {
                    count++;
                }
            }
            __result = count;
        }

        [HarmonyPostfix]
        public static void FindLaunchersPostfix(ref List<Building> __result, Map map, IntVec3 center, float radius)
        {
            //adds each electric pod launcher to the pod funnel list
            List<Building> outputList = __result;
            foreach (Building item in map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("PBL_ElectricPodLauncher")))
            {
                if (item.Position.DistanceTo(center) <= radius)
                {
                    outputList.Add(item);
                }
            }
            __result = outputList;
        }
    }
}
