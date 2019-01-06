using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OptionalUI;
using Menu;


namespace CompletelyOptional
{
    /// <summary>
    /// Special kind of OpTab for ConfigMenu. You don't need this.
    /// </summary>
    public class MenuTab : OpTab
    {
        public MenuTab(string name = "") : base(name)
        {
        }


        public ConfigTabController tabCtrler;
        
        public new void Update(float dt)
        {
            //if (this.isHidden || !init) { return; }
            foreach(UIelement item in this.items)
            {
                item.Update(dt);
            }
            /*
            for (int i = 0; i < this.items.Count; i++)
            {
                this.items[i].Update(dt);
            }
            this.tabCtrler.Update(dt);*/
        }

        public new void Unload()
        {
            foreach (UIelement item in this.items)
            {
                item.Unload();
            }
            foreach (UIelement item in this.tabCtrler.subElements)
            {
                item.Unload();
            }

        }

    }
}
