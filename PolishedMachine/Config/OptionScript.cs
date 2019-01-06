using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Partiality.Modloader;
using System.IO;
using RWCustom;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using OptionalUI;
using Menu;
using On;
using PolishedMachine.Config;

namespace CompletelyOptional
{
    /// <summary>
    /// MonoBehavior part of CompletelyOptional Mod
    /// </summary>
    public class OptionScript : MonoBehaviour
    {
        public OptionScript()
        {
            init = false;
        }
        /// <summary>
        /// CompletelyOptional Mod Instance.
        /// </summary>
        public static ConfigManager manager;


        public static bool init = false;

        /// <summary>
        /// RainWorld Instance.
        /// </summary>
        public static RainWorld rw;
        /// <summary>
        /// ProcessManager Instance.
        /// </summary>
        public static ProcessManager pm;
        /// <summary>
        /// ConfigMenu Instance.
        /// </summary>
        public static ConfigMenu configMenu;

        /// <summary>
        /// Prevent Sound Engine from crashing by not letting sound when this is >100
        /// </summary>
        public static int soundFill;

        /// <summary>
        /// All loadedMods directly taken from Partiality ModManager.
        /// </summary>
        public static List<PartialityMod> loadedMods;
        /// <summary>
        /// Loaded Mod Dictionary without blacklisted Mods.
        /// Key: ModID, Value: PartialityMod Instance
        /// </summary>
        public static Dictionary<string, PartialityMod> loadedModsDictionary;
        /// <summary>
        /// List of OptionInterface Instances
        /// </summary>
        public static List<OptionInterface> loadedInterfaces;
        /// <summary>
        /// Loaded OptionInterface Instances.
        /// Key: ModID, Value: OI Instance
        /// </summary>
        public static Dictionary<string, OptionInterface> loadedInterfaceDict;

        /// <summary>
        /// Rain World/ModConfigs.
        /// </summary>
        public DirectoryInfo directory
        {
            get
            {
                return ConfigManager.directory;
            }
        }

        /// <summary>
        /// Blacklisted mod from config menu.
        /// Mostly because they are bug-fix mods.
        /// </summary>
        public static string[] blackList =
        {
            "CompletelyOptional",
            "CommunicationModule",
            "ConfigMachine",
            "RustyMachine",
            "PartialityFlatmode",
            "daddy_corruption_patch"
        };

        /// <summary>
        /// Current SaveSlot.
        /// </summary>
        public static int slot {
            get { return pm.rainWorld.options.saveSlot; }
        }

        /// <summary>
        /// Whether Config has changed in Config Menu or not
        /// </summary>
        public static bool configChanged;

        /// <summary>
        /// Runs right before MainMenu opens
        /// </summary>
        public void Initialize()
        {
            if (!directory.Exists) { directory.Create(); }

            loadedMods = Partiality.PartialityManager.Instance.modManager.loadedMods;
            loadedModsDictionary = new Dictionary<string, PartialityMod>();
            foreach(PartialityMod mod in loadedMods)
            {
                if (blackList.Contains<string>(mod.ModID)) { continue; } //No Config for this :P
                if (!loadedModsDictionary.ContainsKey(mod.ModID))
                {
                    loadedModsDictionary.Add(mod.ModID, mod);
                }
                else
                {
                    Debug.LogError(string.Concat("Duplicate ModID detected! Only one of them is loaded. (duplicated ID: ", mod.ModID, ")"));
                }
            }

            loadedInterfaceDict = new Dictionary<string, OptionInterface>();
            loadedInterfaces = new List<OptionInterface>();

            //No Mods Installed!
            if (loadedModsDictionary.Count == 0)
            {
                loadedModsDictionary = new Dictionary<string, PartialityMod>(1);
                PartialityMod blankMod = new PartialityMod();
                blankMod.ModID = "No Mods Installed";
                blankMod.Version = "XXXX";
                blankMod.author = "NULL";
                loadedModsDictionary.Add(blankMod.ModID, blankMod);

                UnconfiguableOI itf = new UnconfiguableOI(blankMod, UnconfiguableOI.Reason.NoMod);
                loadedInterfaceDict.Add(blankMod.ModID, itf);
                loadedInterfaces.Add(itf);

                return;
            }

            //Load Mod Interfaces!
            foreach(KeyValuePair<string, PartialityMod> item in loadedModsDictionary)
            {
                PartialityMod mod = loadedModsDictionary[item.Key];
                OptionInterface itf;

                if (!Regex.IsMatch(mod.ModID, "^[^\\/?%*:|\"<>/.]+$"))
                {
                    Debug.Log(string.Concat(new object[] {
                        mod.ModID,
                        " does not support CompletelyOptional: Invaild Mod ID!"
                    }));
                    itf = new UnconfiguableOI(mod, new Exception(string.Concat(mod.ModID, " is invaild ModID! Use something that can be used as folder name!")));
                    loadedInterfaces.Add(itf);
                    loadedInterfaceDict.Add(mod.ModID, itf);
                    continue;
                }


                try
                {
                    object obj = mod.GetType().GetMethod("LoadOI").Invoke(mod, new object[] { });
                    //Debug.Log(obj);
                    //itf = (OptionInterface)obj;
                    //itf = obj as OptionInterface;

                    if (obj.GetType().IsSubclassOf(typeof(OptionInterface)))
                    {
                        itf = obj as OptionInterface;
                        //Your code
                        Debug.Log(string.Concat(new object[] {
                        "Loaded OptionInterface from ",
                        mod.ModID,
                        " (type: ",
                        itf.GetType(),
                        ")"
                    }));
                    }
                    else
                    {
                        Debug.Log(string.Concat(new object[] {
                        mod.ModID,
                        " does not support CompletelyOptional: LoadOI returns what is not OptionInterface."
                        }));
                        itf = new UnconfiguableOI(mod, UnconfiguableOI.Reason.NoInterface);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(string.Concat(new object[] {
                        mod.ModID,
                        " does not support CompletelyOptional: ",
                        ex
                    }));
                    itf = new UnconfiguableOI(mod, UnconfiguableOI.Reason.NoInterface);
                }

                try
                {
                    itf.Initialize();
                }
                catch (Exception ex)
                {
                    itf = new UnconfiguableOI(itf.mod,
                        new Exception(string.Concat(itf.mod.ModID, " had issue in Initialize()!", Environment.NewLine,
                        "If you are accessing MenuObject in UIelements, make sure those don't run when IfConfigScreen is false.",
                        Environment.NewLine, ex
                        )));
                    itf.Initialize();
                }

                if (itf.Tabs == null || itf.Tabs.Length < 1)
                {
                    itf = new UnconfiguableOI(itf.mod, new Exception(string.Concat("TabIsNull: ", mod.ModID, " OI has No OpTabs! ",
                        Environment.NewLine, "Did you put base.Initialize() after your code?",
                        Environment.NewLine, "Leaving OI.Initialize() completely blank will prevent the mod from using LoadData/SaveData."
                        )));
                    //OptionScript.loadedInterfaceDict.Remove(mod.ModID);
                    //OptionScript.loadedInterfaceDict.Add(mod.ModID, itf);
                }

                bool loaded = itf.LoadConfig();
                if (loaded)
                {
                    Debug.Log(string.Concat("CompletelyOptional: ", mod.ModID, " config has been loaded."));
                }
                else
                {
                    Debug.Log(string.Concat("CompletelyOptional: ", mod.ModID, " does not have config.txt; Use default config."));
                }


                loadedInterfaces.Add(itf);
                loadedInterfaceDict.Add(mod.ModID, itf);
                //loadedModsDictionary[item.Key].GetType().GetMember("OI")
            }



        }


        /// <summary>
        /// List of Tabs in ConfigMenu
        /// </summary>
        public static Dictionary<string, OpTab> tabs;

        /// <summary>
        /// Unload All Tabs
        /// </summary>
        public void KillTabs()
        {
            foreach(KeyValuePair<string, OpTab> item in tabs)
            {
                tabs[item.Key].Unload();
            }
        }

        /// <summary>
        /// MonoBehavior Update
        /// </summary>
        public void Update()
        {
            if (!init)
            {
                rw = UnityEngine.Object.FindObjectOfType<RainWorld>();
                if (rw == null) { return; }
                pm = rw.processManager;
                if (pm.upcomingProcess == ProcessManager.ProcessID.MainMenu)
                {
                    try
                    {
                        Initialize();
                    }
                    catch (Exception ex) { Debug.LogError(ex); }
                    init = true;
                }
                ConfigMenu.currentTab = null;
                return;
            }
            if (pm == null)
            {
                rw = UnityEngine.Object.FindObjectOfType<RainWorld>();
                if (rw == null) { return; }
                pm = rw.processManager;
                return;
            }

            if (pm.currentMainLoop?.ID != ProcessManager.ProcessID.OptionsMenu)
            {
                if (pm.currentMainLoop?.ID == ProcessManager.ProcessID.Game && pm.arenaSitting?.levelPlaylist[0] == "SoundTest")
                { //Music Player!
                    //goto MusicPlayerUpdate;
                }
                return;
            }
            else if (!OptionsMenuPatch.mod)
            {
                return;
            }

            //Option is running
            ConfigMenu.script = this;

            if (ConfigMenu.currentTab != null)
            {
                ConfigMenu.description = "";
                ConfigMenu.menuTab.Update(Time.deltaTime);
                if (soundFill > 0) { soundFill--; }
                try
                {
                    if (!ConfigMenu.freezeMenu)
                    {
                        ConfigMenu.currentTab.Update(Time.deltaTime);
                    }
                    else
                    {
                        foreach (UIelement element in ConfigMenu.currentTab.items)
                        {
                            if (element.GetType().IsSubclassOf(typeof(UIconfig)))
                            {
                                if ((element as UIconfig).held) { element.Update(Time.deltaTime); continue; }
                            }
                            else
                            {
                                element.Update(Time.deltaTime);
                            }
                        }
                    }
                    ConfigMenu.currentInterface.Update(Time.deltaTime);
                }
                catch (Exception ex)
                { //Update Error Handle!
                    PartialityMod mod = ConfigMenu.currentInterface.mod;
                    List<Exception> unload = new List<Exception>();
                    ConfigMenu menu = (pm.currentMainLoop as ConfigMenu);
                    foreach (OpTab tab in ConfigMenu.currentInterface.Tabs)
                    {
                        try
                        {
                            tab.Hide();
                            tab.Unload();
                        }
                        catch (Exception ex0) { unload.Add(ex0); }
                    }
                    string fullLog = string.Concat(
                        mod.ModID, " had error in Update(Time.deltaTime)!", Environment.NewLine,
                        ex.ToString());
                    foreach (Exception ex0 in unload)
                    {
                        fullLog += Environment.NewLine + "TabUnloadError: " + ex0.ToString();
                    }

                    UnconfiguableOI newItf = new UnconfiguableOI(mod, new Exception(fullLog));
                    loadedInterfaceDict.Remove(mod.ModID);
                    loadedInterfaceDict.Add(mod.ModID, newItf);

                    int index = 0;
                    foreach (KeyValuePair<int, string> item in menu.modList)
                    {
                        if (item.Value == mod.ModID) { index = item.Key; break; }
                    }
                    int i = 0;
                    do
                    {
                        string key = string.Concat(index.ToString("D3") + "_" + i.ToString("D2"));
                        if (tabs.ContainsKey(key)) { tabs.Remove(key); }
                        else
                        {
                            break;
                        }
                        i++;
                    } while (i < 100);


                    newItf.Initialize();
                    ConfigMenu.selectedTabIndex = 0;
                    tabs.Add(string.Concat(index.ToString("D3") + "_00"), newItf.Tabs[0]);

                    foreach (UIelement element in newItf.Tabs[0].items)
                    {
                        foreach (MenuObject obj in element.subObjects)
                        {
                            menu.pages[0].subObjects.Add(obj);
                        }
                        menu.pages[0].Container.AddChild(element.myContainer);
                    }
                    newItf.Tabs[0].Show();

                    ConfigMenu.currentInterface = newItf;
                    ConfigMenu.currentTab = newItf.Tabs[0];

                    (pm.currentMainLoop as ConfigMenu).PlaySound(SoundID.MENU_Error_Ping);
                    (pm.currentMainLoop as ConfigMenu).opened = false;
                    (pm.currentMainLoop as ConfigMenu).OpenMenu();
                }
            }

            return;
            /*
            MusicPlayerUpdate:
            Debug.Log("!");

            game = pm.currentMainLoop as RainWorldGame;

            if (game.session.GetType() == typeof(SandboxGameSession))
            { //init
                SandboxGameSession oldSession = (game.session as SandboxGameSession);
                if ((game.session as SandboxGameSession)?.overlay != null)
                {
                    (game.session as SandboxGameSession).ProcessShutDown();
                }
                game.session = new SoundingGameSession(game);
                (game.session as SoundingGameSession).overlaySpawned = false;

                )
                if(pm.menuMic != null)
                {
                    pm.sideProcesses.Remove(pm.menuMic);
                    pm.menuMic = null;
                }
                pm.soundLoader.ReleaseAllUnityAudio();
                if (pm.musicPlayer != null)
                {
                    Destroy(pm.musicPlayer.gameObj);
                    pm.musicPlayer.song.StopAndDestroy();
                    pm.musicPlayer.multiplayerDJ.ShutDown();
                    pm.musicPlayer.ShutDownProcess();
                    pm.sideProcesses.Remove(pm.musicPlayer);
                }
                pm.musicPlayer = new MaxMusicPlayer(pm);
                pm.sideProcesses.Add(pm.musicPlayer);
                
                stinit = false;




            }

            if (!stinit)
            {
                //mimic Rustymachine
                Overseer overseer = null;
                this.room = null; oracle = null;
                List<Room> loadedRooms = new List<Room>();
                List<PhysicalObject> physObjects = new List<PhysicalObject>();
                loadedRooms.AddRange(game.world.activeRooms);
                foreach (Room r in loadedRooms)
                {
                    foreach (List<PhysicalObject> collection in r.physicalObjects)
                    {
                        physObjects.AddRange(collection);
                    }
                }
                foreach (PhysicalObject physicalObject in physObjects)
                {
                    if (physicalObject is Overseer)
                    {
                        overseer = physicalObject as Overseer;
                    }
                    else
                    {
                        physicalObject.Destroy();
                    }
                }
                if (overseer != null)
                {
                    this.room = overseer.room;

                    //oracle.room.game.Players[0].realizedCreature
                    game.session.Players = new List<AbstractCreature>();
                    AbstractCreature temp = new AbstractCreature(this.room.world, overseer.abstractCreature.creatureTemplate, null, new WorldCoordinate(room.abstractRoom.index, 15, 15, -1), room.game.GetNewID());
                    game.session.Players.Add(temp);
                    overseer.Destroy();
                    oracle = new MaxOracle(new AbstractPhysicalObject(this.room.world, AbstractPhysicalObject.AbstractObjectType.Oracle, null, new WorldCoordinate(room.abstractRoom.index, 15, 15, -1), room.game.GetNewID()), room);
                    this.room.AddObject(oracle);
                    this.room.waitToEnterAfterFullyLoaded = Math.Max(room.waitToEnterAfterFullyLoaded, 80);
                    temp.Destroy();
                    game.session.Players = new List<AbstractCreature>(0);


                    physObjects = new List<PhysicalObject>();
                    foreach (Room r in loadedRooms)
                    {
                        foreach (List<PhysicalObject> collection in r.physicalObjects)
                        {
                            physObjects.AddRange(collection);
                        }
                    }
                    foreach (PhysicalObject physicalObject in physObjects)
                    {
                        if ((physicalObject is OracleSwarmer))
                        {
                            physicalObject.Destroy();
                        }
                    }
                    oracle.mySwarmers = new List<OracleSwarmer>(0);

                    swarmers = new MaxSwarmer[12];
                    for (int t = 0; t < 12; t++)
                    {
                        swarmers[t] = new MaxSwarmer(new AbstractPhysicalObject(this.room.world, AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer, null, oracle.abstractPhysicalObject.pos, room.game.GetNewID()), room.world);
                        this.room.AddObject(swarmers[t]);
                    }




                    stinit = true;


                    (oracle.graphicsModule as MaxGraphics).halo.ChangeAllRadi();
                }

                return;
            }


            for (int n = 0; n < game.cameras.Length; n++)
            {
                //this.oracle.room.game.cameras[n].ChangeBothPalettes(25, 23, 0.80f * light);
            }


            if (game.pauseMenu != null)
            { //Return
                SoundTestToConfig();

            }


            return;
            */
            /*
            foreach (KeyValuePair<string, OpTab> item in tabs)
            {
                tabs[item.Key].Update(Time.deltaTime);
            }*/
        }

        /*
        /// <summary>
        /// SoundTest RainWorldGame
        /// </summary>
        public static RainWorldGame game;
        /// <summary>
        /// Whether SoundTest has been initiated or not
        /// </summary>
        public static bool stinit;
        /// <summary>
        /// SoundTest Room
        /// </summary>
        public Room room;

        //public static MaxOracle oracle;
        //public MaxSwarmer[] swarmers;
        
        public void SoundTestToConfig()
        {

            game.processActive = false;
            if (pm.menuMic != null)
            {
                pm.sideProcesses.Remove(pm.menuMic);
                pm.menuMic = null;
            }
            game.pauseMenu.ShutDownProcess();
            game.pauseMenu = null;
            for (int n = 0; n < game.cameras.Length; n++)
            {
                game.cameras[n].ClearAllSprites();
            }

            (game.session as SoundingGameSession).ProcessShutDown();

            HeavyTexturesCache.ClearRegisteredFutileAtlases();
            GC.Collect();
            Resources.UnloadUnusedAssets();

            game = null;
            oracle = null;
        }
        */

    }
}

