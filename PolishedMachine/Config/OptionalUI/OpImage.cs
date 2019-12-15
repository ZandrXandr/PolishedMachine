using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using PolishedMachine.Config;

namespace OptionalUI
{
    /// <summary>
    /// Image Display
    /// </summary>
    public class OpImage : UIelement
    {
        /// <summary>
        /// Simply Display Texture2D.
        /// </summary>
        /// <param name="pos">Left/Bottom Position</param>
        /// <param name="image">Image you want to display</param>
        public OpImage(Vector2 pos, Texture2D image) : base(pos, new Vector2(image.width, image.height))
        {
            if (image == null) { throw new ElementFormatException(this, "There is no Texture2D for OpImage"); }
            if (!_init) {
                this.sprite = new FSprite("pixel", true);
                return;
            }

            seed = Mathf.FloorToInt(UnityEngine.Random.value * 10000);

            Futile.atlasManager.LoadAtlasFromTexture(salt + "image", image);

            this.sprite = new FSprite(salt + "image", true);
            this.sprite.SetAnchor(0f, 0f);
            this.myContainer.AddChild(this.sprite);

            this._alpha = 1f;
            this.isTexture = true;
        }
        /// <summary>
        /// Show FAtlasElement in RainWorld.
        /// </summary>
        /// <param name="pos">Left/Bottom Position</param>
        /// <param name="fAtlasElement">Name of FAtlasElement in Rain World you want to display</param>
        public OpImage(Vector2 pos, string fAtlasElement) : base(pos, Vector2.zero)
        {
            if (!_init)
            {
                this.sprite = new FSprite("pixel", true);
                return;
            }

            FAtlasElement element;
            try
            {
                element = Futile.atlasManager.GetElementWithName(fAtlasElement);
            }
            catch (Exception ex)
            {
                throw new ElementFormatException(this, string.Concat("There is no such FAtlasElement called ", fAtlasElement, " : ",Environment.NewLine, ex.ToString()));
            }

            this.sprite = new FSprite(element.name, true);
            this.myContainer.AddChild(this.sprite);
            this._size = element.sourceSize;
            this.sprite.SetAnchor(0f, 0f);

            this._alpha = 1f;
            this.isTexture = false;
        }

        /// <summary>
        /// Swap Image to new one
        /// </summary>
        /// <param name="newImage">new image</param>
        public void ChangeImage(Texture2D newImage)
        {
            if (!_init) { return; }
            if (!isTexture) { throw new ElementFormatException(this, "You must construct this with Texture2D to use this function"); }
            Futile.atlasManager.UnloadAtlas(salt + "image");
            myContainer.RemoveAllChildren();
            Futile.atlasManager.LoadAtlasFromTexture(salt + "image", newImage);
            this.sprite = new FSprite(salt + "image", true);
            this.sprite.SetAnchor(0f, 0f);
            this.myContainer.AddChild(this.sprite);
        }

        public FSprite sprite;
        private readonly bool isTexture;
        private readonly int seed;
        public string salt
        {
            get
            {
                return seed.ToString("D4");
            }
        }

        /// <summary>
        /// Alpha of OpImage.
        /// </summary>
        public float alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                if(_alpha != value)
                {
                    _alpha = value;
                    OnChange();
                }
            }
        }
        private float _alpha;

        /// <summary>
        /// Set Color. This does not work on OpImage based on Texture2D.
        /// </summary>
        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                if (!isTexture && _color != value)
                {
                    _color = value;
                    OnChange();
                }
            }
        }
        private Color _color;

        internal override void OnChange()
        {
            if (!_init) { return; }
            base.OnChange();
            sprite.alpha = _alpha;
            if (!isTexture)
            {
                sprite.color = _color;
            }
            sprite.SetPosition(Vector2.zero);

        }
        /*
        public override bool MouseOver
        {
            get
            {
            if(centered)
                return this.menu.mousePosition.x > pos.x && this.menu.mousePosition.y > pos.y && this.menu.mousePosition.x < pos.x + this.size.x && this.menu.mousePosition.y < pos.y + this.size.y;
            }
        }*/


        public override void Hide()
        {
            base.Hide();
            sprite.isVisible = false;
        }
        public override void Show()
        {
            base.Show();
            sprite.isVisible = true;
        }

        public override void Unload()
        {
            base.Unload();
            if (isTexture)
            { //remove element
                Futile.atlasManager.UnloadAtlas(salt + "image");
            }

        }


    }
}
