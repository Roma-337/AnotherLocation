using System;
using System.Collections.Generic;
using Modding;
using UnityEngine;
using AnotherLocation.Rando;
using AnotherLocation.Interop;

namespace AnotherLocation
{
    public class AnotherLocation : Mod, IGlobalSettings<GlobalSettings>
    {
        internal static AnotherLocation Instance { get; private set; }

        private GlobalSettings globalSettings = new();

        public override string GetVersion() => "1.0.0.0";

        public static GlobalSettings GlobalSettings => Instance?.globalSettings;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                RandoManager.Hook();
                ConnectionMenu.Hook();
            }

            if (ModHooks.GetMod("RandoSettingsManager") is Mod)
            {
                RSM_Interop.Hook();
            }
        }

        public static bool IsRandoSave()
        {
            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                try
                {
                    return RandoManager.IsRandoSave();
                }
                catch (Exception ex)
                {
                    Instance?.Log($"[IsRandoSave] RandoManager.IsRandoSave threw: {ex.Message}");
                    return false;
                }
            }

            return false;
        }

        public void OnLoadGlobal(GlobalSettings s)
        {
            globalSettings = s ?? new GlobalSettings();
        }

        public GlobalSettings OnSaveGlobal()
        {
            return globalSettings;
        }

        private new void Log(object msg) => Modding.Logger.Log("[AnotherLocation] " + msg);
    }

    public class GlobalSettings
    {
        public bool BugPrince { get; set; } = false;
    }
}