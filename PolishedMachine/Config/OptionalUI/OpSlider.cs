using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RWCustom;
using Menu;

//RangeSlider, SubtleSlider
namespace OptionalUI
{
    public class OpSlider : UIconfig, SelectableUIelement
    {
        /// <summary>
        /// Slider that let you input integer
        /// </summary>
        /// <param name="pos">left-bottom corner coordinate (excluding extra length in the end of slider line)</param>
        /// <param name="key">unique keyword for this UIconfig</param>
        /// <param name="range">x = min, y = max</param>
        /// <param name="multi">length of this slider will be this value * range. minimum 1f</param>
        /// <param name="vertical">if true, the slider will go vertical and the length will be used as height</param>
        /// <param name="defaultValue">default integer value</param>
        public OpSlider(Vector2 pos, string key, IntVector2 range, float multi = 1.0f, bool vertical = false, int defaultValue = 0) : base(pos, new Vector2(), key, defaultValue.ToString())
        {
            this.vertical = vertical;
            this.min = range.x; this.max = range.y;
            int r = this.max - this.min + 1;
            this.wheelTick = r > 5 ? Math.Max(Mathf.CeilToInt(r / 12f), 4) : 1;
            float m = Mathf.Max(multi, 1.0f);
            this.mul = m;
            this._size = this.vertical ? new Vector2(24f, Mathf.Max(32f, r * m)) : new Vector2(Mathf.Max(32f, r * m), 24f);
            this.ForceValue(Custom.IntClamp(defaultValue, min, max).ToString());
            if (!init) { return; }
            Initialize();
        }

        /// <summary>
        /// Slider that let you input integer
        /// </summary>
        /// <param name="pos">left-bottom corner coordinate (excluding extra length in the end of slider line)</param>
        /// <param name="key">unique keyword for this UIconfig</param>
        /// <param name="range">x = min, y = max</param>
        /// <param name="length">length of this slider will be this (min 32)</param>
        /// <param name="vertical">if true, the slider will go vertical and the length will be used as height</param>
        /// <param name="defaultValue">default integer value</param>
        public OpSlider(Vector2 pos, string key, IntVector2 range, int length, bool vertical = false, int defaultValue = 0) : base(pos, new Vector2(), key, defaultValue.ToString())
        {
            this.vertical = vertical;
            this.min = range.x; this.max = range.y;
            int r = this.max - this.min + 1;
            this.wheelTick = r > 5 ? Math.Max(Mathf.CeilToInt(r / 12f), 4) : 1;
            float l = Mathf.Max((float)r, 32f, (float)length);
            this.mul = l / r;
            this._size = this.vertical ? new Vector2(24f, Mathf.Max(32f, l)) : new Vector2(Mathf.Max(32f, l), 24f);
            this.ForceValue(Custom.IntClamp(defaultValue, min, max).ToString());
            if (!init) { return; }
            Initialize();
        }

        private bool subtleSlider
        {
            get { return this is OpSliderSubtle; }
        }
        /* private bool rangeSlider
        {
            get { return this is OpSliderRange; }
        }*/


        internal virtual void Initialize()
        {
            this.lineSprites = new FSprite[4];
            for (int i = 0; i < this.lineSprites.Length; i++)
            {
                this.lineSprites[i] = new FSprite("pixel", true);
                this.myContainer.AddChild(this.lineSprites[i]);
            }
            if (this.vertical)
            {
                this.lineSprites[0].scaleY = 2f;
                this.lineSprites[0].scaleX = 6f;
                this.lineSprites[1].scaleX = 2f;
                this.lineSprites[1].anchorY = 0f;
                this.lineSprites[2].scaleX = 2f;
                this.lineSprites[2].anchorY = 1f;
                this.lineSprites[3].scaleY = 2f;
                this.lineSprites[3].scaleX = 6f;
                if (!subtleSlider) this.rect = new DyeableRect(menu, owner, this.pos + new Vector2(4f, -8f), new Vector2(24f, 16f), true);
                //if (rangeSlider) { this.lineSprites[4].scaleX = 2f; this.lineSprites[4].anchorY = 1f; }
            }
            else
            {
                this.lineSprites[0].scaleX = 2f;
                this.lineSprites[0].scaleY = 6f;
                this.lineSprites[1].scaleY = 2f;
                this.lineSprites[1].anchorX = 0f;
                this.lineSprites[2].scaleY = 2f;
                this.lineSprites[2].anchorX = 1f;
                this.lineSprites[3].scaleX = 2f;
                this.lineSprites[3].scaleY = 6f;
                if (!subtleSlider) this.rect = new DyeableRect(menu, owner, this.pos + new Vector2(-8f, 4f), new Vector2(16f, 24f), true);
                //if (rangeSlider) { this.lineSprites[4].scaleY = 2f; this.lineSprites[4].anchorX = 1f; }
            }
            if (!subtleSlider)
            {
                this.subObjects.Add(this.rect);
                this.label = new MenuLabel(menu, owner, this.value, this.rect.pos, new Vector2(40f, 30f), false);
                this.label.label.alpha = 0f;
                this.label.label.alignment = FLabelAlignment.Center;
                this.subObjects.Add(this.label);
                this.myContainer.sortZ = this.rect.sprites[0].sortZ + 1f;
            }
            
        }

        /// <summary>
        /// amount that changes when you move your mousewheel over OpSlider
        /// </summary>
        public int wheelTick;

        internal readonly int min, max;
        internal readonly bool vertical;
        internal float mul;

        internal FSprite[] lineSprites;
        internal DyeableRect rect;
        internal MenuLabel label;

        internal float flash;
        private float greyFade, sin, sizeBump, extraSizeBump, col;
        private bool flashBool;

        bool SelectableUIelement.IsMouseOverMe { get { return !this.held && this.MouseOver; } }

        bool SelectableUIelement.CurrentlySelectableMouse { get { return !this.greyedOut; } }

        bool SelectableUIelement.CurrentlySelectableNonMouse { get { return true; } }


        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);

            Vector2 a, a2;
            if (subtleSlider)
            {
                float t = (this.valueInt - this.min) * this.mul;
                a = new Vector2(this.pos.x + t, this.pos.y + 12f);
                a2 = new Vector2(24f, 24f);
                a -= this.flash * new Vector2(8f, 8f);
                a2 += this.flash * new Vector2(16f, 16f);
            }
            else
            {
                a = this.rect.DrawPos(dt);
                a2 = this.rect.DrawSize(dt);
                a -= Vector2.Lerp(this.rect.lastAddSize, this.rect.addSize, dt) / 2f;
                a2 += Vector2.Lerp(this.rect.lastAddSize, this.rect.addSize, dt);
            }
            a.x = Mathf.Floor(a.x) + 0.01f;
            a.y = Mathf.Floor(a.y) + 0.01f;
            a2.x = Mathf.Round(a2.x) + 0.01f;
            a2.y = Mathf.Round(a2.y) + 0.01f;
            a -= this.pos; a2 -= this.pos;
            float p = this.mul * (this.valueInt - this.min);
            if (!vertical)
            {
                this.lineSprites[1].x = 0f;
                this.lineSprites[1].y = 15f;
                this.lineSprites[1].isVisible = (a.x > 0f);
                this.lineSprites[1].scaleX = a.x;
                this.lineSprites[2].x = this.size.x;
                this.lineSprites[2].y = 15f;
                this.lineSprites[2].isVisible = (a.x + a2.x < this.size.x + 12f);
                this.lineSprites[2].scaleX = this.size.x; // + 12f - (a.x + a2.x);
                this.lineSprites[0].x = 0f;
                this.lineSprites[0].y = 15f;
                this.lineSprites[0].isVisible = (a.x > 0f);
                this.lineSprites[3].x = this.size.x; // + this.ExtraLengthAtEnd;
                this.lineSprites[3].y = 15f;
                this.lineSprites[3].isVisible = (a.x + a2.x < this.size.x + 12f);
                if (!subtleSlider)
                {
                    this.rect.pos.x = this.pos.x + p - 8f;
                    this.label.pos = new Vector2(this.rect.pos.x - 14f, this.rect.pos.y + 20f);
                }
            }
            else
            {
                this.lineSprites[1].y = 0f;
                this.lineSprites[1].x = 15f;
                this.lineSprites[1].isVisible = (a.y > 0f);
                this.lineSprites[1].scaleY = a.y;
                this.lineSprites[2].y = this.size.y;
                this.lineSprites[2].x = 15f;
                this.lineSprites[2].isVisible = (a.y + a2.y < this.size.y + 12f);
                this.lineSprites[2].scaleY = this.size.y;
                this.lineSprites[0].y = 0f;
                this.lineSprites[0].x = 15f;
                this.lineSprites[0].isVisible = (a.y > 0f);
                this.lineSprites[3].y = this.size.y;
                this.lineSprites[3].x = 15f;
                this.lineSprites[3].isVisible = (a.y + a2.y < this.size.y + 12f);
                if (!subtleSlider)
                {
                    this.rect.pos.y = this.pos.y + p - 8f;
                    this.label.pos = new Vector2(this.rect.pos.x - 10f, this.rect.pos.y + 12f);
                }
            }

            if (this.greyedOut)
            {
                foreach (FSprite s in this.lineSprites) { s.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey); }
                if (subtleSlider) return;
                for (int r = 0; r < 9; r++)
                {
                    this.rect.sprites[r].color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black);
                }
                for (int r = 9; r < 17; r++)
                {
                    this.rect.sprites[r].color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey);
                }
                this.label.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
                if (this.MouseOver) { this.label.label.alpha = Mathf.Min(0.5f, this.label.label.alpha + 0.05f); }
                else { this.label.label.alpha = Mathf.Max(0f, this.label.label.alpha - 0.1f); }
                return;
            }

            this.flash = Custom.LerpAndTick(this.flash, 0f, 0.01f, 0.166666672f);
            this.greyFade = Custom.LerpAndTick(this.greyFade, (!CompletelyOptional.ConfigMenu.freezeMenu || this.held) ? 0f : 1f, 0.05f, 0.025f);
            float num = 0.5f - 0.5f * Mathf.Sin(this.sin / 30f * 3.14159274f * 2f);
            num *= this.sizeBump;

            if (MouseOver || flashBool)
            {
                this.extraSizeBump = Mathf.Min(1f, this.extraSizeBump + 0.1f);
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 1f, 0.1f, 0.1f);
                this.col = Mathf.Min(1f, this.col + 0.1f);
                this.sin += 1f;
                if (this.flashBool)
                {
                    this.flashBool = false;
                    this.flash = Mathf.Min(1f, this.flash + 0.4f);
                }
                if (!subtleSlider) this.label.label.alpha = Mathf.Min(this.label.label.alpha + 0.1f, 1f);
            }
            else
            {
                this.extraSizeBump = 0f;
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 0f, 0.1f, 0.05f);
                this.col = Mathf.Max(0f, this.col - 0.0333333351f);
                if (!this.held && !subtleSlider) this.label.label.alpha = Mathf.Max(this.label.label.alpha - 0.15f, 0f);
            }

            this.flash = Mathf.Max(0f, this.flash - 0.142857149f);

            if (!subtleSlider)
            {
                this.rect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, this.col);
                this.rect.addSize = new Vector2(4f, 4f) * (this.sizeBump + 0.5f * Mathf.Sin(this.extraSizeBump * 3.14159274f));
            }

            Color color;
            float alpha = 0.5f + 0.5f * Mathf.Sin(this.sin / 30f * 3.14159274f * 2f);
            if (this.held)
            {
                if (!subtleSlider)
                {
                    color = Color.Lerp(Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), num), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
                    for (int r = 0; r < 9; r++)
                    {
                        this.rect.sprites[r].color = color; this.rect.sprites[r].alpha = alpha;
                    }
                    color = Color.Lerp(color, Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
                    for (int r = 9; r < 17; r++)
                    {
                        this.rect.sprites[r].color = color;
                    }
                }
                color = Color.Lerp(Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey), num), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
            }
            else
            {
                if (!subtleSlider)
                {
                    color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), Mathf.Max(num, this.greyFade));
                    for (int r = 0; r < 9; r++)
                    {
                        this.rect.sprites[r].color = color; this.rect.sprites[r].alpha = alpha;
                    }
                    color = Color.Lerp(color, Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
                    for (int r = 9; r < 17; r++)
                    {
                        this.rect.sprites[r].color = color;
                    }
                }
                color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Mathf.Max(num, this.greyFade));
            }
            foreach (FSprite l in this.lineSprites) { l.color = color; }
            if (!subtleSlider) this.label.label.color = Color.Lerp(this.rect.sprites[9].color, Menu.Menu.MenuColor(Menu.Menu.MenuColors.White).rgb, 0.5f);
            
            


        }

        public override void Update(float dt)
        {

            base.Update(dt);
            if (greyedOut) { return; }

            if (this.held)
            {
                if (Input.GetMouseButton(0))
                {
                    float t = this.vertical ? Input.mousePosition.y - this.pos.y : Input.mousePosition.x - this.pos.x;
                    this.valueInt = Mathf.Clamp(Mathf.RoundToInt(t / this.mul) + this.min, this.min, this.max);
                }
                else
                {
                    this.held = false;
                }

            }
            else if (!this.held && this.menu.manager.menuesMouseMode && this.MouseOver)
            {
                if (Input.GetMouseButton(0))
                {
                    this.held = true;
                    menu.PlaySound(SoundID.MENU_First_Scroll_Tick);
                }
                else if (this.menu.mouseScrollWheelMovement != 0)
                {
                    int num = valueInt - (int)Mathf.Sign(this.menu.mouseScrollWheelMovement) * this.wheelTick;
                    num = Custom.IntClamp(num, this.min, this.max);
                    if (num != valueInt)
                    {
                        this.flash = Mathf.Min(1f, this.flash + 0.7f);
                        this.menu.PlaySound(SoundID.MENU_Scroll_Tick);
                        this.sizeBump = Mathf.Min(2.5f, this.sizeBump + 1f);
                        this.valueInt = num;
                    }
                }
            }


        }

        public override void OnChange()
        {
            base.OnChange();
            if (MouseOver || held)
            {
                if (!soundFilled)
                {
                    soundFill += 5;
                    menu.PlaySound(SoundID.MENU_Scroll_Tick);
                }
                this.flashBool = true;
            }
            if (!subtleSlider) this.label.label.text = this.value;
        }

        public override void Show()
        {
            base.Show();
            for (int i = 0; i < lineSprites.Length; i++) { lineSprites[i].isVisible = true; }
            if (!subtleSlider)
            {
                for (int j = 0; j < this.rect.sprites.Length; j++) { this.rect.sprites[j].isVisible = true; }
                this.label.label.isVisible = true;
            }
        }

        public override void Hide()
        {
            base.Hide();
            for (int i = 0; i < lineSprites.Length; i++) { lineSprites[i].isVisible = false; }
            if (!subtleSlider)
            {
                for (int j = 0; j < this.rect.sprites.Length; j++) { this.rect.sprites[j].isVisible = false; }
                this.label.label.isVisible = false;
            }
        }

        public override void Unload()
        {
            if (!subtleSlider)
            {
                this.subObjects.Remove(this.rect);
                this.subObjects.Remove(this.label);
            }
            for (int i = 0; i < lineSprites.Length; i++) { lineSprites[i].RemoveFromContainer(); }
            base.Unload();
        }

    }
}
