#if DEBUG
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace OverhealthMod.Debug;

public class DebugSystem : ModSystem
{
    public static ModKeybind MaxOverhealthKeybind { get; private set; }

    public override void Load()
    {
        MaxOverhealthKeybind = KeybindLoader.RegisterKeybind(Mod, "MaxOverhealth", Keys.N);
    }
}
#endif