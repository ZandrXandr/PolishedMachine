using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Menu;
using UnityEngine;
using RWCustom;

namespace OptionalUI
{
    /// <summary>
    /// Simply Rounded Rectangle
    /// </summary>
    public class OpRect : UIelement
    {
        /// <summary>
        /// Create Rounded Rectangle for Decoration
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="size">Size</param>
        /// <param name="alpha">0f ~ 1f (default: 0.3f)</param>
        public OpRect(Vector2 pos, Vector2 size, float alpha = 0.3f) : base(pos, size)
        {
            this.alpha = alpha;
            if (!init) { return; }
            
            this.rect = new DyeableRect(menu, owner, this.pos, size, true);
            this.subObjects.Add(this.rect);
            Color grey = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
            for (int i = 9; i < this.rect.sprites.Length; i++)
            {
                this.rect.sprites[i].color = grey;
            }

            this.doesBump = false;
        }
        /// <summary>
        /// MenuObject of this Element.
        /// </summary>
        public DyeableRect rect;


        /// <summary>
        /// If you want this Rect to react with MouseOver
        /// </summary>
        public bool doesBump;

        /// <summary>
        /// fillAlpha of Rect. (Ignored when doesBump)
        /// </summary>
        public float alpha;

        private float col = 0f;
        private float sizeBump = 0f;
        private float extraSizeBump = 0f;

        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);

            Color grey = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);

            if (!doesBump)
            {
                this.rect.fillAlpha = this.alpha;
                for (int i = 9; i < this.rect.sprites.Length; i++)
                {
                    this.rect.sprites[i].color = grey;
                }
                return;
            }

            if (this.MouseOver)
            {
                this.col = Mathf.Min(1f, this.col + 0.1f);
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 1f, 0.1f, 0.1f);
                this.extraSizeBump = Mathf.Min(1f, this.extraSizeBump + 0.1f);
            }
            else
            {
                this.col = Mathf.Max(0f, this.col - 0.0333333351f);
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 0f, 0.1f, 0.05f);
                this.extraSizeBump = 0f;
            }

            this.rect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, this.col);
            this.rect.addSize = new Vector2(4f, 4f) * (this.sizeBump + 0.5f * Mathf.Sin(this.extraSizeBump * 3.14159274f)) * ((this.MouseOver) ? 1f : 0f);
            Color edge = Color.Lerp(grey, Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.col);
            this.rect.color = edge;

        }

        public override void Update(float dt)
        {
            if (!init) { return; }
            base.Update(dt);
        }


        public override void Unload()
        {
            base.Unload();
            this.subObjects.Remove(this.rect);
        }

        public override void Hide()
        {
            base.Hide();
            foreach(FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = false;
            }
            
        }

        public override void Show()
        {
            base.Show();
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = true;
            }
        }

    }
}
