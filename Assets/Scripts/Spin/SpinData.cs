using System.Collections.Generic;
using UnityEngine;

namespace DevCase.Core
{
    [CreateAssetMenu(fileName = "SpinData", menuName = "DevCase/SpinData", order = 0)]
    public class SpinData : ScriptableObject
    {
        [Header("Design Settings")]
        public int SilverZoneSpinCount;
        public int GoldZoneSpinCount;
        public SpinRewardEntry[] SpinRewardEntries = new SpinRewardEntry[8];
        [Header("Visual Settings")]
        public List<string> SpinNames = new List<string>();
        public List<Sprite> SpinWheelSprites = new List<Sprite>();
        public List<Sprite> SpinIndicatorSprites = new List<Sprite>();
        public List<Color> SpinColors = new List<Color>();
        public List<Sprite> SpinRewardTypeSprites = new List<Sprite>();
    }
}
