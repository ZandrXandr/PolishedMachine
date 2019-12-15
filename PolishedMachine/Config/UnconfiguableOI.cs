using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OptionalUI;
using Partiality.Modloader;
using UnityEngine;

namespace CompletelyOptional
{
    /// <summary>
    /// Default OI that's called when your mod does not support CompletelyOptional.
    /// Also shows the error in your OI.
    /// </summary>
    public class UnconfiguableOI : OptionInterface
    {
        public UnconfiguableOI(PartialityMod mod, Reason type) : base(mod)
        {
            this.reason = type;
        }

        public UnconfiguableOI(PartialityMod mod, Exception exception) : base(mod)
        {
            Tabs = new OpTab[1];
            Tabs[0] = new OpTab();
            this.reason = Reason.InitError;
            this.exception = exception.ToString();
        }

        public Reason reason;
        public string exception;

        


        public enum Reason
        {
            NoInterface,
            InitError,
            NoMod
        }

        public override bool Configuable()
        {
            return false;
        }

        public override bool SaveData()
        {
            return false;
        }
        public override void LoadData()
        {
            return;
        }

#pragma warning disable CA1822 // Mark members as static
        public new bool LoadConfig() => true;
#pragma warning disable IDE0060
        public new bool SaveConfig(Dictionary<string, string> newConfig) => true;
#pragma warning restore IDE0060
#pragma warning restore CA1822



        public OpRect testRect;
        public override void Initialize()
        {
            Tabs = new OpTab[1];
            Tabs[0] = new OpTab();

            if (this.reason == Reason.NoMod)
            {
                TutoInit();
                return;
            }

            //Futile.atlasManager.LogAllElementNames();

            labelID = new OpLabel(new Vector2(100f, 500f), new Vector2(400f, 50f), mod.ModID, FLabelAlignment.Center, true);
            Tabs[0].AddItem(labelID);
            labelVersion = new OpLabel(new Vector2(100f, 450f), new Vector2(100f, 20f), string.Concat("Version: ", mod.Version), FLabelAlignment.Left);
            Tabs[0].AddItem(labelVersion);
            if (mod.author != "NULL")
            {
                labelAuthor = new OpLabel(new Vector2(350f, 450f), new Vector2(200f, 20f), string.Concat("Author: ", mod.author), FLabelAlignment.Right);
                Tabs[0].AddItem(labelAuthor);
                labelAuthor.autoWrap = true;
            }
            /*
            if (mod.coauthor != "NULL")
            {
                labelCoauthor = new OpLabel(new Vector2(100f, 420f), new Vector2(300f, 20f), string.Concat("Coautor: ", mod.coauthor));
                Tabs[0].AddItem(labelCoauthor);
                labelCoauthor.autoWrap = true;
            }
            if(mod.description != "NULL")
            {
                labelDesc = new OpLabel(new Vector2(80f, 350f), new Vector2(340f, 20f), mod.description, FLabelAlignment.Left);
                Tabs[0].AddItem(labelDesc);
                labelDesc.autoWrap = true;
            }*/


            switch (this.reason)
            {
                case Reason.NoInterface:
                    labelSluggo0 = new OpLabel(new Vector2(100f, 200f), new Vector2(400f, 20f), "This Partiality Mod/Patch cannot be configured.");
                    Tabs[0].AddItem(labelSluggo0);

                    break;
                case Reason.InitError:
                    blue = new OpRect(new Vector2(40f, 40f), new Vector2(520f, 340f)) { alpha = 0.7f };
                    if (OptionScript.init)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            blue.rect.sprites[j].color = new Color(0.121568627f, 0.40392156862f, 0.69411764705f, 1f);
                        }
                    }
                    Color white = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);
                    oof = new OpLabel(new Vector2(100f, 320f), new Vector2(30f, 40f), ":(", FLabelAlignment.Left, true) { color = white };
                    labelSluggo0 = new OpLabel(new Vector2(100f, 320f), new Vector2(400f, 20f), "There was an issue initializing OptionInterface.") { color = white };
                    labelSluggo1 = new OpLabel(new Vector2(100f, 80f), new Vector2(400f, 240f), exception, FLabelAlignment.Left)
                    {
                        autoWrap = true,
                        color = white
                    };
                    labelSluggo1.OnChange();

                    Tabs[0].AddItem(blue);
                    Tabs[0].AddItem(oof);
                    Tabs[0].AddItem(labelSluggo0);
                    Tabs[0].AddItem(labelSluggo1);

                    break;
            }
        }

        public void TutoInit()
        {
            labelID = new OpLabel(new Vector2(100f, 500f), new Vector2(400f, 50f), "No Mod can be configured", FLabelAlignment.Center, true);
            Tabs[0].AddItem(labelID);
            labelVersion = new OpLabel(new Vector2(100f, 440f), new Vector2(200f, 20f), string.Concat("Config Machine (CompletelyOptional) by topicular"), FLabelAlignment.Left);
            Tabs[0].AddItem(labelVersion);
            labelAuthor = new OpLabel(new Vector2(300f, 410f), new Vector2(200f, 20f), string.Concat("also shoutout to RW Discord for helping me out"), FLabelAlignment.Right);
            Tabs[0].AddItem(labelAuthor);

            labelSluggo0 = new OpLabel(new Vector2(100f, 300f), new Vector2(400f, 20f), "checkout pinned tutorial in modding-support");
            labelSluggo1 = new OpLabel(new Vector2(100f, 260f), new Vector2(400f, 20f), "and create your own config screen!");
            Tabs[0].AddItem(labelSluggo0);
            Tabs[0].AddItem(labelSluggo1);
        }


        public OpLabel labelID;
        public OpLabel labelVersion;
        public OpLabel labelAuthor;
        public OpLabel labelCoauthor;
        public OpLabel labelDesc;
        public OpLabel labelSluggo0;
        public OpLabel labelSluggo1;
        public OpRect blue;
        public OpLabel oof;

        public override void Update(float dt)
        {
            base.Update(dt);
        }

    }
}
