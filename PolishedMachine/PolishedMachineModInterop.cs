using PolishedMachine.ModUtilities;
using PolishedMachine.ModUtilities.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PolishedMachine {
    class PolishedMachineModInterop {

        #region Character Extender
        public static int RegisterNewSlugcat(string playerName, Type newPlayerType) {
            return PolishedMachine.modInstance.utilityManager.characterExtender.RegisterNewSlugcat(playerName, newPlayerType);
        }
        public static void RegisterSlugcatArt(int slugcatID, string artElementName) {
            CharacterExtender.instance.SetSlugcatArt( slugcatID, artElementName );
        }
        #endregion

        #region Input Helper
        public static void RegisterCorrectKeyInput(KeyCode code, Action downKey) {
            UtilityScript.RegisterInputChecker( code, downKey );
        }
        public static void RemoveCorrectKeyInput(KeyCode code, Action downKey) {
            UtilityScript.RemoveInputChecker( code, downKey );
        }
        #endregion
    }
}
