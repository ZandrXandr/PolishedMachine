using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RWCustom;
using UnityEngine;
using Menu;
using PolishedMachine.Config;

namespace OptionalUI
{
    public class OpSliderSubtle : OpSlider, SelectableUIelement
    {
        /// <summary>
        /// SubtleSlider that let you input integer in small range
        /// </summary>
        /// <param name="pos">left-bottom corner coordinate (excluding extra length in the end of slider line)</param>
        /// <param name="key">unique keyword for this UIconfig</param>
        /// <param name="range">x = min, y = max</param>
        /// <param name="length">length of this slider will be this (min 20 * range)</param>
        /// <param name="vertical">if true, the slider will go vertical and the length will be used as height</param>
        /// <param name="defaultValue">default integer value</param>
        public OpSliderSubtle(Vector2 pos, string key, IntVector2 range, int length, bool vertical = false, int defaultValue = 0) : base(pos, key, range, length, vertical, defaultValue)
        {
            int r = range.y - range.x + 1;
            if(r > 31) { throw new ElementFormatException(this, "The range of OpSliderSubtle should be lower than 31! Use normal OpSlider instead.", key); }
            float l = Mathf.Max((float)r, 32f, (float)length);
            this.mul = Mathf.Max(l / r, 20f);
            this._size = this.vertical ? new Vector2(24f, Mathf.Max(32f, l)) : new Vector2(Mathf.Max(32f, l), 24f);
            this.ForceValue(Custom.IntClamp(defaultValue, min, max).ToString());
            this.wheelTick = 1;

            if (!init) { return; }

            this.Nobs = new FSprite[r];
            this.s = 10f;
            for (int i = 0; i < this.Nobs.Length; i++)
            {
                this.Nobs[i] = new FSprite("Menu_Subtle_Slider_Nob", true) { anchorX = 0f, anchorY = 0f };
                this.myContainer.AddChild(this.Nobs[i]);
            }
            this.Circle = new FSprite("buttonCircleB", true) { anchorX = 0f, anchorY = 0f, scale = 0.5f };
            this.myContainer.AddChild(this.Circle);

            this.myContainer.sortZ = this.Nobs[0].sortZ + 1f;
        }

        internal override void Initialize()
        {
            base.Initialize();
            if (!init) { return; }

        }

#pragma warning disable IDE0044 // not gonna add readonly
        private FSprite[] Nobs;
        private FSprite Circle;
#pragma warning restore IDE0044
        private float s;


        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);
            this.lineSprites[0].isVisible = false;
            this.lineSprites[3].isVisible = false;
            float m = ((this.vertical ? this.size.y : this.size.x) + 24f) / (float)(this.max - this.min + 1);
            for (int i = 0; i < this.Nobs.Length; i++)
            {
                if (this.vertical) { this.Nobs[i].x = 12.01f; this.Nobs[i].y = m * i + 0.01f; }
                else { this.Nobs[i].y = 12.01f; this.Nobs[i].x = m * i + 0.01f; }
                this.Nobs[i].scale = this.s / 10f;
                this.Nobs[i].color = this.lineSprites[0].color;
            }
            if (this.vertical) { this.Circle.x = 12.01f; this.Circle.y = m * (this.valueInt - this.min) + 0.01f; }
            else { this.Circle.y = 12.01f; this.Circle.x = m * (this.valueInt - this.min) + 0.01f; }
            this.Circle.scale = this.flash * 0.5f + 0.5f;
            this.Circle.color = Color.Lerp(Menu.Menu.MenuColor(Menu.Menu.MenuColors.White).rgb, this.lineSprites[0].color, 0.5f);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (this.greyedOut)
            {
                this.s = Custom.LerpAndTick(this.s, 0f, 0.08f, 0.333333343f);
                return;
            }

            this.s = Custom.LerpAndTick(this.s, this.flash * 6f + 10f, 0.08f, 0.333333343f);
        }

        public override void OnChange()
        {
            base.OnChange();
        }

        public override void Show()
        {
            base.Show();
            for (int i = 0; i < this.Nobs.Length; i++)
            {
                this.Nobs[i].isVisible = true;
            }
        }

        public override void Hide()
        {
            base.Hide();
            for (int i = 0; i < this.Nobs.Length; i++)
            {
                this.Nobs[i].isVisible = false;
            }
        }

        public override void Unload()
        {
            base.Unload();
        }

    }
}
