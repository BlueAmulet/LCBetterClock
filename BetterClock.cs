using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace BetterClock
{
    [BepInPlugin(ID, Name, Version)]
    public class BetterClock : BaseUnityPlugin
    {
        internal const string Name = "BetterClock";
        internal const string Author = "BlueAmulet";
        internal const string ID = Author + "." + Name;
        internal const string Version = "1.0.3";

        public void Awake()
        {
            Settings.InitConfig(Config);
            Harmony harmony = new Harmony(ID);
            Logger.LogInfo("Applying Harmony patches");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            int patchedMethods = 0;
            foreach (MethodBase method in harmony.GetPatchedMethods())
            {
                Logger.LogInfo("Patched " + method.DeclaringType.Name + "." + method.Name);
                patchedMethods++;
            }
            Logger.LogInfo(patchedMethods + " patches applied");
        }
    }
}
