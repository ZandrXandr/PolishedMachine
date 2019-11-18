using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using Menu;

namespace OptionalUI
{
    public class OpTextBox : UIconfig
    {
        /// <summary>
        /// Simply TextBox.
        /// </summary>
        /// <param name="pos">LeftBottom</param>
        /// <param name="sizeX">Size (minimum size = 30f)</param>
        /// <param name="key">Key for this UIconfig</param>
        /// <param name="defaultValue">Default string value</param>
        /// <param name="accept">Which type of text you want to allow</param>
        public OpTextBox(Vector2 pos, float sizeX, string key, string defaultValue = "TEXT", Accept accept = Accept.StringASCII) : base(pos, new Vector2(30f, 24f), key, defaultValue)
        {
            Vector2 minSize = new Vector2(Mathf.Max(30f, sizeX), 24f);
            this._size = minSize;

            this.accept = accept;
            this.ForceValue(defaultValue);
            this._lastValue = defaultValue;
            this.maxLength = Mathf.FloorToInt((size.x - 20f) / 6f);
            this.allowSpace = false;
            this.password = false;
            this.mouseDown = false;
            this.keyboardOn = false;

            if (!init) { return; }

            this.rect = new DyeableRect(menu, owner, this.pos, this.size, true) { fillAlpha = 0.5f };
            this.subObjects.Add(this.rect);

            this.label = new MenuLabel(menu, owner, defaultValue, this.pos + new Vector2(5f - this._size.x * 0.5f, 12f), new Vector2(this._size.x, 16f), false);
            this.label.label.alignment = FLabelAlignment.Left;
            this.label.label.SetAnchor(0f, 1f);
            this.label.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
            //this.label.label.SetPosition(new Vector2(5f, 3f) - (this._size * 0.5f));
            this.subObjects.Add(this.label);


            this.cursor = new FContainer();
            this.curSpr = new FSprite[3];
            for(int i = 0; i < 3; i++)
            {
                this.curSpr[i] = new FSprite("pixel", true)
                {
                    color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White),
                    scaleX = 2f,
                    scaleY = 2f
                };
                this.curSpr[i].SetAnchor(0f, 0f);
                this.cursor.AddChild(curSpr[i]);
            }
            this.curSpr[0].scaleX = 7f;
            this.curSpr[0].SetPosition(0f, 8f);
            this.curSpr[1].scaleY = 5f;
            this.curSpr[1].SetPosition(6f, 5f);
            this.curSpr[2].SetPosition(5f, 4f);

            cursor.SetPosition(new Vector2(this.value.Length * 7f + 6f, this.size.y * 0.5f - 7f));
            this.myContainer.AddChild(this.cursor);


        }
        /// <summary>
        /// MenuLabel of TextBox.
        /// </summary>
        public MenuLabel label;
        /// <summary>
        /// Boundary.
        /// </summary>
        public DyeableRect rect;
        private string _lastValue;
        private readonly FContainer cursor;
        private readonly FSprite[] curSpr;
        private float cursorAlpha;


        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);
            if (greyedOut)
            {
                keyboardOn = false;
                Color c = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
                this.label.label.color = c;
                this.cursor.alpha = 0f;
                this.rect.color = c;
                return;
            }
            Color grey = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
            Color white = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);

            //this.label.label.color = grey;
            //this.rect.color = grey;

            flash = Mathf.Max(0f, flash - 0.05f);
            if (keyboardOn)
            {
                this.col = Mathf.Min(1f, this.col + 0.1f);
            }
            else
            {
                this.col = Mathf.Max(0f, this.col - 0.0333333351f);
            }

            this.rect.fillAlpha = Mathf.Lerp(0.5f, 0.8f, this.col);
            Color edge = Color.Lerp(grey, white, this.col);
            this.rect.color = edge;
            Color txt = Color.Lerp(grey, white, Mathf.Clamp(flash, 0f, 1f));
            this.label.label.color = txt;

        }
        private float col = 0f; private float flash;


        public override void Update(float dt)
        {
            base.Update(dt);


            if (keyboardOn)
            {
                cursor.alpha = Mathf.Clamp(cursorAlpha, 0f, 1f);
                cursor.SetPosition(new Vector2(this.value.Length * 7f + 6f, this.size.y * 0.5f - 7f));
                cursorAlpha -= 0.05f;
                if (cursorAlpha < -0.5f) { cursorAlpha = 2f; }

                //input spotted! ->cursorAlpha = 2.5f;
                foreach (char c in Input.inputString)
                {
                    //Debug.Log(string.Concat("input: ", c));
                    if (c == '\b')
                    {
                        cursorAlpha = 2.5f; flash = 2.5f;
                        if (this.value.Length > 0)
                        {
                            this.ForceValue(this.value.Substring(0, this.value.Length - 1));
                            if (!soundFilled)
                            {
                                soundFill += 12;
                                menu.PlaySound(SoundID.MENY_Already_Selected_MultipleChoice_Clicked);
                            }
                            OnChange();
                        }
                        break;
                    }
                    else if ((c == '\n') || (c == '\r')) // enter/return
                    {
                        keyboardOn = false;
                        this.cursor.isVisible = false;
                        if (this.accept == Accept.Float)
                        {
                            if (!float.TryParse(this.value, out float temp))
                            {
                                for (int i = this.value.Length - 1; i > 0; i--)
                                {
                                    if (float.TryParse(this.value.Substring(0, i), out temp))
                                    {
                                        this.ForceValue(this.value.Substring(0, i));
                                        OnChange();
                                        menu.PlaySound(SoundID.Mouse_Light_Switch_On);
                                        return;
                                    }
                                }
                            }
                        }
                        menu.PlaySound(SoundID.MENU_Checkbox_Check);
                        break;
                    }
                    else
                    {
                        cursorAlpha = 2.5f;
                        flash = 2.5f;
                        this.value += c;
                    }

                }
            }
            else { cursorAlpha = 0f; }
            if (Input.GetMouseButton(0) && !mouseDown)
            {
                mouseDown = true;
                if (MouseOver && !keyboardOn)
                { //Turn on
                    menu.PlaySound(SoundID.MENU_Button_Select_Gamepad_Or_Keyboard);
                    keyboardOn = true;
                    this.cursor.isVisible = true;
                    this.cursorAlpha = 1f;
                    cursor.SetPosition(new Vector2(this.value.Length * 7f + 6f, this.size.y * 0.5f - 7f));
                }
                else if(!MouseOver && keyboardOn)
                { //Shutdown
                    keyboardOn = false;
                    this.cursor.isVisible = false;
                    if (this.accept == Accept.Float)
                    {
                        if (!float.TryParse(this.value, out _))
                        {
                            for (int i = this.value.Length - 1; i > 0; i--)
                            {
                                if (float.TryParse(this.value.Substring(0, i), out _))
                                {
                                    this.ForceValue(this.value.Substring(0, i));
                                    OnChange();
                                    menu.PlaySound(SoundID.Mouse_Light_Switch_On);
                                    return;
                                }
                            }
                        }
                    }
                    menu.PlaySound(SoundID.MENU_Checkbox_Uncheck);
                }
            }
            else if(!Input.GetMouseButton(0))
            {
                mouseDown = false;
            }
        }
        private bool mouseDown;
        private bool keyboardOn;



        /// <summary>
        /// If you want to hide what is written for whatever reason,
        /// even though Rain World is singleplayer game.
        /// </summary>
        public bool password;
        /// <summary>
        /// whether you allow space or not. default is false.
        /// </summary>
        public bool allowSpace;
        /// <summary>
        /// Which type of string this accept
        /// </summary>
        public Accept accept;
        /// <summary>
        /// maximum length. default is 100.
        /// </summary>
        public int maxLength
        {
            get { return _maxLength; }
            set
            {
                if (value < 2 || this._maxLength == value) { return; }
                _maxLength = value;
                if(this.value.Length > _maxLength)
                {
                    this.value = this.value.Substring(0, _maxLength);
                }
            }
        }
        private int _maxLength;

        public enum Accept
        {
            Int,
            Float,
            StringEng,
            StringASCII
        }

        /// <summary>
        /// value in int form.
        /// </summary>
        public new int valueInt
        {
            get
            {
                switch (this.accept)
                {
                    default: return 0;
                    case Accept.Int:
                        return int.Parse(value);
                    case Accept.Float:
                        float temp;
                        if (float.TryParse(value, out temp))
                        {
                            return Mathf.FloorToInt(temp);
                        }
                        return 0;
                }
            }
            set
            {
                this.value = value.ToString();
            }
        }

        /// <summary>
        /// value in float form.
        /// </summary>
        public new float valueFloat
        {
            get
            {
                switch (this.accept)
                {
                    default: return 0f;
                    case Accept.Int:
                        return (float)int.Parse(value);
                    case Accept.Float:
                        float temp;
                        if(float.TryParse(value, out temp))
                        {
                            return temp;
                        }
                        return 0f;
                }
            }
            set
            {
                this.value = value.ToString();
            }
        }

        public override string value {
            get { return base.value; }
            set
            {
                if(base.value == value) { return; }
                _lastValue = base.value;
                ForceValue(value);
                if (!this.allowSpace)
                {
                    char[] temp0 = base.value.ToCharArray();
                    for (int t = 0; t < temp0.Length; t++)
                    {
                        if (!char.IsWhiteSpace(temp0[t])) { continue; }
                        ForceValue(_lastValue);
                        _lastValue = base.value;
                        return;
                    }
                }

                switch (this.accept)
                {
                    case Accept.Int:
                        if (Regex.IsMatch(base.value, "^[0-9]+$")) { goto accepted; }
                        break;
                    case Accept.Float:
                        if (Regex.IsMatch(base.value, "^[0-9/./-]+$")) { goto accepted; }
                        break;
                    default:
                    case Accept.StringEng:
                        if (Regex.IsMatch(base.value, "^[a-zA-Z/s]+$")) { goto accepted; }
                        break;
                    case Accept.StringASCII:
                        if (Regex.IsMatch(base.value, "^[\x20-\x7E/s]+$")) { goto accepted; }
                        break;
                }
                //revert
                ForceValue(_lastValue);
                _lastValue = base.value;
                return;

                accepted:
                if (Input.anyKey) {
                    if (!soundFilled)
                    {
                        soundFill += 12;
                        menu.PlaySound(SoundID.MENU_Checkbox_Uncheck);
                    }
                }
                
                OnChange();
            }
        }

        public override void OnChange()
        {
            if (!init) { return; }
            base.OnChange();


            if (!this.password)
            {
                this.label.label.text = this.value;
            }
            else
            {
                this.label.text = "";
                for (int n = 0; n < this.value.Length; n++)
                {
                    this.label.text += "#";
                }
            }
            
        }

        public override void Hide()
        {
            base.Hide();
            this.keyboardOn = false;
            this.label.label.isVisible = false;
            this.cursor.isVisible = false;
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = false;
            }
        }

        public override void Show()
        {
            base.Show();

            this.label.label.isVisible = true;
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = true;
            }
        }

        public override void Unload()
        {
            base.Unload();
            this.subObjects.Remove(this.label);
            this.subObjects.Remove(this.rect);

            this.myContainer.RemoveChild(this.cursor);
            this.cursor.RemoveFromContainer();
        }

    }



}
