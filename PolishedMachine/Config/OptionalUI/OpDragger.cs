using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RWCustom;
using Menu;
using UnityEngine;

namespace OptionalUI
{
    /// <summary>
    /// Dragger to adjust int value easily.
    /// </summary>
    public class OpDragger : UIconfig
    {
        /// <summary>
        /// Dragger to adjust int value easily.
        /// size is fixed to 24x24.
        /// </summary>
        /// <param name="pos">BottomLeft</param>
        /// <param name="key">Key of this config</param>
        /// <param name="defaultInt">default value</param>
        public OpDragger(Vector2 pos, string key, int defaultInt = 0) : base(pos, new Vector2(24f, 24f), key, defaultInt.ToString())
        {
            if (!init) { return; }
            this.rect = new DyeableRect(menu, owner, this.pos, this.size, true);
            this.subObjects.Add(this.rect);
            this.label = new MenuLabel(menu, owner, defaultInt.ToString(), this.pos + new Vector2(0f, 2f), new Vector2(24f, 20f), false);
            this.subObjects.Add(this.label);

            this.min = 0; this.max = 99;
            this.description = "Hold your mouse and drag up/down to adjust value";
        }

        /// <summary>
        /// Minimum value. default = 0
        /// </summary>
        public int min;
        /// <summary>
        /// Maximum value. default = 99
        /// </summary>
        public int max;


        /// <summary>
        /// Boundary
        /// </summary>
        public DyeableRect rect;
        /// <summary>
        /// MenuLabel
        /// </summary>
        public MenuLabel label;

        public int valueInt
        {
            get { return int.Parse(_value); }
            set { if(_value != value.ToString()) { this.value = value.ToString(); } }
        }



        private float black { get { return Mathf.Clamp(1f - this.flash, 0f, 1f); } }

        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);
            if (greyedOut)
            {
                this.label.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey);
                this.rect.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey);
                return;
            }

            this.flash = Custom.LerpAndTick(this.flash, 0f, 0.03f, 0.166666672f);
            this.greyFade = Custom.LerpAndTick(this.greyFade, (!CompletelyOptional.ConfigMenu.freezeMenu || this.held) ? 0f : 1f, 0.05f, 0.025f);
            Color color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
            float num = 0.5f - 0.5f * Mathf.Sin(this.sin / 30f * 3.14159274f * 2f);
            num *= this.sizeBump;

            if (MouseOver)
            {
                this.extraSizeBump = Mathf.Min(1f, this.extraSizeBump + 0.1f);
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 1f, 0.1f, 0.1f);
                this.col = Mathf.Min(1f, this.col + 0.1f);
                this.sin += 1f;
                if (!this.flashBool)
                {
                    this.flashBool = true;
                    this.flash = 1f;
                }
            }
            else
            {
                this.flashBool = false;
                this.extraSizeBump = 0f;
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 0f, 0.1f, 0.05f);
                this.col = Mathf.Max(0f, this.col - 0.0333333351f);
            }

            this.flash = Mathf.Max(0f, this.flash - 0.142857149f);

            this.rect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, this.col);
            this.rect.addSize = new Vector2(4f, 4f) * (this.sizeBump + 0.5f * Mathf.Sin(this.extraSizeBump * 3.14159274f));

            if (this.held)
            {
                color = Color.Lerp(Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), num), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
            }
            else
            {
                color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey), Mathf.Max(num, this.greyFade));
            }
            this.label.label.color = color;

            if (this.held)
            {
                color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey), this.flash);
            }
            else
            {
                HSLColor from = HSLColor.Lerp(Menu.Menu.MenuColor(Menu.Menu.MenuColors.DarkGrey), Menu.Menu.MenuColor(Menu.Menu.MenuColors.MediumGrey), Mathf.Max(this.col, this.flash));
                color = HSLColor.Lerp(from, Menu.Menu.MenuColor(Menu.Menu.MenuColors.VeryDarkGrey), this.greyFade).rgb;
            }
            this.rect.color = color;
        }
        private float sizeBump; private float extraSizeBump; private float sin;
        private float flash; private bool flashBool; private float col;
        private float greyFade;


        private float savMouse; private int savValue;
        public override void Update(float dt)
        {
            base.Update(dt);


            int num = valueInt;
            if (this.menu.manager.menuesMouseMode && this.MouseOver)
            {
                int num3 = num;
                num -= (int)Mathf.Sign(this.menu.mouseScrollWheelMovement);
                num = Custom.IntClamp(num, this.min, this.max);
                if (num != num3)
                {
                    //this.flash = 1f;
                    //this.menu.PlaySound(SoundID.MENU_Scroll_Tick);
                    //this.sizeBump = Mathf.Min(2.5f, this.sizeBump + 1f);
                    this.value = num.ToString();
                }
            }

            if (this.held)
            {
                if (Input.GetMouseButton(0))
                {
                    this.value = Custom.IntClamp(this.savValue + Mathf.FloorToInt((Input.mousePosition.y - this.savMouse) / 10f), this.min, this.max).ToString();
                }
                else
                {
                    this.held = false;
                }

            }
            else if (!this.held && this.MouseOver && Input.GetMouseButton(0))
            {
                this.held = true;
                this.savMouse = Input.mousePosition.y;
                this.savValue = this.valueInt;
                menu.PlaySound(SoundID.MENU_First_Scroll_Tick);
            }


        }

        public override void OnChange()
        {
            base.OnChange();
            if (MouseOver || held) {
                if (!soundFilled)
                {
                    soundFill += 5;
                    menu.PlaySound(SoundID.MENU_Scroll_Tick);
                }
                this.flash = 1f;
                this.sizeBump = Mathf.Min(2.5f, this.sizeBump + 1f);
            }
            this.label.label.text = value;
        }

        public override void Hide()
        {
            base.Hide();
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = false;
            }
            this.label.label.isVisible = false;
        }

        public override void Show()
        {
            base.Show();
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = true;
            }
            this.label.label.isVisible = true;
        }

        public override void Unload()
        {
            base.Unload();
            this.subObjects.Remove(this.rect);
            this.subObjects.Remove(this.label);
        }


    }
}
