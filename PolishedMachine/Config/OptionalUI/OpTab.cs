using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OptionalUI
{
    public class OpTab
    {
        /// <summary>
        /// Tab. 600 pxl * 600 pxl.
        /// </summary>
        public OpTab(string name = "")
        {
            menu = false;
            this.items = new List<UIelement>();
            this.isHidden = true;
            this._name = name;
        }

        public List<SelectableUIelement> selectables;


        /// <summary>
        /// Do not use this.
        /// </summary>
        public bool menu;

        public string name
        {
            get { return _name; }
        }
        private readonly string _name;

        public bool isHidden;
        /// <summary>
        /// Use OptionInterface.init instead.
        /// </summary>
        [Obsolete]
        public bool init
        {
            get { return PolishedMachine.Config.OptionScript.init; }
        }


        public List<UIelement> items;

        /// <summary>
        /// Update for OpTab. Automatically called. Don't use this by yourself.
        /// </summary>
        /// <param name="dt">deltaTime</param>
        public void Update(float dt)
        {
            if (this.isHidden || !PolishedMachine.Config.OptionScript.init) { return; }

            foreach (UIelement item in this.items)
            {
                item.Update(dt);
            }
        }

        /// <summary>
        /// Obsolete! Use AddItems instead.
        /// </summary>
        /// <param name="item">UIelement</param>
        [Obsolete]
        public void AddItem(UIelement item)
        {
            this._AddItem(item);
        }

        private void _AddItem(UIelement item)
        {
            if (this.items.Contains(item)) { return; }
            this.items.Add(item);
            item.SetTab(this);
        }

        /// <summary>
        /// Add Multiple UIelements to this Tab.
        /// </summary>
        /// <param name="items">UIelements</param>
        public void AddItems(params UIelement[] items)
        {
            foreach (UIelement item in items) { this._AddItem(item); }
        }

        /// <summary>
        /// Obsolete! Use RemoveItems instead.
        /// </summary>
        /// <param name="item">UIelement</param>
        [Obsolete]
        public void RemoveItem(UIelement item) { _RemoveItem(item); }
        /// <summary>
        /// Remove UIelements in this Tab.
        /// </summary>
        /// <param name="items">UIelements</param>
        public void RemoveItems(params UIelement[] items)
        {
            foreach (UIelement item in items) { this._RemoveItem(item); }
        }

        private void _RemoveItem(UIelement item)
        {
            while (this.items.Contains(item))
            {
                this.items.Remove(item);
            }
            item.SetTab(null);
        }

        /// <summary>
        /// Hide this tab. Automatically called. Don't use this by yourself.
        /// </summary>
        public void Hide()
        {
            this.isHidden = true;
            foreach (UIelement element in this.items)
            {
                element.Hide();
            }
        }
        /// <summary>
        /// Show this tab. Automatically called. Don't use this by yourself.
        /// </summary>
        public void Show()
        {
            this.isHidden = false;
            foreach (UIelement element in this.items)
            {
                element.Show();
            }
        }

        public Dictionary<string, string> GetTabDictionary()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();

            foreach (UIelement element in this.items)
            {
                if (element.GetType().IsSubclassOf(typeof(UIconfig)))
                {
                    if ((element as UIconfig).cosmetic) { continue; }
                    if (config.ContainsKey((element as UIconfig).key))
                    {
                        throw new DupelicateKeyException(this._name, (element as UIconfig).key);
                    }
                    config.Add((element as UIconfig).key, (element as UIconfig).value);
                }
            }

            return config;
        }

        public Dictionary<string, UIconfig> GetTabObject()
        {
            Dictionary<string, UIconfig> config = new Dictionary<string, UIconfig>();

            foreach (UIelement element in this.items)
            {
                if (element.GetType().IsSubclassOf(typeof(UIconfig)))
                {
                    if ((element as UIconfig).cosmetic) { continue; }
                    if (config.ContainsKey((element as UIconfig).key))
                    {
                        throw new DupelicateKeyException(this._name, (element as UIconfig).key);
                    }
                    config.Add((element as UIconfig).key, (element as UIconfig));
                }
            }

            return config;
        }


        public void Unload()
        {
            foreach (UIelement item in this.items)
            {
                item.Unload();
            }

        }


    }
}
