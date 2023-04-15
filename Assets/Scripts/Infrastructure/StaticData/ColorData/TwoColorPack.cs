using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Data
{
    [Serializable]
    public class TwoColorPack
    {
        public List<Color> ColorPaletteOne;
        public List<Color> ColorPaletteTwo;
    }
}