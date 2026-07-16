using System;
using ItemChanger;
using RandomizerMod.IC;

namespace AnotherLocation.Rando
{
    internal static class RandoManager
    {
        public static void Hook()
        {
            ItemHandler.Hook();
        }

        public static bool IsRandoSave()
        {
            try
            {
                var rm = ItemChangerMod.Modules.Get<RandomizerModule>();
                return rm != null;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}