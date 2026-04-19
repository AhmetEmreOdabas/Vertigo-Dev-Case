using DevCase.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace DevCase.Core
{
    public class GameSpriteData : MonoSingleton<GameSpriteData>
    {
        [SerializeField] private SpinData _spinData;
        private Dictionary<RewardTypes, Sprite> _spinRewardTypesSpriteDict = new Dictionary<RewardTypes, Sprite>();
        public void Initialize()
        {
            for (int i = 0; i < _spinData.SpinRewardTypeSprites.Count; i++)
            {
                if (i < System.Enum.GetValues(typeof(RewardTypes)).Length)
                {
                    _spinRewardTypesSpriteDict.Add((RewardTypes)i, _spinData.SpinRewardTypeSprites[i]);
                }
            }
        }
        public Sprite GetSpinRewardSprite(RewardTypes rewardType)
        {
            if (_spinRewardTypesSpriteDict.TryGetValue(rewardType, out Sprite sprite))
            {
                return sprite;
            }
            return null;
        }
    }
}

