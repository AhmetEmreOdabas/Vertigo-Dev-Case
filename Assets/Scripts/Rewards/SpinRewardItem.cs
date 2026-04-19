using DevCase.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevCase.Core
{
    public class SpinRewardItem : MonoBehaviour
    {
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmountText;
        private int _rewardAmount;
        public int RewardAmount => _rewardAmount;
        public void SetReward(SpinRewardEntry rewardEntry)
        {
            _rewardIcon.sprite = GameSpriteData.Instance.GetSpinRewardSprite(rewardEntry.Type);
            _rewardAmountText.text = "x" + StringFormatter.ToShortString(rewardEntry.Amount);
            _rewardAmount = rewardEntry.Amount;
        }
    }
}
