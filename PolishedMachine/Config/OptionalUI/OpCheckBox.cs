using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Menu;
using RWCustom;

namespace OptionalUI
{
    /// <summary>
    /// Simple CheckBox.
    /// </summary>
    public class OpCheckBox : UIconfig
    {
        /// <summary>
        /// Simply CheckBox which returns "true" of "false". size is fixed to 24x24.
        /// </summary>
        /// <param name="pos">LeftBottom of Pos</param>
        /// <param name="key">Key of this config</param>
        /// <param name="defaultBool">default value</param>
        public OpCheckBox(Vector2 pos, string key, bool defaultBool = false) : base(pos, new Vector2(24f, 24f), key, "false")
        {
            //true of false!
            if (defaultBool)
            {
                this.ForceValue("true");
            }
            else
            {
                this.ForceValue("false");
            }
            if (!init) { return; }

            this.rect = new DyeableRect(menu, owner, this.pos, this.size, true);
            this.subObjects.Add(rect);
            this.symbolSprite = new FSprite("Menu_Symbol_CheckBox", true);
            this.myContainer.AddChild(this.symbolSprite);
            this.symbolSprite.SetAnchor(0f, 0f);
            this.symbolSprite.SetPosition(2f, 2f);
            this.description = "Press to Check/Uncheck";

        }

        /// <summary>
        /// RoundedRect that's forming boundary.
        /// </summary>
        public DyeableRect rect;
        /// <summary>
        /// Symbol FSprite of Check Symbol.
        /// </summary>
        public FSprite symbolSprite;

        

        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);


            if (greyedOut)
            {
                if (valueBool) { this.symbolSprite.alpha = 1f; }
                else { this.symbolSprite.alpha = 0f; }
                this.symbolSprite.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
                this.rect.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
                return;
            }


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
                this.symbolHalfVisible = Custom.LerpAndTick(this.symbolHalfVisible, 1f, 0.07f, 0.0166666675f);




            }
            else
            {
                this.flashBool = false;
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 0f, 0.1f, 0.05f);
                this.col = Mathf.Max(0f, this.col - 0.0333333351f);
                this.extraSizeBump = 0f;
                this.symbolHalfVisible = 0f;

            }

            Color color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
            this.rect.color = color;

            Color myColor = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), Mathf.Max(this.col, this.flash));
            color = Color.Lerp(myColor, Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey), num);
            this.symbolSprite.color = color;

            if (this.valueBool)
            {
                this.symbolSprite.alpha = 1f;
            }
            else
            {
                this.symbolSprite.alpha = this.symbolHalfVisible * 0.25f;
            }


            this.rect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, this.col);
            this.rect.addSize = new Vector2(4f, 4f) * (this.sizeBump + 0.5f * Mathf.Sin(this.extraSizeBump * 3.14159274f)) * ((!Input.GetMouseButton(0)) ? 1f : 0f);

        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (greyedOut) { return; }


            if (this.MouseOver&& Input.GetMouseButton(0))
            {
                if (!mouseDown) {
                    mouseDown = true;
                    this.valueBool = !this.valueBool;
                    menu.PlaySound(this.valueBool ? SoundID.MENU_Checkbox_Check : SoundID.MENU_Checkbox_Uncheck);
                }
            }
            else
            {
                mouseDown = false;
            }

        }

        private bool mouseDown;
        private float col;
        private float flash; private bool flashBool;
        private float sizeBump; private float extraSizeBump;
        private float sin; private float symbolHalfVisible;


        public override void OnChange()
        {
            base.OnChange();

        }

        public override void Hide()
        {
            base.Hide();
            foreach(FSprite sprite in this.rect.sprites) { sprite.isVisible = false; }
            this.symbolSprite.isVisible = false;
        }

        public override void Show()
        {
            base.Show();
            foreach (FSprite sprite in this.rect.sprites) { sprite.isVisible = true; }
            this.symbolSprite.isVisible = true;
        }

        public override void Unload()
        {
            base.Unload();
            this.subObjects.Remove(this.rect);
            this.symbolSprite.RemoveFromContainer();

        }

    }
}
