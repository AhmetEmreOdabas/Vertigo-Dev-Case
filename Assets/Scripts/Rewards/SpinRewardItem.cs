using Cysharp.Threading.Tasks;
using DevCase.Utility;
using DG.Tweening;
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
        private bool _isBomb = false;
        private SpinRewardEntry _currentRewardEntry;
        public RewardTypes RewardType => _currentRewardEntry.Type;
        public bool IsBomb => _isBomb;
        public int RewardAmount => _rewardAmount;
        public SpinRewardEntry SpinRewardEntry => _currentRewardEntry;
        private Tween _rewardValueTween;
        private Tween _rewardPunchTween;
        public void SetReward(SpinRewardEntry rewardEntry)
        {
            _currentRewardEntry = rewardEntry;
            SwitchToReward();
        }
        public void SwitchToBomb()
        {
            _rewardIcon.sprite = GameSpriteData.Instance.GetSpinRewardSprite(RewardTypes.Bomb);
            _rewardAmountText.text = "Bomb";
            _isBomb = true;
        }
        public void SwitchToReward()
        {
            _rewardIcon.sprite = GameSpriteData.Instance.GetSpinRewardSprite(_currentRewardEntry.Type);
            _rewardAmountText.text = "x" + StringFormatter.ToShortString(_currentRewardEntry.Amount);
            _rewardAmount = _currentRewardEntry.Amount;
            _isBomb = false;
        }
        public async UniTask AddRewardAmount(int amount)
        {
            int oldAmount = _rewardAmount;
             _rewardValueTween?.Kill(true);
            _rewardAmount += amount;
            _rewardValueTween = DOTween.To(() => oldAmount, x =>
                {
                    _rewardAmountText.text = "x" + StringFormatter.ToShortString(x);
                }, _rewardAmount, 0.5f).SetEase(Ease.OutCubic);
            _rewardAmountText.text = "x" + StringFormatter.ToShortString(_rewardAmount);
            await _rewardValueTween.AsyncWaitForCompletion();
            _rewardPunchTween?.Kill(true);
            _rewardPunchTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetEase(Ease.InSine);
            await _rewardPunchTween.AsyncWaitForCompletion();
        }
    }
}
