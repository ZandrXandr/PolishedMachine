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
            "ConfigMachine",
            "RustyMachine",
            "PolishedMachine",
            "Enum Extender"
        };

        /// <summary>
        /// Current SaveSlot.
        /// </summary>
        public static int slot
        {
            get { return pm.rainWorld.options.saveSlot; }
        }
        /// <summary>
        /// Currently Playing Slugcat.
        /// </summary>
        public static int slugcat
        {
            get { return pm.rainWorld.progression.PlayingAsSlugcat; }
        }

        /// <summary>
        /// Whether Config has changed in Config Menu or not
        /// </summary>
        public static bool configChanged;

        /// <summary>
        /// Runs right before MainMenu opens
        /// </summary>
        public static void Initialize()
        {

            loadedMods = Partiality.PartialityManager.Instance.modManager.loadedMods;
            loadedModsDictionary = new Dictionary<string, PartialityMod>();
            foreach (PartialityMod mod in loadedMods)
            {
                if (blackList.Contains<string>(mod.ModID)) { continue; } //No Config for this :P
                else if (mod.ModID.Substring(0, 1) == "_") { continue; } //Skip this mod from configuration
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
                PartialityMod blankMod = new PartialityMod
                {
                    ModID = "No Mods Installed",
                    Version = "XXXX",
                    author = "NULL"
                };
                loadedModsDictionary.Add(blankMod.ModID, blankMod);

                UnconfiguableOI itf = new UnconfiguableOI(blankMod, UnconfiguableOI.Reason.NoMod);
                loadedInterfaceDict.Add(blankMod.ModID, itf);
                loadedInterfaces.Add(itf);

                return;
            }

            //Load Mod Interfaces!
            foreach (KeyValuePair<string, PartialityMod> item in loadedModsDictionary)
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
                    itf = new UnconfiguableOI(itf.mod, new InvalidMenuObjAccessException(ex));
                    itf.Initialize();
                }

                if (itf.Tabs == null || itf.Tabs.Length < 1)
                {
                    itf = new UnconfiguableOI(itf.mod, new NoTabException(mod.ModID));
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
        public static void KillTabs()
        {
            foreach (KeyValuePair<string, OpTab> item in tabs)
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
                goto BackgroundUpdate;
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
                        bool h = false;
                        foreach (UIelement element in ConfigMenu.currentTab.items)
                        {
                            if (element.GetType().IsSubclassOf(typeof(UIconfig)))
                            {
                                if ((element as UIconfig).held) { h = true; element.Update(Time.deltaTime); continue; }
                            }
                        }
                        if (!h)
                        {
                            foreach (UIelement element in ConfigMenu.currentTab.items)
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

                    UnconfiguableOI newItf = new UnconfiguableOI(mod, new GenericUpdateException(fullLog));
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

        BackgroundUpdate:
            //Background running
            if (pm.currentMainLoop?.ID == ProcessManager.ProcessID.IntroRoll) { return; }
            /*
            foreach (OptionInterface oi in loadedInterfaces)
            {

            }
            */
        }

    }
}

