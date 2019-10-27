using System;
using System.Collections.Generic;
using Music;
using RWCustom;
using UnityEngine;
using Menu;
using Partiality.Modloader;
using System.IO;
using OptionalUI;
using System.Reflection;
using PolishedMachine.Config;

namespace CompletelyOptional
{
    /// <summary>
    /// Menu Screen for Mod Config
    /// </summary>
    public class ConfigMenu : Menu.Menu, SelectOneButton.SelectOneButtonOwner, CheckBox.IOwnCheckBox
    {
        public OptionsMenu vanillaMenu;

        public ConfigMenu(ProcessManager manager) : base(manager, ProcessManager.ProcessID.OptionsMenu)
        {
            Debug.Log("ConfigMenu ctor!");
            this.manager.currentMainLoop = this; //duplicate
            this.manager.upcomingProcess = null;

            this.pages.Add(new Page(this, null, "hub", 0));
            OptionScript.configMenu = this;
            OptionScript.tabs = new Dictionary<string, OpTab>();

            
            if (this.manager.musicPlayer == null)
            {
                this.manager.musicPlayer = new MusicPlayer(this.manager);
                this.manager.sideProcesses.Add(this.manager.musicPlayer);
            }
            this.manager.musicPlayer.MenuRequestsSong(randomSong, 1f, 2f);
            //this.manager.musicPlayer.song = new MenuOrSlideShowSong(this.manager.musicPlayer, randomSong, 1f, 2f);
            //this.manager.musicPlayer.song.playWhenReady = true;


            redUnlocked = (this.manager.rainWorld.progression.miscProgressionData.redUnlocked ||
                File.Exists(string.Concat(new object[] {
                    Custom.RootFolderDirectory(), "unlockred.txt"
                })) ||
                this.manager.rainWorld.progression.miscProgressionData.redMeatEatTutorial > 2
                );

            opened = false;
            selectedModIndex = 0;
            menuTab = new MenuTab();

            List<string> allLevels = new List<string>();
            this.multiplayerUnlocks = new MultiplayerUnlocks(manager.rainWorld.progression, allLevels);
            currentInterface = null;

            OptionScript.soundFill = 0;
            freezeMenu = false;
            BoundKey = new Dictionary<string, string>();
            //Get Vanilla Keys
            for(int i = 0; i < OptionScript.rw.options.controls.Length; i++)
            {
                Options.ControlSetup setup = OptionScript.rw.options.controls[i];
                if (setup.preset == Options.ControlSetup.Preset.KeyboardSinglePlayer)
                {
                    for (int p = 0; p < setup.keyboardKeys.Length; p++)
                    {
                        if (!BoundKey.ContainsValue(setup.keyboardKeys[p].ToString()))
                        { BoundKey.Add(string.Concat("Vanilla_", i.ToString(), "_", p.ToString()), setup.keyboardKeys[p].ToString()); }

                    }
                }
                else
                {
                    for (int p = 0; p < setup.gamePadButtons.Length; p++)
                    {
                        string key = setup.gamePadButtons[p].ToString();
                        if (key.Length > 9 && int.TryParse(key.Substring(8, 1), out int _))
                        { }
                        else
                        {
                            key = key.Substring(0, 8) + i.ToString() + key.Substring(8);
                        }
                        if (!BoundKey.ContainsValue(key))
                        { BoundKey.Add(string.Concat("Vanilla_", i.ToString(), "_", p.ToString()), key); }

                    }

                }

            }
             

        }

        /// <summary>
        /// List of Binded Key.
        /// Key: ...it's key. Value: ...this is also Key... of OpKeyBinder.
        /// </summary>
        public static Dictionary<string, string> BoundKey;


        /// <summary>
        /// Whether to freeze menu or not
        /// </summary>
        public static bool freezeMenu;

        public static OptionScript script;
        public static bool gamepad;


        private static readonly int[] bgList =
        {
            33, 34, 35, 42
        };
        private static readonly int[] bgListRed =
        {
            32, 33, 34, 35, 36, 38, 39, 40, 42,
            6, 9, 10, 18, 7
        };
        public static bool redUnlocked;

        public void Initialize() //UI
        {

            if (vanillaMenu != null)
            {
                vanillaMenu.ShutDownProcess();
                vanillaMenu = null;
                
            }

            



            if (!redUnlocked)
            {
                this.scene = new InteractiveMenuScene(this, this.pages[0], (MenuScene.SceneID)(bgList[Mathf.FloorToInt(UnityEngine.Random.value * (bgList.Length))]));
            }
            else
            {
                this.scene = new InteractiveMenuScene(this, this.pages[0], (MenuScene.SceneID)(bgListRed[Mathf.FloorToInt(UnityEngine.Random.value * (bgListRed.Length))]));
            }
            Debug.Log(string.Concat("Chosen Background : " + this.scene.sceneID.ToString()));
            this.pages[0].subObjects.Add(this.scene);

            this.fadeSprite.RemoveFromContainer();
            this.pages[0].Container.AddChild(this.fadeSprite); //reset fadeSprite




            this.darkSprite = new FSprite("pixel", true)
            {
                color = new Color(0f, 0f, 0f),
                anchorX = 0f,
                anchorY = 0f,
                scaleX = 1368f,
                scaleY = 770f,
                x = -1f,
                y = -1f,
                alpha = 0.6f
            };
            this.pages[0].Container.AddChild(this.darkSprite);


            modListBound = new OpRect(new Vector2(205f, 225f) - UIelement.offset, new Vector2(280f, 510f), 0.3f);
            //modCanvasBound = new OpRect(new Vector2(553f, 105f), new Vector2(630f, 630f), 0.4f);
            modCanvasBound = new OpRect(new Vector2(533f, 105f) - UIelement.offset, new Vector2(630f, 630f), 0.4f);
            //Base: new Vector2(468f, 120f);
            menuTab.AddItem(modListBound);
            menuTab.AddItem(modCanvasBound);
            this.pages[0].subObjects.Add(modListBound.rect);
            this.pages[0].subObjects.Add(modCanvasBound.rect);
            
            

            this.backButton = new SimpleButton(this, this.pages[0], "BACK", "CANCEL", new Vector2(450f, 50f), new Vector2(110f, 30f));
            this.pages[0].subObjects.Add(this.backButton);
            this.saveButton = new SimpleButton(this, this.pages[0], "APPLY", "APPLY", new Vector2(600f, 50f), new Vector2(110f, 30f));
            this.pages[0].subObjects.Add(this.saveButton);
            base.MutualHorizontalButtonBind(saveButton, backButton);
            this.resetButton = new HoldButton(this, this.pages[0], "RESET CONFIG", "RESET CONFIG", new Vector2(300f, 90f), 30f);
            this.pages[0].subObjects.Add(this.resetButton);

            //Dark Box for ModList & Canvas
            //modlist x200 y400 w240 h600
            //canvas  x568 y120 w600 h600

            this.resetButton.nextSelectable[2] = this.backButton;
            this.backButton.nextSelectable[0] = this.resetButton;
            this.backButton.nextSelectable[2] = this.saveButton;
            this.saveButton.nextSelectable[0] = this.backButton;

            
            
            modButtons = new SelectOneButton[OptionScript.loadedModsDictionary.Count]; //<==MenuLabel
            int i = 0; selectedModIndex = 0;
            Dictionary<int, string> dictionary = new Dictionary<int, string>(modButtons.Length);
            Dictionary<int, bool> dictionary0 = new Dictionary<int, bool>(modButtons.Length);
            foreach (string id in OptionScript.loadedModsDictionary.Keys)
            {
                modButtons[i] = new SelectOneButton(this, this.pages[0], id, "ModSelect", new Vector2(220f, 700f - 30f * i), new Vector2(250f, 24f), modButtons, i);
                
                this.pages[0].subObjects.Add(modButtons[i]);

                dictionary.Add(i, id);
                Debug.Log(string.Concat("Mod(" + i + ") : " + id));
                OptionInterface itf = OptionScript.loadedInterfaceDict[id];
                try
                {
                    itf.Initialize();
                }
                catch (Exception ex)
                {
                    itf = new UnconfiguableOI(itf.mod, new Exception(string.Concat("OIinitializeError: ", id, " had a problem in Initialize().",
                           Environment.NewLine, ex
                           )));
                    OptionScript.loadedInterfaceDict.Remove(id);
                    OptionScript.loadedInterfaceDict.Add(id, itf);
                    itf.Initialize();
                }


                if (itf.Tabs == null || itf.Tabs.Length < 1)
                {
                    itf = new UnconfiguableOI(itf.mod, new Exception(string.Concat("TabIsNull: ", id, " OI has No OpTabs! ",
                           Environment.NewLine, "Did you put base.Initialize() after your code?",
                           Environment.NewLine, "Leaving OI.Initialize() completely blank will prevent the mod from using LoadData/SaveData."
                           )));
                    OptionScript.loadedInterfaceDict.Remove(id);
                    OptionScript.loadedInterfaceDict.Add(id, itf);
                    itf.Initialize();
                }
                else if (itf.Tabs.Length > 20)
                {
                    itf = new UnconfiguableOI(itf.mod, new Exception("Too Many Tabs! D: Maximum tab number is 20.\nAlso what are you going to do with all those settings?"));
                    OptionScript.loadedInterfaceDict.Remove(id);
                    OptionScript.loadedInterfaceDict.Add(id, itf);
                    itf.Initialize();
                }

                try
                {
                    itf.LoadConfig();
                    itf.ShowConfig();
                }
                catch (Exception ex)
                {
                    itf = new UnconfiguableOI(itf.mod, new Exception(string.Concat("OILoad/ShowConfigError: ", id, " had a problem in Load/ShowConfig()\nAre you editing LoadConfig()/ShowConfig()? That could cause serious error.",
                           Environment.NewLine, ex
                           )));
                    OptionScript.loadedInterfaceDict.Remove(id);
                    OptionScript.loadedInterfaceDict.Add(id, itf);
                    itf.Initialize();

                    itf.LoadConfig();
                    itf.ShowConfig();
                }

                dictionary0.Add(i, itf.Configuable());


                for (int t = 0; t < itf.Tabs.Length; t++)
                {
                    OptionScript.tabs.Add(string.Concat(i.ToString("D3") + "_" + t.ToString("D2")), itf.Tabs[t]);
                    foreach (UIelement element in itf.Tabs[t].items)
                    {
                        foreach (MenuObject obj in element.subObjects)
                        {
                            this.pages[0].subObjects.Add(obj);
                        }
                        this.pages[0].Container.AddChild(element.myContainer);
                    }
                    itf.Tabs[t].Hide();
                }
                
                i++;
            }
            modList = dictionary;
            modConfiguability = dictionary0;


            this.resetButton.nextSelectable[1] = this.modButtons[this.modButtons.Length - 1];
            this.backButton.nextSelectable[1] = this.modButtons[this.modButtons.Length - 1];
            this.saveButton.nextSelectable[1] = this.modButtons[this.modButtons.Length - 1];
            this.modButtons[this.modButtons.Length - 1].nextSelectable[3] = this.saveButton;
            if (this.modButtons.Length > 1)
            {
                for (int m = 0; m < this.modButtons.Length - 1; m++)
                {
                    this.modButtons[m].nextSelectable[3] = this.modButtons[m + 1];
                    this.modButtons[m + 1].nextSelectable[1] = this.modButtons[m];
                }
            }




            //Load Tab
            selectedTabIndex = 0;
            currentInterface = OptionScript.loadedInterfaceDict[modList[0]];
            currentTab = OptionScript.tabs[string.Concat(selectedModIndex.ToString("D3") + "_" + selectedTabIndex.ToString("D2"))];

            currentTab.Show();
            if (currentInterface.Configuable())
            {
                saveButton.buttonBehav.greyedOut = false;
                resetButton.buttonBehav.greyedOut = false;
            }
            else
            {
                saveButton.buttonBehav.greyedOut = true;
                resetButton.buttonBehav.greyedOut = true;
            }


            tabCtrler = new ConfigTabController(new Vector2(493f, 120f) - UIelement.offset, new Vector2(40f, 600f), menuTab, this);
            menuTab.AddItem(tabCtrler);
            foreach (MenuObject obj in tabCtrler.subObjects)
            {
                this.pages[0].subObjects.Add(obj);
            }

            this.selectedObject = this.backButton;


            OptionScript.configChanged = false;
        }
        public Dictionary<int, string> modList;
        public Dictionary<int, bool> modConfiguability;
        public static int selectedModIndex;
        public static OptionInterface currentInterface;
        public static OpTab currentTab;
        public static int selectedTabIndex;
        public static FContainer tabContainer;
        public static MenuTab menuTab;
        public static ConfigTabController tabCtrler;
        public static string description;

        public SelectOneButton[] modButtons;

        
        public SimpleButton backButton;
        public SimpleButton saveButton;
        public HoldButton resetButton;

        public OpRect modListBound;
        public OpRect modCanvasBound;


        public string randomSong
        {
            get
            {
                return ConfigManager.randomSong;
            }
        }

        public FSprite fadeSprite;
        public FSprite darkSprite;


        public static void ChangeSelectedTab()
        {
            currentTab.Hide();

            currentTab = OptionScript.tabs[string.Concat(selectedModIndex.ToString("D3") + "_" + selectedTabIndex.ToString("D2"))];

            currentTab.Show();
        }

        public void ChangeSelectedMod()
        {
            //Unload Current Ones

            currentInterface = OptionScript.loadedInterfaceDict[modList[selectedModIndex]];
            selectedTabIndex = 0;
            tabCtrler.OnChange();
            ChangeSelectedTab();
            if (currentInterface.Configuable())
            {
                saveButton.buttonBehav.greyedOut = false;
                resetButton.buttonBehav.greyedOut = false;
            }
            else
            {
                saveButton.buttonBehav.greyedOut = true;
                resetButton.buttonBehav.greyedOut = true;
            }
        }

        public static void SaveAllConfig()
        {
            foreach(KeyValuePair<string,OptionInterface> item in OptionScript.loadedInterfaceDict)
            {
                if (!OptionScript.loadedInterfaceDict[item.Key].Configuable()) { continue; }
                Dictionary<string, string> newConfig = OptionScript.loadedInterfaceDict[item.Key].GrabConfig();
                OptionScript.loadedInterfaceDict[item.Key].SaveConfig(newConfig);
            }
        }
        public static void SaveCurrentConfig()
        {
            if (!currentInterface.Configuable()) { return; }
            Dictionary<string, string> newConfig = currentInterface.GrabConfig();
            currentInterface.SaveConfig(newConfig);
        }
        public void ResetCurrentConfig()
        {
            if(!currentInterface.Configuable()) { return; }

            foreach (OpTab tab in currentInterface.Tabs)
            {
                foreach (UIelement element in tab.items)
                {
                    foreach (MenuObject obj in element.subObjects)
                    {
                        obj.RemoveSprites();
                        this.pages[0].subObjects.Remove(obj);
                    }
                    this.pages[0].Container.RemoveChild(element.myContainer);
                    element.myContainer.RemoveFromContainer();
                }
                tab.Unload();
            }
            int i = 0;
            do
            {
                string key = string.Concat(selectedModIndex.ToString("D3") + "_" + i.ToString("D2"));
                if (OptionScript.tabs.ContainsKey(key)) { OptionScript.tabs.Remove(key); }
                else
                {
                    break;
                }
                i++;
            } while (i < 100);

            currentInterface.Initialize();
            for(i = 0; i < currentInterface.Tabs.Length; i++)
            {
                string key = string.Concat(selectedModIndex.ToString("D3") + "_" + i.ToString("D2"));
                OptionScript.tabs.Add(key, currentInterface.Tabs[i]);
                foreach (UIelement element in currentInterface.Tabs[i].items)
                {
                    foreach (RectangularMenuObject obj in element.subObjects)
                    {
                        this.pages[0].subObjects.Add(obj);
                    }
                    this.pages[0].Container.AddChild(element.myContainer);
                }
                currentInterface.Tabs[i].Hide();
            }

            selectedTabIndex = 0;
            currentTab = currentInterface.Tabs[0];
            currentTab.Show();
            currentInterface.SaveConfig(currentInterface.GrabConfig());
            currentInterface.ConfigOnChange();

            //OptionScript.configChanged = false;
        }



        private string lastDescription;
        public override void Update()
        {
            if (!string.IsNullOrEmpty(description) && string.IsNullOrEmpty(this.UpdateInfoText()))
            {
                this.infoLabelFade = 1f;
                this.infoLabel.text = description;
                if (lastDescription != description)
                {
                    this.infolabelDirty = false;
                    this.infoLabelSin = 0f;
                }
            }
            lastDescription = description;

            base.Update(); //keep buttons to be sane


            
            if (fadein && this.scene != null && (int)this.scene.sceneID < 12 && (int)this.scene.sceneID > 6)
            { //clamp offset
                this.scene.camPos = new Vector2(this.scene.camPos.x * 0.7f, this.scene.camPos.y * 0.7f);
            }

            

            if (this.fadeSprite != null)
            {
                if (!fadein)
                {
                    this.fadeoutFrame++;
                    if (this.fadeoutFrame > 40)
                    {
                        //if not loaded yet ==> this.fadeoutFrame = 30; return;
                        this.fadeoutFrame = 20;
                        this.fadein = true;
                        if(currentInterface == null)
                        {
                            this.Initialize();
                        }
                    }
                }
                else
                {
                    this.fadeoutFrame--;
                    if (this.fadeoutFrame < 1)
                    {
                        this.fadeoutFrame = 0;
                        this.fadeSprite.RemoveFromContainer();
                        this.fadeSprite = null;
                        return;
                    }
                }
                this.OpenMenu();
                return;
            }
            else
            {
                if (OptionScript.configChanged && this.backButton != null)
                { this.backButton.menuLabel.text = "CANCEL"; }
                else
                { this.backButton.menuLabel.text = "BACK"; }
            }



            if (this.manager.musicPlayer != null && this.manager.musicPlayer.song?.FadingOut == true)
            {
                this.manager.musicPlayer.MenuRequestsSong(randomSong, 1f, 2f);
            }


        }


        public Dictionary<string, int> buttonList;
        public override void Singal(MenuObject sender, string message)
        {
            if (message != null)
            {
                if (buttonList == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(4)
                    {
                        { "CANCEL", 0 },
                        { "APPLY", 1 },
                        { "RESET CONFIG", 2 },
                        { "SOUNDTEST", 3 }
                    };
                    buttonList = dictionary;
                }

                if (buttonList.TryGetValue(message, out int num))
                {
                    switch (num)
                    {
                        case 0:
                            OptionsMenuPatch.mod = false;
                            opened = false;
                            base.PlaySound(SoundID.MENU_Switch_Page_Out);
                            this.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.OptionsMenu);
                            break;
                        case 1:
                            if (OptionScript.configChanged)
                            {
                                base.PlaySound(SoundID.MENU_Switch_Page_In);
                                SaveCurrentConfig();
                                OptionScript.configChanged = false;
                                this.saveButton.menuLabel.text = "SAVE ALL";
                            }
                            else
                            {
                                if (this.saveButton.menuLabel.text == "SAVE ALL")
                                {
                                    base.PlaySound(SoundID.MENU_Next_Slugcat);
                                    SaveAllConfig();
                                    //EXIT
                                    OptionsMenuPatch.mod = false;
                                    opened = false;
                                    this.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.OptionsMenu);
                                }
                                else
                                {
                                    base.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                                }
                            }
                            break;
                        case 2:
                            ResetCurrentConfig();
                            base.PlaySound(SoundID.MENU_Switch_Page_In);
                            opened = false;
                            OpenMenu();
                            break;
                        case 3:
                            if (!redUnlocked) { base.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed); }
                            else
                            {
                                base.PlaySound(SoundID.MENU_Switch_Page_In);

                                Debug.Log("Enter SoundTest");

                                this.InitializeSitting();
                                this.manager.rainWorld.progression.ClearOutSaveStateFromMemory();
                                if (this.manager.arenaSitting.ReadyToStart)
                                {
                                    this.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);//, 1.2f);
                                }
                                else
                                {
                                    this.manager.arenaSitting = null;
                                }



                            }
                            break;
                    }
                }
            }
        }

        public ArenaSetup GetArenaSetup
        {
            get
            {
                return this.manager.arenaSetup;
            }
        }

        public ArenaSetup.GameTypeSetup GetGameTypeSetup
        {
            get
            {
                return this.GetArenaSetup.GetOrInitiateGameTypeSetup(ArenaSetup.GameTypeID.Sandbox);
            }
        }

        private readonly MultiplayerUnlocks multiplayerUnlocks;

        private void InitializeSitting()
        {
            if(this.manager.arenaSetup == null)
            {
                ArenaSetup setup = new ArenaSetup
                {
                    currentGameType = ArenaSetup.GameTypeID.Sandbox
                };

                setup.gametypeSetups.Add(new ArenaSetup.GameTypeSetup());
                setup.gametypeSetups[setup.gametypeSetups.Count - 1].InitAsGameType(ArenaSetup.GameTypeID.Sandbox);
                
                this.manager.arenaSetup = setup;
            }

            this.manager.arenaSitting = new ArenaSitting(this.GetGameTypeSetup, this.multiplayerUnlocks)
            {
                levelPlaylist = new List<string>()
            };
            this.manager.arenaSitting.levelPlaylist.Add("SoundTest");

        }

        public void KillTabElements()
        {
            foreach(KeyValuePair<string, OpTab> item in OptionScript.tabs)
            {
                OpTab tab = OptionScript.tabs[item.Key];
                
                foreach (UIelement element in tab.items)
                {
                    foreach (MenuObject obj in element.subObjects)
                    {
                        //obj.RemoveSprites();
                        //this.pages[0].subObjects.Remove(obj);
                    }
                    this.pages[0].Container.RemoveChild(element.myContainer);
                    element.myContainer.RemoveFromContainer();
                }
                tab.Unload();
            }

            //this.pages[0].subObjects.Remove(modListBound.menuObj);
            //this.pages[0].subObjects.Remove(modCanvasBound.menuObj);
            menuTab.Unload();



        }

        public override void ShutDownProcess()
        {
            //foreach (UIelement element in menuTab.items)
            //{
                //foreach (MenuObject obj in element.subObjects)
                //{
                    //this.pages[0].subObjects.Remove(obj);
                //}
                //this.pages[0].Container.RemoveChild(element.myContainer);
                //element.myContainer.RemoveFromContainer();
            //}
            KillTabElements();
            OptionScript.KillTabs();
            base.ShutDownProcess();
            this.darkSprite.RemoveFromContainer();
            currentTab = null;
        }

        public override float ValueOfSlider(Slider slider)
        {
            switch (slider.ID)
            {
                case Slider.SliderID.SfxVol:
                    return this.manager.rainWorld.options.soundEffectsVolume;
                case Slider.SliderID.MusicVol:
                    return this.manager.rainWorld.options.musicVolume;
                case Slider.SliderID.ArenaMusicVolume:
                    return this.manager.rainWorld.options.arenaMusicVolume * this.manager.rainWorld.options.musicVolume;
                default:
                    return 0f;
            }
        }


        public override void SliderSetValue(Slider slider, float f)
        {
            switch (slider.ID)
            {
                case Slider.SliderID.SfxVol:
                    this.manager.rainWorld.options.soundEffectsVolume = f;
                    break;
                case Slider.SliderID.MusicVol:
                    this.manager.rainWorld.options.musicVolume = f;
                    break;
                case Slider.SliderID.ArenaMusicVolume:
                    this.manager.rainWorld.options.arenaMusicVolume = Mathf.InverseLerp(0f, this.manager.rainWorld.options.musicVolume, f);
                    if (this.manager.musicPlayer != null && this.manager.musicPlayer.song != null && this.manager.musicPlayer.song.context == MusicPlayer.MusicContext.Arena)
                    {
                        this.manager.musicPlayer.song.baseVolume = 0.3f * this.manager.rainWorld.options.arenaMusicVolume;
                    }
                    break;
            }
            this.selectedObject = slider;
            this.infoLabel.text = this.UpdateInfoText();
            this.infoLabelSin = 0f;
            this.infoLabelFade = 1f;
            if (this.manager.rainWorld.options.musicVolume > 0f)
            {
                if (this.manager.musicPlayer == null)
                {
                    this.manager.musicPlayer = new MusicPlayer(this.manager);
                    this.manager.sideProcesses.Add(this.manager.musicPlayer);
                }
                if (this.manager.musicPlayer.song == null)
                {
                    this.manager.musicPlayer.MenuRequestsSong(randomSong, 1f, 0.7f);
                }
            }
        }

        public override string UpdateInfoText()
        {
            if (this.selectedObject is SelectOneButton)
            {
                if ((this.selectedObject as SelectOneButton).signalText == "ModSelect")
                {
                    if (modList.TryGetValue((this.selectedObject as SelectOneButton).buttonArrayIndex, out string id))
                    {
                        string output = "";
                        if (modConfiguability.TryGetValue((this.selectedObject as SelectOneButton).buttonArrayIndex, out bool able))
                        {
                            output = able ? "Configure " : "Display ";
                        }

                        output += id;

                        if (OptionScript.loadedModsDictionary.TryGetValue(id, out PartialityMod mod))
                        {
                            if (!string.IsNullOrEmpty(mod.author) && mod.author != "NULL")
                            {
                                output = output + " by " + mod.author;
                            }
                        }

                        return output;
                    }

                    return string.Empty;
                }
            }
            if (this.selectedObject is HoldButton)
            {
                if (modList.TryGetValue(selectedModIndex, out string id))
                {
                    string output = "Hold down to restore original config of " + id;

                    return output;
                }
                return "Hold down to restore original config for this mod";
            }
            if(this.selectedObject == this.backButton)
            {
                if (OptionScript.configChanged)
                {
                    return "Return to vanilla option menu (WITHOUT saving!)";
                }
                else
                {
                    return "Return to vanilla option menu";
                }
            }
            if (this.selectedObject == this.saveButton)
            {
                if (this.saveButton.menuLabel.text == "SAVE ALL")
                {
                    return "Save all changes to file and exit";
                }
                else
                {
                    if (modList.TryGetValue(selectedModIndex, out string id))
                    {
                        string output = "Save changed config of " + id;

                        return output;
                    }
                }
            }


            return base.UpdateInfoText();
        }


        bool CheckBox.IOwnCheckBox.GetChecked(CheckBox box)
        {
            throw new NotImplementedException();
        }

        void CheckBox.IOwnCheckBox.SetChecked(CheckBox box, bool c)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, int> radioDictionary;
        public int GetCurrentlySelectedOfSeries(string series)
        {
            if (series != null)
            {
                if(radioDictionary == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(2)
                    {
                        { "ModSelect", 0 },
                        { "NOTSaveSlot", 1 }
                    };
                    radioDictionary = dictionary;
                }
                if (radioDictionary.TryGetValue(series, out int num))
                {
                    switch (num)
                    {
                        case 0:
                            return selectedModIndex;
                        default:
                            return 0;
                    }
                }

            }
            return -1;
        }

        public void SetCurrentlySelectedOfSeries(string series, int to)
        {
            if (series != null)
            {
                if (radioDictionary == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(2)
                    {
                        { "ModSelect", 0 },
                        { "SaveSlot", 1 }
                    };
                    radioDictionary = dictionary;
                }
                if (radioDictionary.TryGetValue(series, out int num))
                {
                    switch (num)
                    {
                        case 0:
                            //Change Selected Mod
                            if (selectedModIndex != to)
                            {
                                selectedModIndex = to;
                                ChangeSelectedMod();
                            }


                            break;
                    }

                }

            }
        }


        public bool opened = false;
        private bool fadein = false;
        private int fadeoutFrame;
        public void OpenMenu()
        {
            if (!opened)
            { //init
                opened = true;
                fadeoutFrame = 9;
                fadein = false;
                if(this.fadeSprite != null)
                {
                    this.fadeSprite.RemoveFromContainer();
                }
                this.fadeSprite = new FSprite("Futile_White", true)
                {
                    color = new Color(0f, 0f, 0f),
                    x = this.manager.rainWorld.screenSize.x / 2f,
                    y = this.manager.rainWorld.screenSize.y / 2f,
                    alpha = 0f,
                    shader = this.manager.rainWorld.Shaders["EdgeFade"]
                };

                this.pages[0].Container.AddChild(this.fadeSprite);
                return;
            }
            float multiplier = Math.Min(1f, this.fadeoutFrame * 0.05f);
            this.fadeSprite.scaleX = (this.manager.rainWorld.screenSize.x * Mathf.Lerp(1.5f, 1f, multiplier) + 2f) / 16f;
            this.fadeSprite.scaleY = (this.manager.rainWorld.screenSize.y * Mathf.Lerp(2.5f, 1.5f, multiplier) + 2f) / 16f;
            this.fadeSprite.alpha = Mathf.InverseLerp(0f, 0.9f, multiplier);

        }
    }
}
