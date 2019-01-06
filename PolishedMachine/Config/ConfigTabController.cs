using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RWCustom;
using UnityEngine;
using OptionalUI;
using Menu;

namespace CompletelyOptional
{
    /// <summary>
    /// UI for switching tabs
    /// </summary>
    public class ConfigTabController : UIelement
    {
        public ConfigTabController(Vector2 pos, Vector2 size, MenuTab tab, ConfigMenu menu) : base(pos, size)
        {
            this.menuTab = tab;
            tab.tabCtrler = this;
            this.cfgMenu = menu;
            this.mode = TabMode.NULL;
            subElements = new List<UIelement>();

            OnChange();
        }

        public MenuTab menuTab;
        public ConfigMenu cfgMenu;

        public List<UIelement> subElements;


        public int index
        {
            get { return ConfigMenu.selectedTabIndex; }
            set {
                if (_index == value)
                {
                    menu.PlaySound(SoundID.MENY_Already_Selected_MultipleChoice_Clicked);
                    return;
                }
                menu.PlaySound(SoundID.MENU_MultipleChoice_Clicked);
                ConfigMenu.selectedTabIndex = value;
                cfgMenu.ChangeSelectedTab();
                OnChange();
            }
        }
        private int _index;

        public int tabCount
        {
            get { return ConfigMenu.currentInterface.Tabs.Length; }
        }
        private int _tabCount = -1;

        private enum TabMode
        {
            single, //==1
            tab, //>1, <=7
            button, //>7
            NULL
        }
        private TabMode mode;

        public override void Update(float dt)
        {
            base.Update(dt);
            foreach(UIelement element in this.subElements)
            {
                element.Update(dt);
            }

        }

        public override void OnChange()
        { //Selected Tab has changed
            base.OnChange();
            if(_tabCount != tabCount)
            {
                Unload();
                _tabCount = tabCount;
                if (tabCount == 1)
                {
                    mode = TabMode.single;
                }
                else if (tabCount <= 12)
                {
                    mode = TabMode.tab;
                }
                else if (tabCount <= 20)
                {
                    mode = TabMode.button;
                }
                else
                {
                    _index = 0;
                    return;
                    //throw new Exception("Too Many Tabs! Maximum tab number is 20.\nAlso what are you going to do with all those settings?");
                }
                Initialize();
                _index = 0;
            }
            else if(_index == index) { return; }

            if(_index != index)
            {
                _index = index;
            }

        }

        public void Initialize()
        {
            this.subElements = new List<UIelement>();
            switch (this.mode)
            {
                default:
                case TabMode.single:
                    //Nothing to draw
                    break;
                case TabMode.tab:
                    for(int i = 0; i < _tabCount; i++)
                    {
                        SelectTab tab = new SelectTab(i, this);
                        this.subElements.Add(tab);
                        menu.pages[0].subObjects.Add(tab.rect);
                    }

                    break;
                case TabMode.button:
                    for (int i = 0; i < _tabCount; i++)
                    {
                        SelectButton btn = new SelectButton(i, this);
                        this.subElements.Add(btn);
                        menu.pages[0].subObjects.Add(btn.rect);
                    }
                    break;
            }
            

        }

        public override void Unload()
        {
            base.Unload();
            switch (this.mode)
            {
                default:
                case TabMode.single:
                    //Nothing drawn
                    return;
                case TabMode.tab:
                    foreach(SelectTab tab in this.subElements)
                    {
                        menu.pages[0].subObjects.Remove(tab.rect);
                        tab.rect.RemoveSprites();
                    }
                    break;
                case TabMode.button:
                    foreach (SelectButton btn in this.subElements)
                    {
                        menu.pages[0].subObjects.Remove(btn.rect);
                        btn.rect.RemoveSprites();
                    }
                    break;
            }
            foreach(UIelement element in subElements)
            {
                element.Unload();
            }

        }


        public class SelectTab : UIelement
        {
            public SelectTab(int index, ConfigTabController ctrler) : base(new Vector2(), new Vector2())
            {
                this.index = index;
                this.ctrl = ctrler;
                float height = Mathf.Min(120f, 600f / ctrler._tabCount);

                this._pos = ctrl.pos + new Vector2(17f, height * (ctrler._tabCount - index - 1) + 3f);
                this.size = new Vector2(30f, height - 6f);
                this.rect = new DyeableRect(menu, owner, this.pos, this.size, true);
                this.rect.tab = true;

                this.subObjects.Add(this.rect);
                this.rect.fillAlpha = 0.3f;


            }
            private ConfigTabController ctrl;

            /// <summary>
            /// Index this Object is presenting
            /// </summary>
            public int index;

            //public MenuLabel label;
            /// <summary>
            /// Tab Boundary
            /// </summary>
            public DyeableRect rect;

            

            public override void GrafUpdate(float dt)
            {
                base.GrafUpdate(dt);

                this.flash = Custom.LerpAndTick(this.flash, 0f, 0.03f, 0.166666672f);
                float num = 0.5f - 0.5f * Mathf.Sin((this.sin) / 30f * 3.14159274f * 2f);
                num *= this.sizeBump;


                if (this.MouseOver)
                {
                    this.sizeBump = Custom.LerpAndTick(this.sizeBump, 1f, 0.1f, 0.1f);
                    this.extraSizeBump = Mathf.Min(1f, this.extraSizeBump + 0.1f);
                    this.sin += 1f;
                    if (!this.flashBool)
                    {
                        this.flashBool = true;
                        this.flash = 1f;
                    }
                    this.col = Mathf.Min(1f, this.col + 0.1f);


                }
                else
                {
                    this.flashBool = false;
                    this.sizeBump = Custom.LerpAndTick(this.sizeBump, 0f, 0.1f, 0.05f);
                    this.col = Mathf.Max(0f, this.col - 0.0333333351f);
                    this.extraSizeBump = 0f;

                }

                Color color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
                this.rect.color = color;


                this.rect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, this.col);
                this.rect.addSize = new Vector2(4f, 4f) * (this.sizeBump + 0.5f * Mathf.Sin(this.extraSizeBump * 3.14159274f)) * ((!Input.GetMouseButton(0)) ? 1f : 0f);
                this.rect.pos = this.pos + new Vector2(-this.rect.addSize.x * 0.5f, 0f);

            }
            private float col; private float sin;
            private float flash; private bool flashBool;
            private float sizeBump; private float extraSizeBump;
                        

            private bool mouseTop;
            public override void Update(float dt)
            {
                

                if (MouseOver)
                {
                    if (!mouseTop)
                    {
                        mouseTop = true;
                        ctrl.cfgMenu.PlaySound(SoundID.MENU_Button_Select_Mouse);
                    }
                    string name = ConfigMenu.currentInterface.Tabs[index].name;
                    if (name == "")
                    {
                        CompletelyOptional.ConfigMenu.description = string.Concat("Switch to Tab No ", index.ToString());
                    }
                    else
                    {
                        CompletelyOptional.ConfigMenu.description = string.Concat("Switch to Tab ", name);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        ctrl.index = this.index;
                    }


                }
                else { mouseTop = false; }

                GrafUpdate(dt);
            }

            public override void Unload()
            {
                base.Unload();
                this.subObjects.Remove(this.rect);
            }

        }

        /// <summary>
        /// When Tab is more than 12
        /// </summary>
        public class SelectButton : UIelement
        {
            public SelectButton(int index, ConfigTabController ctrler) : base(new Vector2(), new Vector2())
            {
                this.index = index;
                this.ctrl = ctrler;

                this._pos = ctrl.pos + new Vector2(18f, 30f * (19 - index) + 3f);
                this.size = new Vector2(30f, 24f);
                this.rect = new DyeableRect(menu, owner, this.pos, this.size, true);
                this.rect.tab = true;
                this.subObjects.Add(this.rect);
                this.rect.fillAlpha = 0.3f;


            }
            private ConfigTabController ctrl;

            public int index;

            public MenuLabel label;
            public DyeableRect rect;


            private bool mouseTop;
            public override void Update(float dt)
            {

                if (MouseOver)
                {
                    if (!mouseTop)
                    {
                        mouseTop = true;
                        ctrl.cfgMenu.PlaySound(SoundID.MENU_Button_Select_Mouse);
                    }

                    string name = ConfigMenu.currentTab.name;
                    if (name == "")
                    {
                        ConfigMenu.description = string.Concat("Switch to Tab No ", index.ToString());
                    }
                    else
                    {
                        ConfigMenu.description = string.Concat("Switch to Tab ", name);
                    }


                    if (Input.GetMouseButtonDown(0))
                    {
                        ctrl.index = this.index;
                    }


                }
                else { mouseTop = false; }

                GrafUpdate(dt);
            }

            public override void Unload()
            {
                base.Unload();
                this.subObjects.Remove(this.rect);
            }

        }


    }
}
