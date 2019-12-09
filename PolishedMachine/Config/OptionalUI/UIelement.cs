using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.ComponentModel;
using Menu;
using RWCustom;

namespace OptionalUI
{
    /// <summary>
    /// UIelement for Partiality Mod Config Canvas(800x600)
    /// </summary>
    public class UIelement
    {
        /// <summary>
        /// UIelement.
        /// </summary>
        /// <param name="pos">BottomLeft Position</param>
        /// <param name="size">Size in pxl</param>
        public UIelement(Vector2 pos, Vector2 size)
        {
            this.isRectangular = true;
            this._pos = pos + offset;
            this._size = size;
            if (init)
            {
                this.menu = CompletelyOptional.OptionScript.configMenu;
                this.owner = CompletelyOptional.OptionScript.configMenu.pages[0];
                this.subObjects = new List<PositionedMenuObject>();
                this.nextSelectable = new PositionedMenuObject[4];
                this.myContainer = new FContainer();
                this.myContainer.SetPosition(this._pos);
                this.myContainer.scaleX = 1f;
                this.myContainer.scaleY = 1f;
            }
            this.description = "";

            //CompletelyOptional.OptionScript.uielements.Add(this);
        }
        /// <summary>
        /// UIelement.
        /// </summary>
        /// <param name="pos">BottomLeft Position</param>
        /// <param name="rad">Radian in pxl</param>
        public UIelement(Vector2 pos, float rad)
        {

            this.isRectangular = false;
            this._pos = pos + offset;
            this._rad = rad;
            if (init)
            {
                this.menu = CompletelyOptional.OptionScript.configMenu;
                this.owner = CompletelyOptional.OptionScript.configMenu.pages[0];
                this.subObjects = new List<PositionedMenuObject>();
                this.nextSelectable = new PositionedMenuObject[4];
                this.myContainer = new FContainer();
                this.myContainer.SetPosition(this._pos);
            }

            //CompletelyOptional.OptionScript.uielements.Add(this);
        }
        public virtual void Reset()
        {
            
        }

        /// <summary>
        /// Offset from BottomLeft of the screen.
        /// </summary>
        public static Vector2 offset = new Vector2(538f, 120f);

        /// <summary>
        /// Prevent Sound Engine from Crashing.
        /// </summary>
        public static int soundFill
        {
            get
            {
                return CompletelyOptional.OptionScript.soundFill;
            }
            set
            {
                CompletelyOptional.OptionScript.soundFill = value;
            }
        }
        /// <summary>
        /// Whether the Sound Engine is full or not.
        /// </summary>
        public static bool soundFilled
        {
            get
            {
                return soundFill > 80;
            }
        }

        /// <summary>
        /// Whether this is in ConfigMenu or not.
        /// </summary>
        public static bool init => CompletelyOptional.OptionScript.init;

        /// <summary>
        /// Position of this element.
        /// </summary>
        public Vector2 pos
        {
            get
            {
                return _pos;
            }
            set
            {
                if (_pos != value + offset)
                {
                    _pos = value + offset;
                    if (init)
                    {
                        OnChange();
                    }
                }
            }
        }
        internal Vector2 _pos;

        /// <summary>
        /// Size of this element.
        /// </summary>
        public Vector2 size
        {
            get
            {
                return _size;
            }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    if (init)
                    {
                        OnChange();
                    }
                }
            }
        }
        internal Vector2 _size;
        /// <summary>
        /// Radian of the element.
        /// </summary>
        public float rad
        {
            get
            {
                return _rad;
            }
            set
            {
                if (_rad != value)
                {
                    _rad = value;
                    if (init)
                    {
                        OnChange();
                    }
                }
            }
        }
        internal float _rad;

        public Menu.Menu menu;
        /// <summary>
        /// Whether the element is Rectangular or Circular(false)
        /// </summary>
        public readonly bool isRectangular;
        /// <summary>
        /// OpTab this element is belong to.
        /// </summary>
        public OpTab tab;



        public Page page
        {
            get
            {
                if (this.owner is Page)
                {
                    return this.owner as Page;
                }
                return this.owner.page;
            }
        }

        /// <summary>
        /// MenuObject this element have.
        /// </summary>
        public List<PositionedMenuObject> subObjects;
        public Page owner;
        public PositionedMenuObject[] nextSelectable;
        public FContainer myContainer;



        /// <summary>
        /// When this element needs graphical change.
        /// </summary>
        public virtual void OnChange()
        {
            if (!init) { return; }
            this.myContainer.SetPosition(this.ScreenPos);
        }

        internal Vector2 ScreenPos
        {
            get
            {
                if (this.owner == null)
                {
                    return this.pos;
                }
                return (this.owner as PositionedMenuObject).ScreenPos + this.pos;
            }
        }

        /// <summary>
        /// Whether mousecursor is over this element or not.
        /// </summary>
        public virtual bool MouseOver
        {
            get
            {
                Vector2 screenPos = this.ScreenPos;
                if (this.isRectangular)
                {
                    return this.menu.mousePosition.x > screenPos.x && this.menu.mousePosition.y > screenPos.y && this.menu.mousePosition.x < screenPos.x + this.size.x && this.menu.mousePosition.y < screenPos.y + this.size.y;
                }
                else
                {
                    return Custom.DistLess(new Vector2(screenPos.x + rad, screenPos.y + rad), this.menu.mousePosition, rad);
                }
            }
        }

        /// <summary>
        /// Mouse Position on UIelement
        /// </summary>
        public Vector2 MousePos
        {
            get
            {
                Vector2 screenPos = this.ScreenPos;
                if (this.MouseOver)
                {
                    return new Vector2(this.menu.mousePosition.x - screenPos.x, this.menu.mousePosition.y - screenPos.y);
                }
                else
                {
                    return new Vector2(-1, -1);
                }
            }
        }


        /// <summary>
        /// Infotext that will show underneath.
        /// </summary>
        public string description;


        /// <summary>
        /// Update that happens every frame.
        /// </summary>
        /// <param name="dt">deltaTime</param>
        public virtual void Update(float dt)
        {
            if (!init) { return; }
            foreach (MenuObject obj in this.subObjects)
            {
                obj.Update();
                obj.GrafUpdate(dt);
            }
            this.myContainer.SetPosition(this.ScreenPos);
            GrafUpdate(dt);
            showDesc = !tab.isHidden && this.MouseOver && !string.IsNullOrEmpty(this.description);
            if (showDesc && !(this is UIconfig))
            {
                CompletelyOptional.ConfigMenu.description = this.description;
            }
        }
        public bool showDesc;

        /// <summary>
        /// Update that happens every frame,
        /// but this is only for graphical detail,
        /// for visiblity of Update code.
        /// </summary>
        /// <param name="dt">deltaTime</param>
        public virtual void GrafUpdate(float dt)
        {

        }


        /// <summary>
        /// Called when exiting ConfigMenu.
        /// </summary>
        public virtual void Unload()
        {
            this.myContainer.RemoveAllChildren();
        }

        /// <summary>
        /// When the tab this element belongs to gets hidden.
        /// </summary>
        public virtual void Hide()
        {
            this.myContainer.isVisible = false;
        }
        /// <summary>
        /// When the tab this element belongs to displays.
        /// </summary>
        public virtual void Show()
        {
            this.myContainer.isVisible = true;
        }

    }
}
