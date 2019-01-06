using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolishedMachine.ModUtilities.Extenders;

namespace PolishedMachine.ModUtilities {
    public class ModUtilityManager {

        public CharacterExtender characterExtender;
        public ObjectExtender objectExtender;
        public CreatureExtender creatureExtender;

        public ModUtilityManager() {

            characterExtender = new CharacterExtender();
            objectExtender = new ObjectExtender();
            creatureExtender = new CreatureExtender();

        }

    }
}