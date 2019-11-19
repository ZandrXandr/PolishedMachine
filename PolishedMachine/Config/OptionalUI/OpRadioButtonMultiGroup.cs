﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CompletelyOptional;
using Menu;

namespace OptionalUI
{
    public class OpRadioButtonMultiGroup : OpRadioButtonGroup
    {
        /// <summary>
        /// Allows you to chose fixed number of multiple options in RadioGroup.
        /// If you want non-fixed number of choices, just use OpCheckBox.
        /// Max number of radiobutton on this is 15.
        /// </summary>
        /// <param name="key">Unique Key for this UIconfig</param>
        /// <param name="defaultSet">Default values</param>
        /// <param name="multi">Number of choices you want. 2 or more.</param>
        public OpRadioButtonMultiGroup(string key, string defaultSet, int multi = 2) : base(key, 0)
        {
            this.multi = Math.Max(2, multi);
            if (defaultSet.Length < this.multi)
            {
                throw new ElementFormatException(this, "OpRadioButtonMultiGroup init error: Length of Default values must be equal to number of choices", key);
            }
            this.ForceValue(defaultSet.Substring(0, this.multi));
            this.valueOrder = this.value;
            if (!init) { return; }

            this._greyedOut = false;

            this.subObjects = new List<PositionedMenuObject>(0);
        }

        public readonly int multi;

        public int[] GetValueInts()
        {
            int[] result = new int[multi];
            for (int i = 0; i < multi; i++)
            {
                if (!int.TryParse(this.value.Substring(i, 1), out result[i]))
                {
                    switch (this.value.Substring(i, 1).ToUpper())
                    {
                        case "A":
                            result[i] = 10; break;
                        case "B":
                            result[i] = 11; break;
                        case "C":
                            result[i] = 12; break;
                        case "D":
                            result[i] = 13; break;
                        case "E":
                            result[i] = 14; break;
                        case "F":
                            result[i] = 15; break;
                        default:
                            result[i] = 0; break;
                    }
                }
            }
            return result;
        }
        public void SetValueInts(int[] newValue)
        {
            if (newValue.Length < multi)
            {
                throw new ElementFormatException(this, "OpRadioButtonMultiGroup.SetValueInts error: newValue's length must be equal to num of selection", this.key);
            }
            string s = string.Empty;
            for (int i = 0; i < multi; i++)
            {
                s += newValue[i].ToString("X1");
            }
            this.ForceValue(s);
        }

        public override void SetButtons(OpRadioButton[] buttons)
        {
            if (buttons.Length > 15) { throw new ElementFormatException(this, "Too many RadioButtons assigned to OpRadioButtonMultiGroup! must be lower than 16.", this.key); }
            this.buttons = buttons;
            int[] list = this.GetValueInts();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].group = this;
                buttons[i].index = i;
                if (list.Contains<int>(i))
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


        public override void Switch(int index)
        {
            this.valueOrder = this.valueOrder.Substring(1, this.value.Length - 1) + index.ToString("X1");
            this.value = this.valueOrder;
            int[] l = this.GetValueInts();
            for (int i = 0; i < this.buttons.Length; i++)
            {
                if (l.Contains(i)) { this.buttons[i]._value = "true"; }
                else { this.buttons[i]._value = "false"; }
                this.buttons[i].OnChange();
            }
        }

        private string valueOrder;

        public override void OnChange()
        {
            if (!init) { return; }
            base.OnChange();
            this.Reorder();
        }
        private void Reorder()
        {
            List<int> l = GetValueInts().ToList<int>();
            l.Sort();
            string s = string.Empty;
            for (int i = 0; i < multi; i++)
            {
                s += l[i].ToString("X1");
            }
            this.ForceValue(s);
        }

    }
}