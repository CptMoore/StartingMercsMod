
using System.Collections.Generic;
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

        [HarmonyPatch(typeof(SimGameState), "RequestDataManagerResources")]
        public static class SimGameStateRequestDataManagerResourcesPatch
        {
            public static void Prefix(SimGameState __instance)
            {
                settings.addRoninMercs.Do(roninId => {
                    __instance.DataManager.RequestResource(BattleTechResourceType.PilotDef, roninId);
                });
            }
        }

        [HarmonyPatch(typeof(SimGameState), "FirstTimeInitializeDataFromDefs")]
        public static class SimGameStateFirstTimeInitializeDataFromDefsPatch
        {
            public static void Postfix(SimGameState __instance)
            {
                settings.addRoninMercs.Do(roninId =>
                {
                    var pilotDef = __instance.DataManager.PilotDefs.Get(roninId);
                    __instance.AddPilotToRoster(pilotDef, true);
                });

                if (settings.addRandomMercsCount > 0)
                {
                    List<PilotDef> rononPilotDefs;
                    var pilotDefs = __instance.PilotGenerator.GeneratePilots(
                        settings.addRandomMercsCount,
                        settings.randomMercQuality,
                        settings.roninChance,
                        out rononPilotDefs);

                    pilotDefs.Do(pd => __instance.AddPilotToRoster(pd));
                }
            }
        }
    }
}
