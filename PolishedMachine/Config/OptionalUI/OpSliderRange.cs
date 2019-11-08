using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RWCustom;
using UnityEngine;
using Menu;

namespace OptionalUI
{
    public class OpSliderRange : OpSlider
    {
        public OpSliderRange(Vector2 pos, string key, IntVector2 range, float multi = 1, bool vertical = false, int defaultValue = 0) : base(pos, key, range, multi, vertical, defaultValue)
        {
        }

        public OpSliderRange(Vector2 pos, string key, IntVector2 range, int length, bool vertical = false, int defaultValue = 0) : base(pos, key, range, length, vertical, defaultValue)
        {
        }

        internal override void Initialize()
        {
            base.Initialize();
        }


    }
}
