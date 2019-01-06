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
    public class ConfigManager {

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

        public static bool soundTestEnabled = false;

        public static Dictionary<string, string> songNameDict;

        public ConfigManager() {
            directory = new DirectoryInfo( string.Concat( new object[] {
                Custom.RootFolderDirectory(),
                "ModConfigs",
                Path.DirectorySeparatorChar
            } ) );

            On.Menu.OptionsMenu.UpdateInfoText += new On.Menu.OptionsMenu.hook_UpdateInfoText( OptionsMenuPatch.UpdateInfoTextPatch );
            On.Menu.OptionsMenu.Update += new On.Menu.OptionsMenu.hook_Update( OptionsMenuPatch.UpdatePatch );
            On.Menu.OptionsMenu.Singal += new On.Menu.OptionsMenu.hook_Singal( OptionsMenuPatch.SingalPatch );
            On.Menu.OptionsMenu.ShutDownProcess += new On.Menu.OptionsMenu.hook_ShutDownProcess( OptionsMenuPatch.ShutDownProcessPatch );


            go = new GameObject( "OptionController" );
            script = go.AddComponent<OptionScript>();
            OptionScript.manager = this;

            if( File.Exists( string.Concat( levelpath, "SoundTest.txt" ) ) ) {
                RemoveMusicRoom();
            }
        }


        private static string[] playlistMoody =
        {
            "NA_07 - Phasing", //16%
            "NA_11 - Reminiscence",
            "NA_18 - Glass Arcs",
            "NA_19 - Halcyon Memories",
            "NA_21 - New Terra",

        };
        private static string[] playlistWork =
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

        /// <summary>
        /// Path of Levels
        /// </summary>
        public static string levelpath = string.Concat( new object[] {
            Custom.RootFolderDirectory(),
            "Levels",
            Path.DirectorySeparatorChar
        } );
        /// <summary>
        /// Creates SoundTest Room files
        /// </summary>
        public static void CopyMusicRoom() {
            string sspath = string.Concat( new object[] {
                "World",
                Path.DirectorySeparatorChar,
                "Regions",
                Path.DirectorySeparatorChar,
                "SS",
                Path.DirectorySeparatorChar,
                "Rooms",
                Path.DirectorySeparatorChar
            } );

            File.Copy( string.Concat( sspath, "SS_AI.txt" ), string.Concat( levelpath, "SoundTest.txt" ), true );
            File.Copy( string.Concat( sspath, "SS_AI_1.png" ), string.Concat( levelpath, "SoundTest_1.png" ), true );


            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "CompletelyOptional.SoundTest.MaxRoom_Settings.txt";
            string result;

            /*
            string[] names = assembly.GetManifestResourceNames();
            foreach(string name in names)
            {
                Debug.Log(name);
            }*/

            using( Stream stream = assembly.GetManifestResourceStream( resourceName ) )
            using( StreamReader reader = new StreamReader( stream ) ) {
                result = reader.ReadToEnd();
            }

            File.WriteAllText( string.Concat( levelpath, "SoundTest_Settings.txt" ), result );

        }
        /// <summary>
        /// Removes SoundTest room files to prevent unwanted predicaments
        /// </summary>
        public static void RemoveMusicRoom() {
            File.Delete( string.Concat( levelpath, "SoundTest.txt" ) );
            File.Delete( string.Concat( levelpath, "SoundTest_1.png" ) );
            File.Delete( string.Concat( levelpath, "SoundTest_Settings.txt" ) );
        }

    }
}