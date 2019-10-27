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
        /// <param name="pos">left-bottom corner coordinate</param>
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
            float m = Mathf.Min(multi, 1.0f);
            this.size = this.vertical ? new Vector2(24f, Mathf.Min(32f, r * m)) : new Vector2(Mathf.Min(32f, r * m), 24f);
            this.ForceValue(Custom.IntClamp(defaultValue, min, max).ToString());
            if (!init) { return; }
            Initialize();
        }

        /// <summary>
        /// Slider that let you input integer
        /// </summary>
        /// <param name="pos">left-bottom corner coordinate</param>
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
            float l = Mathf.Max((float)r, 32f, (float)length);
            this.size = this.vertical ? new Vector2(24f, Mathf.Min(32f, l)) : new Vector2(Mathf.Min(32f, l), 24f);
            this.ForceValue(Custom.IntClamp(defaultValue, min, max).ToString());
            if (!init) { return; }
            Initialize();
        }


        private void Initialize()
        {
            this.rect = new DyeableRect(menu, owner, this.pos, new Vector2((!this.vertical) ? 16f : 24f, (!this.vertical) ? 24f : 16f), true);
            this.subObjects.Add(this.rect);
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
            }
            this.label = new MenuLabel(menu, owner, this.value, this.rect.pos, new Vector2(50f, 30), false);
            this.label.label.alignment = FLabelAlignment.Center;
            this.subObjects.Add(this.label);
        }


        private readonly int min, max;
        private readonly bool vertical;

        private FSprite[] lineSprites;
        private DyeableRect rect;
        private MenuLabel label;

        bool SelectableUIelement.IsMouseOverMe { get { return !this.held && this.MouseOver; } }

        bool SelectableUIelement.CurrentlySelectableMouse { get { return !this.greyedOut; } }

        bool SelectableUIelement.CurrentlySelectableNonMouse { get { return true; } }


        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);

        }

        public override void Update(float dt)
        {
            base.Update(dt);

        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void Unload()
        {
            base.Unload();

        }

    }
}
