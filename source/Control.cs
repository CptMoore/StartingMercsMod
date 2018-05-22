
using System;
using System.Collections.Generic;
using System.Linq;
using HBS.Logging;
using Harmony;
using System.Reflection;
using BattleTech;
using DynModLib;

namespace StartingMercsMod
{
    public class StartingMercsModSettings : ModSettings
    {
        public int addRandomMercsCount = 0;
        public int randomMercQuality = 1; //1-5
        public float roninChance = 0.08f;
        public string[] addRoninMercs = { };
        public bool addRoninsOnSavegameLoad = false;
        public bool allowMultipleRoninCopies = false;
    }

    public static class Control
    {
        public static Mod mod;

        public static StartingMercsModSettings settings = new StartingMercsModSettings();

        public static void Start(string modDirectory, string json)
        {
            mod = new Mod(modDirectory);
            Logger.SetLoggerLevel(mod.Logger.Name, LogLevel.Log);

            mod.LoadSettings(settings);
			
			var harmony = HarmonyInstance.Create(mod.Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // logging output can be found under BATTLETECH\BattleTech_Data\output_log.txt
            // or also under yourmod/log.txt
            mod.Logger.Log("Loaded " + mod.Name);
        }

        // starting a new game
        [HarmonyPatch(typeof(SimGameState), "FirstTimeInitializeDataFromDefs")]
        public static class SimGameStateFirstTimeInitializeDataFromDefsPatch
        {
            public static void Postfix(SimGameState __instance)
            {
                __instance.AddRoninsToRoster();
                __instance.AddRandomMercsToRoster();
                __instance.DumpRoster();
            }
        }

        // loading up a save game
        [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
        public static class SimGameStateRehydratePatch
        {
            public static void Postfix(SimGameState __instance)
            {
                if (!settings.addRoninsOnSavegameLoad)
                {
                    return;
                }

                __instance.AddRoninsToRoster();
                __instance.DumpRoster();
            }
        }

        public static void AddRoninsToRoster(this SimGameState sim)
        {
            settings.addRoninMercs.Do(roninId =>
            {
                if (settings.allowMultipleRoninCopies)
                {
                    sim.UsedRoninIDs.Remove(roninId);
                }
                else if (sim.UsedRoninIDs.Contains(roninId))
                {
                    mod.Logger.Log("Not adding pilot " + roninId + " to roster, as pilot was already hired");
                    return;
                }

                mod.Logger.Log("Adding pilot " + roninId + " to roster");
                sim.AddPilotToRoster(roninId);
            });
        }

        public static void AddRandomMercsToRoster(this SimGameState sim)
        {
            if (settings.addRandomMercsCount <= 0)
            {
                return;
            }

            List<PilotDef> roninPilotDefs;
            var pilotDefs = sim.PilotGenerator.GeneratePilots(
                settings.addRandomMercsCount * 2,
                settings.randomMercQuality,
                settings.roninChance,
                out roninPilotDefs);

            var all = pilotDefs.Union(roninPilotDefs).ToList();
            all.Shuffle();

            foreach (var pd in all.Take(settings.addRandomMercsCount))
            {
                mod.Logger.Log("Adding pilot " + pd.Description.Id + " to roster");
                sim.AddPilotToRoster(pd);
            }
        }

        public static void DumpRoster(this SimGameState sim)
        {
            sim.PilotRoster.Do(pilot => mod.Logger.LogDebug("Roster contains pilot " + pilot.Name));
        }
    }
}
