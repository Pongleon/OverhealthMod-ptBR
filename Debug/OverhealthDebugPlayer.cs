#if DEBUG
using Terraria.GameInput;
using Terraria.ModLoader;

namespace OverhealthMod.Debug;

public class OverhealthDebugPlayer : ModPlayer
{
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (DebugSystem.MaxOverhealthKeybind.JustPressed)
            Player.statLife = Player.statLifeMax2 + Player.GetMaximumOverhealth();
    }
}

#endif