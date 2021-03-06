using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Menu;

namespace OptionalUI
{
    /// <summary>
    /// Ties number of OpRadioButton together, so only one of them can be activated.
    /// Also this is how you access the value of RadioButtons.
    /// This returns valueInt.
    /// After creating OpRadioButton instances, Use SetButtons to bind buttons to the group
    /// </summary>
    public class OpRadioButtonGroup : UIconfig
    {
        public OpRadioButtonGroup(string key, int defaultValue = 0) : base(Vector2.zero, new Vector2(1f, 1f), key, defaultValue.ToString())
        {
            this.ForceValue(defaultValue.ToString());
            this.defaultValue = this.value;
            if (!_init) { return; }
            this._greyedOut = false;

            this.subObjects = new List<PositionedMenuObject>(0);
        }

        /// <summary>
        /// Bind OpRadioButtons to this group
        /// </summary>
        /// <param name="buttons">Array of OpRadioButton</param>
        public virtual void SetButtons(OpRadioButton[] buttons)
        {
            this.buttons = buttons;
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].group = this;
                buttons[i].index = i;
                if (i == base.valueInt)
                {
                    buttons[i]._value = "true";
                }
                else
                {
                    buttons[i]._value = "false";
                }
                buttons[i].OnChange();
            }
        }

        /// <summary>
        /// Whether this RadioButtonGroup is greyedOut or not
        /// Changing this overrides original greyedOut setting of RadioButtons.
        /// </summary>
        public new bool greyedOut
        {
            get { return _greyedOut; }
            set
            {
                if (_greyedOut != value)
                {
                    _greyedOut = value;
                    for (int i = 0; i < this.buttons.Length; i++)
                    {
                        this.buttons[i].greyedOut = value;
                    }
                }
            }
        }
        internal bool _greyedOut;



        public OpRadioButton[] buttons;
        public new int valueInt
        {
            get
            {
                return int.Parse(this.value);
            }
            set
            {
                if (value >= this.buttons.Length) { return; }
                //this._value = value.ToString();

                Switch(value);
            }
        }

        public override string value
        {
            get => base.value;
            set
            {
                if (base.value != value)
                {
                    this.ForceValue(value);
                    if (_init)
                    {
                        Switch(int.Parse(value));
                    }
                }
            }
        }

        public virtual void Switch(int index)
        {
            for (int i = 0; i < this.buttons.Length; i++)
            {
                this.buttons[i]._value = "false";
                this.buttons[i].OnChange();
            }
            this.buttons[index]._value = "true";
            this.buttons[index].OnChange();
            base.value = index.ToString();
            this.OnChange();
        }

        internal override void OnChange()
        {
            if (!_init) { return; }
            base.OnChange();


        }


    }
}
