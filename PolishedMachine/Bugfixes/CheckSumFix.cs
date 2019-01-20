using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolishedMachine.Bugfixes
{
    public static class CheckSumFix
    {
        public static void Patch()
        {
            On.WorldChecksumController.ControlCheckSum += new On.WorldChecksumController.hook_ControlCheckSum(WorldCheckSumPatch);

        }

        public static bool WorldCheckSumPatch(On.WorldChecksumController.orig_ControlCheckSum orig, RainWorld.BuildType type)
        {
            return true;
        }
    }
}
