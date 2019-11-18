using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Partiality.Modloader;
using UnityEngine;
using RWCustom;
using CompletelyOptional;

namespace PolishedMachine.Config {
    public static class ConfigManager {

        /// <summary>
        /// Directory that all the data/configs is saved
        /// </summary>
        public static DirectoryInfo directory;
        /// <summary>
        /// GameObject for CompletelyOptional MonoBehavior
        /// </summary>
        public static GameObject go;
        /// <summary>
        /// CompletelyOptional MonoBehavior
        /// </summary>
        public static OptionScript script;

        public static Dictionary<string, string> songNameDict;
        private static readonly string[] playlistMoody =
        {
            "NA_07 - Phasing", //16%
            "NA_11 - Reminiscence",
            "NA_18 - Glass Arcs",
            "NA_19 - Halcyon Memories",
            "NA_21 - New Terra",

        };
        private static readonly string[] playlistWork =
        {
            "NA_20 - Crystalline", //7%
            "NA_29 - Flutter",
            "NA_30 - Distance"
        };

        /// <summary>
        /// List of Random Song gets played in ConfigMenu,
        /// carefully chosen by me :P
        /// </summary>
        public static string randomSong {
            get {
                if( UnityEngine.Random.value < 0.8f ) {
                    int num = Mathf.FloorToInt( UnityEngine.Random.value * 3f );
                    return playlistMoody[num];
                } else {
                    int num = Mathf.FloorToInt( UnityEngine.Random.value * 3f );
                    return playlistWork[num];
                }
            }
        }
        
        public static void Initialize()
        {
            directory = new DirectoryInfo(string.Concat(new object[] {
                Custom.RootFolderDirectory(),
                "ModConfigs",
                Path.DirectorySeparatorChar
                }));
            if (!directory.Exists) { directory.Create(); }

            On.Menu.OptionsMenu.UpdateInfoText += new On.Menu.OptionsMenu.hook_UpdateInfoText(OptionsMenuPatch.UpdateInfoTextPatch);
            On.Menu.OptionsMenu.Update += new On.Menu.OptionsMenu.hook_Update(OptionsMenuPatch.UpdatePatch);
            On.Menu.OptionsMenu.Singal += new On.Menu.OptionsMenu.hook_Singal(OptionsMenuPatch.SingalPatch);
            On.Menu.OptionsMenu.ShutDownProcess += new On.Menu.OptionsMenu.hook_ShutDownProcess(OptionsMenuPatch.ShutDownProcessPatch);

            go = new GameObject("OptionController");
            script = go.AddComponent<OptionScript>();
        }

    }
}