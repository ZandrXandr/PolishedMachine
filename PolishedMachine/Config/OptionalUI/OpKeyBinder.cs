using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Menu;
using RWCustom;
using Partiality.Modloader;
using PolishedMachine.Config;

namespace OptionalUI
{
    /// <summary>
    /// Simple Key Binder
    /// </summary>
    public class OpKeyBinder : UIconfig
    {
        /// <summary>
        /// Simple Key Binder. Value is the KeyCode in string form.
        /// </summary>
        /// <param name="pos">LeftBottom Position of this UI</param>
        /// <param name="size">Size, minimum size is 30x30.</param>
        /// <param name="modID">Your PartialityMod's ModID</param>
        /// <param name="key">Unique Key for UIconfig</param>
        /// <param name="defaultKey">Default Keycode name</param>
        /// <param name="collisionCheck">Whether you will check the key is colliding with other KeyBinder or not</param>
        /// <param name="ctrlerNo">Which controller you want to bind to. The number is equal to RW control config menu.</param>
        public OpKeyBinder(Vector2 pos, Vector2 size, string modID, string key, string defaultKey, bool collisionCheck = true, BindController ctrlerNo = BindController.AnyController) : base(pos, size, key, defaultKey)
        {
            if (string.IsNullOrEmpty(defaultKey)) { throw new ElementFormatException(this, "OpKeyBinderNull: defaultKey for this keyBinder is null.", key); }
            this.controlKey = string.Concat(modID, "_", key);
            this.modID = modID;
            Vector2 minSize = new Vector2(Mathf.Max(30f, size.x), Mathf.Max(30f, size.y));
            this._size = minSize;
            this.check = collisionCheck;
            this.bind = ctrlerNo;
            this.defaultValue = this.value;

            if (!_init) { return; }
            this.Initalize(defaultKey);
        }


        /// <summary>
        /// Simple Key Binder. Value is the KeyCode in string form.
        /// </summary>
        /// <param name="pos">LeftBottom Position of this UI</param>
        /// <param name="size">Size. minimum size is 30f*30f.</param>
        /// <param name="mod">Your PartialityMod</param>
        /// <param name="key">Unique Key for UIconfig</param>
        /// <param name="defaultKey">Default Keycode name</param>
        /// <param name="collisionCheck">Whether you will check the key is colliding with other KeyBinder or not</param>
        /// <param name="ctrlerNo">Which controller you want to bind to. The number is equal to RW control config menu.</param>
        public OpKeyBinder(Vector2 pos, Vector2 size, PartialityMod mod, string key, string defaultKey, bool collisionCheck = true, BindController ctrlerNo = BindController.AnyController) : this(pos, size, mod.ModID, key, defaultKey)
        {
            if (string.IsNullOrEmpty(defaultKey)) { throw new ElementFormatException(this, "OpKeyBinderNull: defaultKey for this keyBinder is null.", key); }
            this.controlKey = string.Concat(modID, "_", key);
            this.modID = mod.ModID;
            Vector2 minSize = new Vector2(Mathf.Max(30f, size.x), Mathf.Max(30f, size.y));
            this._size = minSize;
            this.check = collisionCheck;
            this.bind = ctrlerNo;

            if (!_init) { return; }
            this.Initalize(defaultKey);
        }

        private void Initalize(string defaultKey)
        {
            if (this.check && BoundKey.ContainsValue(defaultKey))
            { //defaultKey Conflict!
                //string anotherControlKey;
                foreach (KeyValuePair<string, string> item in BoundKey)
                {
                    if (item.Value == defaultKey)
                    {
                        string anotherMod = Regex.Split(item.Key, "_")[0];

                        if (modID != anotherMod)
                        {
                            Debug.LogError(string.Concat("More than two mods are using same defaultKey!", Environment.NewLine,
                            "Conflicting Control: ", item.Key, " & ", controlKey, " (duplicate defalutKey: ", item.Value, ")"
                            ));

                            this._desError = string.Concat("Conflicting defaultKey with Mod ", anotherMod);
                            break;
                        }
                        throw new ElementFormatException(this, "You are using duplicated defaultKey for OpKeyBinders!", key);

                    }
                }
            }
            else { this._desError = ""; }
            if (this.check)
            {
                BoundKey.Add(this.controlKey, defaultKey);
            }
            this._description = "";


            this.rect = new DyeableRect(menu, owner, this.pos, this.size, true);
            this.subObjects.Add(this.rect);
            this.rect.fillAlpha = 0.3f;

            this.label = new MenuLabel(menu, owner, defaultKey, this.pos, this.size, true);
            this.subObjects.Add(this.label);
        }

        
        protected bool check;


        private Dictionary<string, string> BoundKey
        {
            get { return ConfigMenu.BoundKey; }
            set { ConfigMenu.BoundKey = value; }
        }
        private readonly string controlKey; private readonly string modID;

        //clicked ==> input key (Mouse out ==> reset)
        /// <summary>
        /// Boundary
        /// </summary>
        public DyeableRect rect;

        /// <summary>
        /// This displays Key
        /// </summary>
        public MenuLabel label;

        /// <summary>
        /// Which Controller this binder can bind
        /// </summary>
        public enum BindController{
            AnyController = 0,
            Controller1 = 1,
            Controller2 = 2,
            Controller3 = 3,
            Controller4 = 4
        };
        private BindController bind;

        public void SetController(BindController controller){
            this.bind = controller;
        }

        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);

            if (greyedOut)
            {
                this.rect.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
                if(string.IsNullOrEmpty(this._desError))
                {
                    this.label.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
                }
                else
                {
                    this.label.label.color = new Color(0.5f, 0f, 0f);
                }
                return;
            }


            this.flash = Custom.LerpAndTick(this.flash, 0f, 0.03f, 0.166666672f);
            float num = 0.5f - 0.5f * Mathf.Sin((this.sin) / 30f * 3.14159274f * 2f);
            num *= this.sizeBump;


            if (this.MouseOver)
            {
                ConfigMenu.description = this.Getdescription();
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 1f, 0.1f, 0.1f);
                this.extraSizeBump = Mathf.Min(1f, this.extraSizeBump + 0.1f);
                this.sin += 1f;
                if (!this.flashBool)
                {
                    this.flashBool = true;
                    this.flash = 1f;
                }
                this.col = Mathf.Min(1f, this.col + 0.1f);

            }
            else
            {
                this.flashBool = false;
                this.sizeBump = Custom.LerpAndTick(this.sizeBump, 0f, 0.1f, 0.05f);
                this.col = Mathf.Max(0f, this.col - 0.0333333351f);
                this.extraSizeBump = 0f;

            }



            Color color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), this.flash);
            this.rect.color = color;
            this.rect.fillAlpha = Mathf.Lerp(0.3f, 0.6f, this.col);
            this.rect.addSize = new Vector2(4f, 4f) * (this.sizeBump + 0.5f * Mathf.Sin(this.extraSizeBump * 3.14159274f)) * ((!Input.GetMouseButton(0)) ? 1f : 0f);

            if (string.IsNullOrEmpty(this._desError))
            {
                Color myColor = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), Mathf.Max(this.col, this.flash));
                color = Color.Lerp(myColor, Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey), num);
                this.label.label.color = color;
            }
            else
            {
                Color myColor = Color.Lerp(Color.red, Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), Mathf.Max(this.col, this.flash));
                color = Color.Lerp(myColor, new Color(0.4f, 0f, 0f), num);
                this.label.label.color = color;
            }
            

        }
        private float col; private float sin;
        private float flash; private bool flashBool;
        private float sizeBump; private float extraSizeBump;

        private bool anyKeyDown; private bool lastAnyKeyDown;

        public string Getdescription()
        {
            if (!string.IsNullOrEmpty(_desError))
            { return _desError; }
            if (!string.IsNullOrEmpty(_description))
            { return _description; }
            return "Click this and Press any key to bind";
        }

        public void Setdescription(string value)
        { _description = value; }
        private string _description;
        private string _desError;

        public override string value
        {
            get { return base.value; }
            set
            {
                if (base.value != value)
                {
                    if (!this.check || !BoundKey.ContainsValue(value))
                    {
                        this._desError = "";
                    }
                    else
                    {
                        foreach (KeyValuePair<string, string> item in BoundKey)
                        {
                            if (item.Value == value)
                            {
                                if(item.Key == this.controlKey) { break; }
                                string anotherMod = Regex.Split(item.Key, "_")[0];
                                if(anotherMod != this.modID)
                                {
                                    if(anotherMod == "Vanilla")
                                    {
                                        this._desError = string.Concat("Conflicting key with Vanilla Options");
                                    }
                                    else
                                    {
                                        this._desError = string.Concat("Conflicting key with Mod ", anotherMod);
                                    }
                                }
                                else
                                {
                                    this._desError = string.Concat("The key [", value, "] you have assigned is already in use");
                                }
                                break;
                            }
                        }
                    }
                    if(string.IsNullOrEmpty(this._desError))
                    {
                        this.ForceValue(value);
                        menu.PlaySound(SoundID.MENU_Button_Successfully_Assigned);
                    }
                    else
                    {
                        menu.PlaySound(SoundID.MENU_Error_Ping);
                    }
                    if (this.check)
                    {
                        BoundKey.Remove(controlKey);
                        BoundKey.Add(controlKey, value);
                    }
                    OnChange();
                }
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (greyedOut) { return; }

            if (this.MouseOver && Input.GetMouseButton(0))
            {
                this.held = true;
            }

            this.lastAnyKeyDown = this.anyKeyDown;
            this.anyKeyDown = Input.anyKey;

            if (this.held)
            {
                if (!this.lastAnyKeyDown && this.anyKeyDown)
                {
                    foreach (object obj in Enum.GetValues(typeof(KeyCode)))
                    {
                        KeyCode keyCode = (KeyCode)((int)obj);
                        if (Input.GetKey(keyCode))
                        {
                            string text = keyCode.ToString();
                            if (text.Length > 4 && text.Substring(0, 5) == "Mouse")
                            {
                                if (!this.MouseOver)
                                {
                                    menu.PlaySound(SoundID.MENU_Error_Ping);
                                    this.held = false;
                                }
                                return;
                            }
                            if (this.value != text)
                            {
                                
                                if (text.Length > 8 && text.Substring(0, 8) == "Joystick")
                                {
                                    if (this.bind != BindController.AnyController)
                                    {
                                        int b = (int)this.bind;
                                        text = text.Substring(0, 8) + b.ToString() + text.Substring(8);
                                    }
                                }

                                this.value = text;
                                this.held = false;
                            }
                            break;
                        }
                    }
                }


            }
            else if (!this.held && this.MouseOver && Input.GetMouseButton(0))
            {
                this.held = true;
                menu.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                this.label.label.text = "?";
            }


        }

        internal override void OnChange()
        {
            if (!_init) { return; }
            base.OnChange();
            this.label.text = this.value;
            Debug.Log(this.value);
        }

        public override void Hide()
        {
            base.Hide();
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = false;
            }
            this.label.label.isVisible = false;
        }

        public override void Show()
        {
            base.Show();
            foreach (FSprite sprite in this.rect.sprites)
            {
                sprite.isVisible = true;
            }
            this.label.label.isVisible = true;
        }


        public override void Unload()
        {
            base.Unload();
            this.subObjects.Remove(this.rect);
            this.subObjects.Remove(this.label);
            BoundKey.Remove(this.controlKey);
        }
    }
}
