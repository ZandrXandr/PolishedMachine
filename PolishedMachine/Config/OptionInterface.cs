using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Reflection;
using OptionalUI;
using Partiality.Modloader;
using System;
using System.Text.RegularExpressions;
using RWCustom;
using PolishedMachine.Config;

namespace OptionalUI
{
    /// <summary>
    /// To Interact with CompletelyOptional Mod
    /// Check if PartialityMod file has this class
    /// </summary>
    public class OptionInterface
    {
        /// <summary>
        /// Option Interface for Partiality Mod/Patch.
        /// Create public static [YourOIclass] LoadOI() in your PartialityMod.
        /// CompletelyOptional will load your OI after Intro Roll.
        /// </summary>
        /// <param name="mod">Your Partiality mod.</param>
        public OptionInterface(PartialityMod mod)
        {
            this.mod = mod;
            this.rawConfig = "Unconfiguable";
            instance = this;
        }

        private static OptionInterface instance;

        /// <summary>
        /// Returns whether CompletelyOptional is loaded or not.
        /// </summary>
        public static bool ConfigModExist()
        {
            List<PartialityMod> loadedMods = Partiality.PartialityManager.Instance.modManager.loadedMods;
            foreach (PartialityMod mod in loadedMods)
            {
                if (mod.ModID == "CompletelyOptional") { return true; }
            }
            _slot = slot;
            return false;
        }

        /// <summary>
        /// Is this ConfigScreen or not.
        /// If you are editing MenuObject in UIelements,
        /// make sure that those codes don't run when this is false.
        /// Or it will throw NullRefException.
        /// </summary>
        public static bool IsConfigScreen
        {
            get { return OptionScript.init; }
        }

        /// <summary>
        /// How much the Sound Engine is full.
        /// </summary>
        public static int soundFill
        {
            get
            {
                return OptionScript.soundFill;
            }
            set
            {
                OptionScript.soundFill = value;
            }
        }

        /// <summary>
        /// Whether the Sound Engine is full or not.
        /// </summary>
        public static bool soundFilled
        {
            get
            {
                return soundFill > 80;
            }
        }


        /// <summary>
        /// Whether the mod is configuable or not.
        /// You can just replace this to 'return true;' or false to save computing time.
        /// </summary>
        /// <returns>Whether the mod is configuable or not</returns>
        public virtual bool Configuable()
        {
            if (!OptionScript.init) { return false; }
            Dictionary<string, string> temp = GrabConfig();
            if (temp.Count > 0) { return true; }
            else { return false; }
        }


        /// <summary>
        /// The Mod using this OptionInterface.
        /// </summary>
        public PartialityMod mod;

        /// <summary>
        /// OpTab that contains UIelements for your config screen.
        /// Do something like 'Tabs = new OpTab[count]' in your Initialize().
        /// </summary>
        public OpTab[] Tabs;

        /// <summary>
        /// Default Save Data of this mod. If this isn't needed, just leave it be.
        /// </summary>
        public virtual string defaultData
        {
            get { return string.Empty; }
        }

        private DirectoryInfo directory
        {
            get
            {
                return
                    new DirectoryInfo(string.Concat(new object[] {
                    ConfigManager.directory.FullName,
                    mod.ModID,
                    Path.DirectorySeparatorChar
                    }));
            }
        }
        /// <summary>
        /// Currently selected saveslot
        /// </summary>
        public static int slot { get { return OptionScript.slot; } }
        private static int _slot;
        /// <summary>
        /// Currently selected slugcat
        /// </summary>
        public static int slugcat { get { return OptionScript.slugcat; } }
        private static int _slugcat;


        /// <summary>
        /// Config Data. Key is the key you set when making UIconfigs in Initialize().
        /// Value is the value of those configs, in string form.
        /// </summary>
        public static Dictionary<string, string> config;
        private string rawConfig;

        /// <summary>
        /// This will be called by CompletelyOptional modmanager.
        /// You don't need to worry about managing config.
        /// </summary>
        public bool LoadConfig()
        {
            config = new Dictionary<string, string>();
            rawConfig = "Unconfiguable";
            if (!directory.Exists)
            {
                directory.Create(); return false;
            }

            string path = string.Concat(new object[] {
            directory.FullName,
            "config.txt"
            });
            if (File.Exists(path))
            {
                try
                {
                    string txt = File.ReadAllText(path, Encoding.UTF8);
                    string key = txt.Substring(0, 32);
                    txt = txt.Substring(32, txt.Length - 32);
                    if (Custom.Md5Sum(txt) != key)
                    {
                        Debug.Log(string.Concat(mod.ModID, " config file has been tinkered! Load Default Config instead."));
                        return false;
                    }

                    this.rawConfig = Crypto.DecryptString(txt, string.Concat("OptionalConfig " + mod.ModID));
                }
                catch
                {
                    Debug.Log(new LoadDataException(string.Concat(mod.ModID, " config file has been corrupted! Load Default Config instead.")));
                    return false;
                }

            }
            else
            {
                //load default config from dictionary and save.
                return false;
            }

            //Set Initialized stuff to new value
            Dictionary<string, string> loadedConfig = new Dictionary<string, string>();

            //convert to dictionary
            //<CfgC>key<CfgB><CfgD>value<CfgA>
            string[] array = Regex.Split(this.rawConfig, "<CfgA>");

            for (int i = 0; i < array.Length; i++)
            { //<CfgC>key<CfgB><CfgD>value
                string key = string.Empty;
                string value = string.Empty;
                string[] array2 = Regex.Split(array[i], "<CfgB>");
                for (int j = 0; j < array2.Length; j++)
                { //<CfgC>key || <CfgD>value
                    if (array2[j].Length < 6) { continue; }
                    if (array2[j].Substring(0, 6) == "<CfgC>")
                    {
                        key = array2[j].Substring(6, array2[j].Length - 6);
                    }
                    else if (array2[j].Substring(0, 6) == "<CfgD>")
                    {
                        value = array2[j].Substring(6, array2[j].Length - 6);
                    }
                }
                if (string.IsNullOrEmpty(key))
                { //?!?!
                    continue;
                }
                loadedConfig.Add(key, value);
            }

            config = loadedConfig;

            try
            {
                ConfigOnChange();
            }
            catch (Exception e)
            {
                Debug.Log("CompletelyOptional: Lost backward capability in Config! Reset Config.");
                Debug.Log(new LoadDataException(e.ToString()));
                File.Delete(path);
                config = new Dictionary<string, string>();
                rawConfig = "Unconfiguable";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Displaying loaded config to config menu. You won't use this.
        /// </summary>
        public void ShowConfig()
        {
            GrabObject();
            if (!(config?.Count > 0)) { return; } //Nothing Loaded.
            foreach (KeyValuePair<string, string> setting in config)
            {
                if (objectDictionary.TryGetValue(setting.Key, out UIconfig obj))
                {
                    obj.value = setting.Value;
                }
                else
                {
                    Debug.Log(string.Concat(mod.ModID, " setting has been removed. (key: ", setting.Key, " / value: ", setting.Value, ")"));
                }
            }
        }

        private void GrabObject()
        {
            Dictionary<string, UIconfig> displayedConfig = new Dictionary<string, UIconfig>();
            for (int i = 0; i < this.Tabs.Length; i++)
            {
                Dictionary<string, UIconfig> tabDictionary = this.Tabs[i].GetTabObject();
                if (tabDictionary.Count < 1) { continue; }

                foreach (KeyValuePair<string, UIconfig> item in tabDictionary)
                {
                    if (item.Value.cosmetic) { continue; }
                    if (displayedConfig.ContainsKey(item.Key)) { throw new DupelicateKeyException(string.Empty, item.Key); }
                    displayedConfig.Add(item.Key, item.Value);
                }
            }
            this.objectDictionary = displayedConfig;
        }

        /// <summary>
        /// Grabbing config from config menu. You won't use this.
        /// </summary>
        public Dictionary<string, string> GrabConfig()
        {
            GrabObject();
            if (this.objectDictionary.Count < 1) { return new Dictionary<string, string>(0); }
            Dictionary<string, string> displayedConfig = new Dictionary<string, string>();
            foreach (KeyValuePair<string, UIconfig> setting in this.objectDictionary)
            {
                if (setting.Value.cosmetic) { continue; }
                displayedConfig.Add(setting.Key, setting.Value.value);
            }
            return displayedConfig;
        }

        /// <summary>
        /// Saving Config. You don't need to worry about this.
        /// </summary>
        public bool SaveConfig(Dictionary<string, string> newConfig)
        {
            if (newConfig.Count < 1) { return false; } //Nothing to Save.
            config = newConfig;
            ConfigOnChange();

            this.rawConfig = string.Empty;
            //convert to raw data
            //<CfgC>key<CfgB><CfgD>value<CfgA>

            foreach (KeyValuePair<string, string> item in newConfig)
            {
                this.rawConfig = this.rawConfig + "<CfgC>" + item.Key + "<CfgB><CfgD>" + item.Value + "<CfgA>";
            }

            //Write in file
            try
            {
                string path = string.Concat(new object[] {
                directory.FullName,
                "config.txt"
                });
                string enc = Crypto.EncryptString(this.rawConfig, string.Concat("OptionalConfig " + mod.ModID));
                string key = Custom.Md5Sum(enc);

                File.WriteAllText(path, key + enc, Encoding.UTF8);

                return true;
            }
            catch (Exception ex) { Debug.LogError(new SaveDataException(ex.ToString())); }

            return false;

        }

        /// <summary>
        /// Dictionary that contains configuable objects.
        /// I suggest using 'config' instead.
        /// </summary>
        public Dictionary<string, UIconfig> objectDictionary;




        private static string[] _data;

        /// <summary>
        /// Use progData instead
        /// </summary>
        public string data { get { throw new NotImplementedException("OptionInterface.data is no longer used! Use GetData and SetData instead."); } }
        public static string progData
        {
            get { return _data[slugcat]; }
            set
            {
                if (_data[slugcat] != value) { instance.DataOnChange(); _data[slugcat] = value; }
            }
        }


        /// <summary>
        /// Event when saved data is changed
        /// This is called when 1. LoadData, 2. Your mod changes data.
        /// </summary>
        public virtual void DataOnChange()
        {

        }

        /// <summary>
        /// Event that happens when Config is loaded from file/changed by config menu.
        /// Put your configuable var in here.
        /// </summary>
        public virtual void ConfigOnChange()
        {
            if (init)
            {
                foreach (OpTab tab in Tabs)
                {
                    foreach (UIelement item in tab.items)
                    {
                        if (item is UIconfig && (item as UIconfig).cosmetic)
                        {
                            item.Reset();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event that happens when selected SaveSlot has been changed.
        /// This automatically saves and loads data by default.
        /// </summary>
        public virtual void SlotOnChange()
        {
            SaveData();
            _slot = slot; _slugcat = slugcat;
            LoadData();
        }

        /// <summary>
        /// If this is true, data is automatically Saved/Loaded like vanilla game
        /// </summary>
        public bool progressData = false;
        public bool saveAsDeath
        {
            get { if (!progressData) { throw new Exception(); } return _saveAsDeath; }
            set { _saveAsDeath = value; }
        }
        public bool saveAsQuit
        {
            get { if (!progressData) { throw new Exception(); } return _saveAsQuit; }
            set { _saveAsQuit = value; }
        }
        private bool _saveAsDeath = false, _saveAsQuit = false;



        /// <summary>
        /// Load your raw data from CompletelyOptional Mod.
        /// Call this by your own.
        /// Check dataTinkered boolean to see if saved data is tinkered or not.
        /// </summary>
        /// <returns>Loaded Data</returns>
        public virtual void LoadData()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = defaultData;
            }
            try
            {
                string data = string.Empty;
                foreach (FileInfo file in directory.GetFiles())
                {
                    if (file.Name.Substring(file.Name.Length - 4) != ".txt") { continue; }

                    if (file.Name.Substring(0, 4) == "data")
                    {
                        if (slot.ToString() != file.Name.Substring(file.Name.Length - 5, 1)) { continue; }
                    }
                    else { continue; }

                    //LoadSlotData:
                    data = File.ReadAllText(file.FullName, Encoding.UTF8);
                    string key = data.Substring(0, 32);
                    data = data.Substring(32, data.Length - 32);
                    if (Custom.Md5Sum(data) != key)
                    {
                        Debug.Log(string.Concat(mod.ModID, " data file has been tinkered!"));
                        dataTinkered = true;
                    }
                    else
                    {
                        dataTinkered = false;
                    }
                    data = Crypto.DecryptString(data, string.Concat("OptionalData " + mod.ModID));
                }
                string[] raw = Regex.Split(data, "<slugChar>");
                _data = new string[Math.Max(_data.Length, raw.Length)];
                for (int j = 0; j < raw.Length; j++)
                {
                    _data[j] = raw[j];
                }
                return;
            }
            catch (Exception ex) { Debug.LogError(new LoadDataException(ex.ToString())); }


        }

        /// <summary>
        /// If you want to see whether your data is tinkered or not.
        /// </summary>
        public bool dataTinkered { get; private set; } = false;

        /// <summary>
        /// Save your raw data in file. bool is whether it succeed or not
        /// Call this by your own.
        /// </summary>
        public virtual bool SaveData()
        {
            string data = string.Empty;
            for (int i = 0; i < _data.Length; i++) { data += _data[i] + "<slugChar>"; };
            //if (string.IsNullOrEmpty(_data)) { return false; }
            try
            {
                string path = string.Concat(new object[] {
                directory.FullName,
                "data_",
                slot.ToString(),
                ".txt"
                });
                string enc = Crypto.EncryptString(data, string.Concat("OptionalData " + mod.ModID));
                string key = Custom.Md5Sum(enc);

                File.WriteAllText(path, key + enc);


                return true;
            }
            catch (Exception ex) { Debug.LogError(new SaveDataException(ex.ToString())); }

            return false;
        }

        /// <summary>
        /// If true, Initialize is in Mod Config; if false, this is game initialization and
        /// do not edit graphical details of UIelements when init is false
        /// </summary>
        public static bool init => OptionScript.init;

        /// <summary>
        /// Write your UI overlay here.
        /// </summary>
        public virtual void Initialize()
        { //Also Reset Config (Initialize w/o LoadConfig), and call ConfigOnChange().
            if (this.Configuable())
            {
                if (!directory.Exists) { directory.Create(); }
            }
            this.Tabs = null;
        }

        /// <summary>
        /// Event that's called every frame. Do not call this by your own.
        /// </summary>
        /// <param name="dt">deltaTime</param>
        public virtual void Update(float dt)
        {
            
        }

        /// <summary>
        /// Do not call this by your own.
        /// </summary>
        public void BackgroundUpdate(int saveOrLoad)
        {
            switch (saveOrLoad)
            {
                case 1: SaveData(); break;
                case 2: LoadData(); break;
            }
            if (_slot != slot) { SlotOnChange(); }
            else if (_slugcat != slugcat) { SlotOnChange(); }
        }

        /// <summary>
        /// Do not call this by your own.
        /// </summary>
        /// <param name="slugcatLength"></param>
        public void GenerateDataArray(int slugcatLength)
        {
            _data = new string[slugcatLength];
            LoadData();
        }

    }
}
