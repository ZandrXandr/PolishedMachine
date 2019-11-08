using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RWCustom;
using PolishedMachine.Config;

namespace OptionalUI
{
    /// <summary>
    /// Configuable Settings. Every configuable value is tied to UIconfig and Key.
    /// UIconfig value will be saved automatically when it is added to the tab.
    /// </summary>
    public class UIconfig : UIelement
    {
        /// <summary>
        /// Rectangular UIconfig.
        /// </summary>
        /// <param name="pos">BottomLeft Position</param>
        /// <param name="size">Size</param>
        /// <param name="key">Key: this must be unique</param>
        /// <param name="defaultValue">Default Value</param>
        public UIconfig(Vector2 pos, Vector2 size, string key, string defaultValue = "") : base(pos, size)
        {
            if (string.IsNullOrEmpty(key)) { throw new NullKeyException(this); }
            this._key = key;
            this._value = defaultValue;
            this.greyedOut = false;
            this.held = false;
        }
        /// <summary>
        /// Circular UIconfig.
        /// </summary>
        /// <param name="pos">BottomLeft Position (NOT center!)</param>
        /// <param name="rad">Radian</param>
        /// <param name="key">Key: this must be unique</param>
        /// <param name="defaultValue">Default Value</param>
        public UIconfig(Vector2 pos, float rad, string key, string defaultValue = "") : base(pos, rad)
        {
            if (string.IsNullOrEmpty(key)) { throw new NullKeyException(this); }
            this._key = key;
            this._value = defaultValue;
            this.greyedOut = false;
            this.held = false;
        }

        /// <summary>
        /// Whether this is held or not.
        /// If this is true, this freezes other objects.
        /// </summary>
        public bool held
        {
            get { return _held; }
            set
            {
                if(_held != value)
                {
                    _held = value;
                    CompletelyOptional.ConfigMenu.freezeMenu = value;
                }
            }
        }
        private bool _held;

        /// <summary>
        /// Key
        /// </summary>
        public string key { get { return _key; } }
        private readonly string _key;

        /// <summary>
        /// Whether this button is greyedOut or not
        /// </summary>
        public bool greyedOut;

        /// <summary>
        /// If you want to change value w/o running OnChange().
        /// This is not recommended unless you know what you are doing.
        /// </summary>
        public void ForceValue(string newValue)
        {
            this._value = newValue;
        }

        private string _value;
        /// <summary>
        /// Value.
        /// </summary>
        public virtual string value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (init) { OnChange(); }
                }
            }
        }

        /// <summary>
        /// Value in integer form
        /// </summary>
        public int valueInt
        {
            get { return int.TryParse(value, out int i) ? i : 0; }
            set { this.value = value.ToString(); }
        }

        /// <summary>
        /// Value in float form
        /// </summary>
        public float valueFloat
        {
            get
            {
                return float.TryParse(value, out float d) ? d : 0f;
            }
            set
            {
                this.value = value.ToString();
            }
        }

        /// <summary>
        /// Value in bool form
        /// </summary>
        public bool valueBool
        {
            set { this.value = value ? "true" : "false"; }
            get
            {
                if (this.value == "true") { return true; }
                else { return false; }
            }
        }

        public override void OnChange()
        {
            base.OnChange();
            CompletelyOptional.OptionScript.configChanged = true;
            (menu as CompletelyOptional.ConfigMenu).saveButton.menuLabel.text = "APPLY";
        }

        /// <summary>
        /// Separates Graphical update for code-visiblilty.
        /// </summary>
        /// <param name="dt">deltaTime</param>
        public override void GrafUpdate(float dt)
        {
            base.GrafUpdate(dt);
            
        }

        /// <summary>
        /// Update that happens every frame.
        /// </summary>
        /// <param name="dt">deltaTime</param>
        public override void Update(float dt)
        {
            if (!init) { return; }
            base.Update(dt);
            if (showDesc && !this.greyedOut)
            {
                CompletelyOptional.ConfigMenu.description = this.description;
            }
        }

    }



    
}
