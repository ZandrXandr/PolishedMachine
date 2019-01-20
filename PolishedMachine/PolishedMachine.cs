using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Partiality.Modloader;
using PolishedMachine.Bugfixes;
using PolishedMachine.ModUtilities;
using PolishedMachine.Config;
using MonoMod.ModInterop;

namespace PolishedMachine {
    class PolishedMachine : PartialityMod {

        public static PolishedMachine modInstance;

        public BugfixManager bugfixManager;
        public ModUtilityManager utilityManager;
        public ConfigManager configManager;

        public override void Init() {
            base.Init();

            //Initialize mod stuff
            ModID = "PolishedMachine";
            author = "The Community!";

            modInstance = this;

            //Create mod part managers
            bugfixManager = new BugfixManager();
            utilityManager = new ModUtilityManager();
            configManager = new ConfigManager();

            typeof( PolishedMachineModInterop ).ModInterop();
        }

        public override void OnLoad() {

        }

        public override void OnEnable()
        {
            CheckSumFix.Patch();

        }

    }
}