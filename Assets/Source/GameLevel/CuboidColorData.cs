using Gooyes.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Cubic
{
    public class CuboidColorData : Singleton<CuboidColorData>
    {
        [SerializeField] private Color32[] _colors;
        private Dictionary<int, Color32> _colorMap;

        private void Start()
        {
            CreateMap();
        }

        public Color32 GetColor(int power)
        {
            if (_colorMap.TryGetValue(power, out Color32 color)) return color;
            else return _colorMap[_colorMap.Count];
        }

        private void CreateMap()
        {
            if (_colorMap == null) _colorMap = new Dictionary<int, Color32>();
            if (_colors == null || _colors.Length <= 0) return;
            for (int i = 0; i < _colors.Length; i++)
            {
                _colorMap[i + 1] = _colors[i];
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            CreateMap();
        }
    }
}
