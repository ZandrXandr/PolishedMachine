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


        /// <summary>
        /// Do not use this.
        /// </summary>
        public bool menu;

        public string name
        {
            get { return _name; }
        }
        private string _name;

        public bool isHidden;
        public bool init
        {
            get { return CompletelyOptional.OptionScript.init; }
        }


        public List<UIelement> items;

        public void Update(float dt)
        {
            if (this.isHidden || !init) { return; }
            
            foreach (UIelement item in this.items)
            {
                item.Update(dt);
            }
        }

        /// <summary>
        /// Add UIelement to this Tab.
        /// </summary>
        /// <param name="item">UIelement.</param>
        public void AddItem(UIelement item)
        {
            if (this.items.Contains(item)) { return; }
            this.items.Add(item);
            item.tab = this;
        }

        /// <summary>
        /// Remove UIelement in this Tab.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(UIelement item)
        {
            while (this.items.Contains(item))
            {
                this.items.Remove(item);
            }
            item.tab = null;
        }

        public void Hide()
        {
            this.isHidden = true;
            foreach (UIelement element in this.items)
            {
                element.Hide();
            }
        }
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

            foreach(UIelement element in this.items)
            {
                if (element.GetType().IsSubclassOf(typeof(UIconfig)))
                {
                    if (config.ContainsKey((element as UIconfig).key))
                    {
                        throw new Exception(string.Concat(
                          this._name == "" ? "Tab" : "Tab ", this._name, " has duplicated key for UIconfig.",
                          Environment.NewLine, "(key: ", (element as UIconfig).key, ")"
                          ));
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
                    if(config.ContainsKey((element as UIconfig).key))
                    { throw new Exception(string.Concat(
                        this._name == "" ? "Tab" : "Tab ", this._name, " has duplicated key for UIconfig.",
                        Environment.NewLine, "(key: ", (element as UIconfig).key, ")"
                        ));
                    }
                    config.Add((element as UIconfig).key, (element as UIconfig));
                }
            }

            return config;
        }


        public void Unload()
        {
            foreach(UIelement item in this.items)
            {
                item.Unload();
            }

        }


    }
}
