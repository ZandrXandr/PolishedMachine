using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Menu;
using RWCustom;
using UnityEngine;

namespace OptionalUI
{
    /// <summary>
    /// RoundedRect + color to dye edge,
    /// so CompletelyOptional can mimic RW styled UI
    /// without actually using Menu.ButtonTemplate.
    /// </summary>
    public class DyeableRect : RoundedRect
    {
        public DyeableRect(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size, bool filled = true) : base(menu, owner, pos, size, filled)
        {
            this.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
            this.filled = filled;
            this.tab = false;
            if (filled)
            {
                tabInvisible = new List<FSprite>();
                tabInvisible.Add(this.sprites[2]);
                tabInvisible.Add(this.sprites[6]);
                tabInvisible.Add(this.sprites[7]);
                tabInvisible.Add(this.sprites[11]);
                tabInvisible.Add(this.sprites[15]);
                tabInvisible.Add(this.sprites[16]);
            }
        }

        private new int SideSprite(int side)
        {
            return ((!this.filled) ? 0 : 9) + side;
        }

        private new int CornerSprite(int corner)
        {
            return ((!this.filled) ? 0 : 9) + 4 + corner;
        }

        //public int[] visibleIndex = { 0, 1, 3, 4, 5, 8, 9, 10, 12, 13, 14 };
        private List<FSprite> tabInvisible;

        /// <summary>
        /// Edge Color of this Rect.
        /// </summary>
        public Color color;
        private new bool filled;

        /// <summary>
        /// whether you cut right side or not.
        /// </summary>
        public bool tab;

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
            if (tab)
            {
                foreach (FSprite edge in tabInvisible) { edge.isVisible = false; }
            }
            for (int i = 0; i < 4; i++)
            {
                this.sprites[this.SideSprite(i)].color = this.color;
                this.sprites[this.CornerSprite(i)].color = this.color;
            }
        }

    }
}
