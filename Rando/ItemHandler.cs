using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using AnotherLocation.ConditionChecks;
using AnotherLocation.LocationSpecifics;
using AnotherLocation.SceneModifyingTags;
using ItemChanger;
using ItemChanger.Tags;
using Newtonsoft.Json;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using UnityEngine;

namespace AnotherLocation.Rando
{
    internal static class ItemHandler
    {
        private const string LocationKey = "East_Sanctum_Spike_Roof";

        private static int _objectsDefined = 0;
        private static int _addToPoolRunning = 0;
        private static int _subscribed = 0;
        private static readonly object _defineLock = new object();

        private static bool ShouldEnableAdvancedPrinceShrogoLocation(GenerationSettings gs)
        {
            return BugPrinceCheck.ShouldEnableBugPrinceAdvancedLocation()
                && ShriekPogoCheck.ShouldEnableShriekPogoLocations(gs);
        }

        public static void Hook()
        {
            if (Interlocked.CompareExchange(ref _objectsDefined, 1, 0) == 0)
            {
                DefineObjects();
            }

            if (Interlocked.CompareExchange(ref _subscribed, 1, 0) == 0)
            {
                RequestBuilder.OnUpdate.Subscribe(0f, rb =>
                {
                    try
                    {
                        if (ShouldEnableAdvancedPrinceShrogoLocation(rb.gs))
                            AddToPool(rb);
                    }
                    catch (Exception e)
                    {
                        Modding.Logger.LogError($"[AnotherLocation] AddToPool exception: {e}");
                        throw;
                    }
                });

                RCData.RuntimeLogicOverride.Subscribe(50f, (gs, lmb) =>
                {
                    try
                    {
                        if (ShouldEnableAdvancedPrinceShrogoLocation(gs))
                            InjectLogic(gs, lmb);
                    }
                    catch (Exception e)
                    {
                        Modding.Logger.LogError($"[AnotherLocation] InjectLogic exception: {e}");
                        throw;
                    }
                });

                RandoController.OnExportCompleted += _ =>
                {
                    lock (_defineLock)
                    {
                        try
                        {
                            DefineObjects();
                            Interlocked.Exchange(ref _objectsDefined, 1);
                        }
                        catch (Exception e)
                        {
                            Modding.Logger.LogError($"[AnotherLocation] Exception in DefineObjects during OnExportCompleted: {e}");
                        }
                    }
                };
            }
            
            SettingsLog.AfterLogSettings += AddFileSettings;
        }

        private static void DefineObjects()
        {
            lock (_defineLock)
            {
                try
                {
                    if (Finder.GetLocationInternal(LocationKey) == null)
                    {
                        var loc = new ShinyLocation(
                            LocationKey,
                            "Ruins1_25",
                            12.93f,
                            110.4f
                        );
                        var mapTag = loc.AddTag<InteropTag>();
                        mapTag.Message = "RandoSupplementalMetadata";
                        mapTag.Properties["ModSource"] = AnotherLocation.Instance.GetName();
                        mapTag.Properties["DoNotMakePin"] = true;
                        mapTag.Properties["PoolGroup"] = "Secret";//"Harmless Spikes"

                        var tag = loc.AddTag<SceneModifierTag>();
                        tag.SceneName = "Ruins1_25";

                        tag.HazardBoxes.Add(new ColliderBox
                        {
                            p1 = new Vector2(-1f, 104f),
                            p2 = new Vector2(0f, 129f),
                            Terrain = true,
                            Spikes = false
                        });

                        tag.HazardBoxes.Add(new ColliderBox
                        {
                            p1 = new Vector2(-2f, 104f),
                            p2 = new Vector2(-1f, 139f),
                            Terrain = false,
                            Spikes = false
                        });

                        tag.HazardRespawnTriggerPos = new Vector2(20.75f, 113.7f);
                        tag.HazardRespawnTriggerSize = new Vector2(5.5f, 6.6f);
                        tag.HazardRespawnMarkerPos = new Vector2(20.75f, 111.0f);
                        tag.RespawnRight = true;
                        Finder.DefineCustomLocation(loc);

                        //CitySpikeRoofFix.Hook();
                        //uncomment depending on public opinion
                    }
                }
                catch (Exception e)
                {
                    Modding.Logger.LogError($"[AnotherLocation] DefineObjects threw: {e}");
                    throw;
                }
            }
        }

        private static void AddToPool(RequestBuilder rb)
        {
            if (Interlocked.CompareExchange(ref _addToPoolRunning, 1, 0) == 1)
                return;

            try
            {
                if (!ShouldEnableAdvancedPrinceShrogoLocation(rb.gs))
                    return;

                rb.RemoveLocationByName(LocationKey);
                rb.AddLocationByName(LocationKey);
                rb.EditLocationRequest(LocationKey, info =>
                {
                    info.getLocationDef = () => new LocationDef
                    {
                        Name = LocationKey,
                        SceneName = "Ruins1_25",
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false
                    };
                });
            }
            catch (Exception e)
            {
                Modding.Logger.LogError($"[AnotherLocation] AddToPool threw: {e}");
                throw;
            }
            finally
            {
                Interlocked.Exchange(ref _addToPoolRunning, 0);
            }
        }

        private static void InjectLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            try
            {
                if (!ShouldEnableAdvancedPrinceShrogoLocation(gs))
                    return;

                Assembly asm = typeof(ItemHandler).Assembly;
                JsonLogicFormat jsonFmt = new JsonLogicFormat();

                Stream s = FindEmbeddedLocationsJson(asm);
                if (s == null)
                {
                    Modding.Logger.LogError("[AnotherLocation] Could not find embedded Locations.json");
                    return;
                }

                using (s)
                {
                    lmb.DeserializeFile(LogicFileType.Locations, jsonFmt, s);
                }
            }
            catch (Exception e)
            {
                Modding.Logger.LogError($"[AnotherLocation] InjectLogic threw: {e}");
                throw;
            }
        }
        private static Stream FindEmbeddedLocationsJson(Assembly asm)
        {
            var names = asm.GetManifestResourceNames();

            var name =
                names.FirstOrDefault(n => n.EndsWith("Rando.Locations.json", StringComparison.OrdinalIgnoreCase)) ??
                names.FirstOrDefault(n => n.EndsWith("Locations.json", StringComparison.OrdinalIgnoreCase));

            return name == null ? null : asm.GetManifestResourceStream(name);
        }
        //settings.txt log
        private static void AddFileSettings(LogArguments args, TextWriter tw)
        {
            if (!AnotherLocation.GlobalSettings.BugPrince)
                return;

            tw.WriteLine("Logging Another Location Settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, AnotherLocation.GlobalSettings);
            tw.WriteLine();
        }
    }
}