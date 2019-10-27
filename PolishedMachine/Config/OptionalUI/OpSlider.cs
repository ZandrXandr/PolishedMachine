using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RWCustom;

//RangeSlider, SnapSlider
namespace OptionalUI
{
    public class OpSlider : UIconfig
    {
        /// <summary>
        /// Slider that let you input integer
        /// </summary>
        /// <param name="pos">left-bottom corner coordinate</param>
        /// <param name="length">length in pixel to be dragged (min 24f)</param>
        /// <param name="key">unique keyword for this UIconfig</param>
        /// <param name="horizontal">if false, the slider will go vertical and the length will be used as height</param>
        /// <param name="range">x = min, y = max</param>
        /// <param name="defaultValue">default integer value</param>
        public OpSlider(Vector2 pos, int length, string key, bool horizontal, IntVector2 range, int defaultValue = 0) : base(pos, new Vector2(), key, defaultValue.ToString())
        {
            this.horizontal = horizontal;
            this.size = horizontal ? new Vector2(Mathf.Min(24f, length), 18f) : new Vector2(18f, Mathf.Min(24f, length));
            this.min = range.x; this.max = range.y;
            this.ForceValue(Custom.IntClamp(defaultValue, min, max).ToString());
        }

        private readonly bool horizontal;
        private readonly int min, max;


    }
}
