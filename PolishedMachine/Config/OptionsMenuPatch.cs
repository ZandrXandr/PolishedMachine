using System;
using System.Collections.Generic;
using Music;
using RWCustom;
using UnityEngine;
using Menu;
using PolishedMachine.Config;

namespace CompletelyOptional
{
    /// <summary>
    /// These code attach themselves to OptionsMenu.
    /// </summary>
    public static class OptionsMenuPatch
    {
        public static string UpdateInfoTextPatch(On.Menu.OptionsMenu.orig_UpdateInfoText orig, OptionsMenu menu)
        {
            //Code
            if (menu.selectedObject is SelectOneButton)
            {
                if ((menu.selectedObject as SelectOneButton).signalText == "ScreenRes")
                {
                    return menu.Translate("Change screen resolution");
                }
                if ((menu.selectedObject as SelectOneButton).signalText == "Language")
                {
                    return menu.Translate("Change language");
                }
                if ((menu.selectedObject as SelectOneButton).signalText == "SaveSlot")
                {
                    return menu.Translate("Select save slot") + " " + ((menu.selectedObject as SelectOneButton).buttonArrayIndex + 1).ToString();
                }
            }
            if (menu.selectedObject is Slider)
            {
                switch ((menu.selectedObject as Slider).ID)
                {
                    case Slider.SliderID.SfxVol:
                        return menu.Translate("Sound effects volume:") + " " + Custom.IntClamp((int)(menu.manager.rainWorld.options.soundEffectsVolume * 100f), 0, 100).ToString() + "%";
                    case Slider.SliderID.MusicVol:
                        return menu.Translate("Music volume:") + " " + Custom.IntClamp((int)(menu.manager.rainWorld.options.musicVolume * 100f), 0, 100).ToString() + "%";
                    case Slider.SliderID.ArenaMusicVolume:
                        return menu.Translate("Arena mode music volume:") + " " + Custom.IntClamp((int)(menu.manager.rainWorld.options.arenaMusicVolume * menu.manager.rainWorld.options.musicVolume * 100f), 0, 100).ToString() + "%";
                }
            }
            if (menu.selectedObject is HoldButton)
            {
                return menu.Translate("Hold down to wipe your save slot and start over");
            }
            if (menu.selectedObject is CustomMessageButton && (menu.selectedObject as CustomMessageButton).message == "Toggle Fullscreen")
            {
                return (!menu.manager.rainWorld.options.windowed) ? menu.Translate("Switch to windowed mode") : menu.Translate("Switch to fullscreen mode");
            }
            if (menu.selectedObject is ControlsButton)
            {
                return menu.Translate("Configure controls");
            }
            if (menu.selectedObject == menu.backButton)
            {
                return menu.Translate("Back to main menu");
            }
            if (menu.selectedObject == menu.creditsButton)
            {
                return menu.Translate("View credits");
            }
            if (menu.selectedObject == enterConfig)
            {
                return "Configure Settings for Partiality Mods";
            }
            return menu.UpdateInfoText();
        }


        public static void UpdatePatch(On.Menu.OptionsMenu.orig_Update orig, OptionsMenu menu)
        {

            if (mod)
            {
                //modmenu.Update();
                menu.manager.currentMainLoop = null;
                //menu.manager.soundLoader.ReleaseAllUnityAudio();
                menu.processActive = false;

                menu.manager.currentMainLoop = new ConfigMenu(menu.manager);
                modmenu = menu.manager.currentMainLoop as ConfigMenu;
                modmenu.vanillaMenu = menu;

                return;
            }
            else if(modmenu != null)
            {
                modmenu.ShutDownProcess();
                modmenu = null;
            }

            if (menu.manager.currentMainLoop != menu)
            {
                menu.ShutDownProcess();
                return;
            }
            if (!mod && enterConfig == null)
            { //ctor
                enterConfig = new SimpleButton(menu, menu.pages[0], "MOD CONFIG", "MOD CONFIG", new Vector2(340f, 50f), new Vector2(110f, 30f));
                menu.pages[0].subObjects.Add(enterConfig);
                //menu.manager.musicPlayer.MenuRequestsSong(CompletelyOptional.ConfigManager.randomSong, 2f, 2f);
                menu.backButton.nextSelectable[2] = enterConfig;
                enterConfig.nextSelectable[1] = menu.soundSlider;
                enterConfig.nextSelectable[0] = menu.backButton;
                enterConfig.nextSelectable[2] = menu.creditsButton;
                menu.creditsButton.nextSelectable[0] = enterConfig;

            }


            orig.Invoke(menu);

            if (resolutionDirty)
            {
                resolutionDirty = false;
                Screen.SetResolution((int)menu.manager.rainWorld.options.ScreenSize.x, (int)menu.manager.rainWorld.options.ScreenSize.y, false);
                Screen.fullScreen = false;
                Screen.showCursor = true;
                menu.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.OptionsMenu);
            }

        }

        public static void SingalPatch(On.Menu.OptionsMenu.orig_Singal orig, OptionsMenu menu, MenuObject sender, string message)
        {
            if (message != null)
            {
                if (tuch6A == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(6)
                    {
                        { "Toggle Fullscreen", 0 },
                        { "BACK", 1 },
                        { "CREDITS", 2 },
                        { "RESET PROGRESS", 3 },
                        { "INPUT", 4 },

                        { "MOD CONFIG", 5 }
                    };//5
                    tuch6A = dictionary;
                }
                if (tuch6A.TryGetValue(message, out int num))
                {
                    switch (num)
                    {
                        case 0:
                            menu.manager.rainWorld.options.windowed = !menu.manager.rainWorld.options.windowed;
                            resolutionDirty = true;
                            menu.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                            break;
                        case 1:
                            menu.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                            menu.PlaySound(SoundID.MENU_Switch_Page_Out);
                            menu.manager.rainWorld.options.Save();
                            break;
                        case 2:
                            menu.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Credits);
                            menu.PlaySound(SoundID.MENU_Switch_Page_In);
                            menu.manager.rainWorld.options.Save();
                            break;
                        case 3:
                            menu.manager.rainWorld.progression.WipeAll();
                            menu.PlaySound(SoundID.MENU_Switch_Page_In);
                            menu.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.OptionsMenu);
                            break;
                        case 4:
                            menu.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.InputOptions);
                            menu.PlaySound(SoundID.MENU_Switch_Page_In);
                            menu.manager.rainWorld.options.Save();
                            break;

                        case 5:
                            mod = true;
                            menu.PlaySound(SoundID.MENU_Switch_Page_In);
                            menu.manager.rainWorld.options.Save();
                            //this.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.OptionsMenu);
                            menu.manager.soundLoader.ReleaseAllUnityAudio();

                            modmenu = new ConfigMenu(menu.manager);
                            menu.manager.currentMainLoop = modmenu;
                            modmenu.vanillaMenu = menu;
                            modmenu.OpenMenu();
                            break;
                    }
                }
            }
        }


        public static void ShutDownProcessPatch(On.Menu.OptionsMenu.orig_ShutDownProcess orig, OptionsMenu menu)
        {
            orig.Invoke(menu);

            string songid = "";
            if (menu.manager.musicPlayer != null)
            {
                songid = menu.manager.musicPlayer.song?.name.Substring(0, 5);
            }
            
            if (!mod)
            { //going back to main menu
                if (menu.manager.musicPlayer != null && songid != "RW_8 " && songid != "Title")
                {
                    Debug.Log(string.Concat("Shutdown Option Music :" + menu.manager.musicPlayer.song?.name));
                    menu.manager.musicPlayer.nextSong = new MenuOrSlideShowSong(menu.manager.musicPlayer, "RW_8 - Sundown", 0.8f, 2f)
                    {
                        playWhenReady = false
                    };
                }
                
            }
            if (enterConfig != null)
            {
                enterConfig.RemoveSprites();
                enterConfig = null;
            }
        }



        public static ConfigMenu modmenu;
        public static SimpleButton enterConfig;
        public static bool mod = false;
        public static ConfigMenu config;
        public static Dictionary<string, int> tuch6A;
        public static bool resolutionDirty;



    }
}
