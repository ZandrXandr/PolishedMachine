using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.ComponentModel;
using Menu;
using RWCustom;
using PolishedMachine.Config;

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
            this._pos = pos + _offset;
            this._size = size;
            if (_init)
            {
                this.menu = OptionScript.configMenu;
                this.owner = OptionScript.configMenu.pages[0];
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
            this._pos = pos + _offset;
            this._rad = rad;
            if (_init)
            {
                this.menu = OptionScript.configMenu;
                this.owner = OptionScript.configMenu.pages[0];
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
        public virtual void Reset()
        {

        }

        /// <summary>
        /// Offset from BottomLeft of the screen.
        /// </summary>
        [Obsolete]
        public static Vector2 offset => _offset;

        /// <summary>
        /// Offset from BottomLeft of the screen.
        /// </summary>
        internal static readonly Vector2 _offset = new Vector2(538f, 120f);


        /// <summary>
        /// Prevent Sound Engine from Crashing. Use OptionInterface one instead.
        /// </summary>
        [Obsolete]
        public static int soundFill => _soundFill;

        internal static int _soundFill
        {
            get
            {
                return OptionScript.soundFill;
            }
            set
            {
                OptionScript.soundFill = value;
            }
        }

        /// <summary>
        /// Whether the Sound Engine is full or not. Use OptionInterface one instead.
        /// </summary>
        [Obsolete]
        public static bool soundFilled => _soundFilled;

        public static bool _soundFilled
        {
            get
            {
                return _soundFill > 80;
            }
        }

        /// <summary>
        /// Whether this is in ConfigMenu or not. Use OptionInterface one instead.
        /// </summary>
        [Obsolete]
        public static bool init => OptionScript.init;

        internal static bool _init => OptionScript.init;


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
                if (_pos != value + _offset)
                {
                    _pos = value + _offset;
                    if (_init)
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
                    if (_init)
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
                if (isRectangular) { throw new InvaildGetPropertyException(this, "rad"); }
                return _rad;
            }
            set
            {
                if (_rad != value)
                {
                    _rad = value;
                    if (_init)
                    {
                        OnChange();
                    }
                }
            }
        }
        internal float _rad;

        internal Menu.Menu menu;
        /// <summary>
        /// Whether the element is Rectangular(true) or Circular(false)
        /// </summary>
        public readonly bool isRectangular;
        /// <summary>
        /// OpTab this element is belong to.
        /// </summary>
        private OpTab tab;
        /// <summary>
        /// Do not use this. Instead, use OpTab.AddItems and OpTab.RemoveItems.
        /// </summary>
        /// <param name="newTab">new OpTab this item will be belong to</param>
        public void SetTab(OpTab newTab) { this.tab = newTab; }



        internal Page page
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
        internal Page owner;
        public PositionedMenuObject[] nextSelectable;
        internal FContainer myContainer;



        /// <summary>
        /// When this element needs graphical change.
        /// </summary>
        internal virtual void OnChange()
        {
            if (!_init) { return; }
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
        internal virtual bool MouseOver
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
            if (!_init) { return; }
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
                ConfigMenu.description = this.description;
            }
        }
        public bool showDesc;

        /// <summary>
        /// Update that happens every frame, but this is only for graphical detail for visiblity of Update code.
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
