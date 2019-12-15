using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Menu;
using UnityEngine;

namespace OptionalUI
{
    /// <summary>
    /// Simple Label.
    /// </summary>
    public class OpLabel : UIelement
    {
        /// <summary>
        /// Simply Label that displays text.
        /// </summary>
        /// <param name="pos">BottomLeft Position</param>
        /// <param name="size">Size of the Box</param>
        /// <param name="text">Text you want to display; max length 600</param>
        /// <param name="alignment">Alignment. Left/Center/Right</param>
        /// <param name="bigText">Whether this use bigFont or not</param>
        public OpLabel(Vector2 pos, Vector2 size, string text = "TEXT", FLabelAlignment alignment = FLabelAlignment.Center, bool bigText = false) : base(pos, size)
        {
            Vector2 minSize = new Vector2(Mathf.Max(size.x, 20f), Mathf.Max(size.y, 20f));
            this._size = minSize;
            this._text = text.Length < 600 ? text : text.Substring(0, 600);
            this._bigText = bigText;
            this.lineLength = Mathf.FloorToInt((size.x - 10f) / 6f);
            this.autoWrap = false;
            this.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
            if (_init)
            {
                this.label = new MenuLabel(menu, owner, _text, this.pos, minSize, this._bigText);
                this.label.label.color = this.color;
                this.subObjects.Add(this.label);
            }
            this._alignment = alignment;
            OnChange();
        }
        /// <summary>
        /// Access MenuLabel. Be aware: when IsConfigScreen is false, accessing this will throw NullRefException.
        /// </summary>
        public MenuLabel label;
        private readonly bool _bigText;
        /// <summary>
        /// (default : false) If this is true, OpLabel will automatically make text in MultiLine.
        /// </summary>
        public bool autoWrap;
        private readonly int lineLength;
        public FLabelAlignment alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                if(_alignment != value)
                {
                    _alignment = value;
                    OnChange();
                }
            }
        }
        private FLabelAlignment _alignment;

        /// <summary>
        /// Color of the text. Grey in default.
        /// </summary>
        public Color color;


        internal override void OnChange()
        {
            if (!_init) { return; }
            base.OnChange();
            if (!this.autoWrap)
            {
                this.label.text = _text.Length < 600 ? _text : _text.Substring(0, 600);
            }
            else
            {
                string ml = _text.Length < 600 ? _text : _text.Substring(0, 600);
                char[] array = ml.ToCharArray();
                int d = 0; bool f = true; int l = 0;
                do
                {
                    if (f)
                    { //forward
                        if (array[d] == '\n') { l = 0; d++; continue; }
                        if (l < this.lineLength) { d++; l++; continue; }
                        else { f = false; continue; }

                    }
                    else
                    { //backward
                        if (array[d] == ' ') { array[d] = '\n'; l = 0; d++; f = true; continue; }
                        d--; l--;
                        continue;
                    }
                } while (d < array.Length);

                this.label.text = new string(array);
            }
            switch (this._alignment)
            {
                default:
                case FLabelAlignment.Center:
                    this.label.label.alignment = FLabelAlignment.Center;
                    this.label.pos = this.pos;
                    break;
                case FLabelAlignment.Left:
                    this.label.label.alignment = FLabelAlignment.Left;
                    this.label.pos = this.pos - new Vector2(this.size.x * 0.5f, 0f);
                    break;
                case FLabelAlignment.Right:
                    this.label.label.alignment = FLabelAlignment.Right;
                    this.label.pos = this.pos + new Vector2(this.size.x * 0.5f, 0f);
                    break;

            }

        }

        public string text
        {
            get { return _text; }
            set
            {
                if(_text != value)
                {
                    _text = value;
                    OnChange();
                }
            }
        }
        private string _text;


        public override void Hide()
        {
            base.Hide();
            this.label.label.isVisible = false;
        }
        public override void Show()
        {
            base.Show();
            this.label.label.isVisible = true;
        }

        public override void Unload()
        {
            base.Unload();

            this.subObjects.Remove(this.label);
            //owner.subObjects.Remove(menuObj);
        }


    }
}
