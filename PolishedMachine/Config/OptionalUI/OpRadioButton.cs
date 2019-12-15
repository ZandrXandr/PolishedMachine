using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Menu;
using RWCustom;

namespace OptionalUI
{
    public class OpRadioButton : UIelement
    {
        /// <summary>
        /// Size is fixed (24x24)
        /// This returns value in "true" of "false", although this is NOT UIconfig thus this value won't be saved
        /// </summary>
        /// <param name="pos">LeftBottom of the Button (12x12 offset from center)</param>
        public OpRadioButton(Vector2 pos) : base(pos, new Vector2(24f, 24f))
        {
            this._value = "false";
            if (!_init) { return; }
            
            this.rect = new DyeableRect(menu, owner, this.pos, this.size, true);
            this.subObjects.Add(rect);
            this.symbolSprite = new FSprite("Menu_Symbol_Clear_All", true);
            this.myContainer.AddChild(this.symbolSprite);
            this.symbolSprite.SetAnchor(0f, 0f);
            this.symbolSprite.SetPosition(2f, 2f);
            
            //this.group = group;
            this.index = -1;
            this.greyedOut = false;

            this.sin = 0f;

            this.click = false;

            this.description = "Press to choose this option";
        }
        //Use Circle from food.
        public FSprite symbolSprite;
        private bool click;
        public DyeableRect rect;

        /// <summary>
        /// Whether this button is greyedOut or not
        /// </summary>
        public bool greyedOut;


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
                this.symbolSprite.alpha = this.symbolHalfVisible * 0.5f;
            }


            this.rect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, this.col);
            this.rect.addSize = new Vector2(4f, 4f) * (this.sizeBump + 0.5f * Mathf.Sin(this.extraSizeBump * 3.14159274f)) * ((!Input.GetMouseButton(0)) ? 1f : 0f);



        }

        /// <summary>
        /// size: mouseOver : big / pressed/leave : smol
        /// symbolColor : mouseOver : flash(not selected: 0.5flash) / not: 1/0
        /// 
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);
            if (greyedOut) { return; }
            

            if (this.MouseOver && Input.GetMouseButton(0))
            {
                if (!this.click)
                {
                    this.click = true;
                    this.menu.PlaySound((this.value != "true") ? SoundID.MENU_MultipleChoice_Clicked : SoundID.MENY_Already_Selected_MultipleChoice_Clicked);
                    this.value = "true";
                }

            }
            else
            {
                this.click = false;
            }

            
        }

        private float col;
        private float flash; private bool flashBool;
        private float sizeBump; private float extraSizeBump;
        private float sin; private float symbolHalfVisible;

        /// <summary>
        /// OpRadioButtonGroup this button is belong to.
        /// This will be automatically set when you SetButtons for Group.
        /// </summary>
        public OpRadioButtonGroup group;
        /// <summary>
        /// Index of this button.
        /// </summary>
        public int index;


        public string _value;
        /// <summary>
        /// OpRadioButton is not UIconfig, so this value will not be saved.
        /// (OpRadioButtonGroup is the one gets saved instead)
        /// </summary>
        public virtual string value
        {
            get
            {
                return _value;
            }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    group.Switch(index);
                    OnChange();
                }
            }
        }


        /// <summary>
        /// Access value in bool form.
        /// </summary>
        public bool valueBool
        {
            set
            {
                if (value)
                {
                    this.value = "true";
                }
                else
                {
                    this.value = "false";
                }
            }
            get
            {
                if (this._value == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public override void Hide()
        {
            base.Hide();
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = false;
            }
            this.symbolSprite.isVisible = false;
        }

        public override void Show()
        {
            base.Show();
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = true;
            }
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
