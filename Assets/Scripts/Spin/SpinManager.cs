using Cysharp.Threading.Tasks;
using DevCase.Pooling;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevCase.Core
{
    public class SpinManager : MonoBehaviour
    {
        [SerializeField] private SpinData _spinData;
        [SerializeField] private GameObject _spinPanel;
        [SerializeField] private GameObject _rewardsPanel;
        [SerializeField] private Transform _spinRotateParent;
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Transform _rewardsParent;
        [SerializeField] private List<Transform> _spinContentParents = new List<Transform>();
        [SerializeField] private SpinRewardItem _spinRewardItemPrefab;
        [SerializeField] private SpinRewardItem _rewardsPanelItemPrefab;
        [SerializeField] private TextMeshProUGUI _spinNameText;
        [SerializeField] private TextMeshProUGUI _spinMultiplierText;
        [SerializeField] private Image _spinWheelImage;
        [SerializeField] private Image _spinIndicatorImage;
        private int _currentSpinIndex = 0;
        private List<SpinRewardItem> _currentRewardItems = new List<SpinRewardItem>();
        private List<SpinRewardItem> _currentRewardsPanelItems = new List<SpinRewardItem>();
        private SpinTypes _currentSpinType = SpinTypes.NormalSpin;
        private SpinWheelTypes _currentSpinWheelType = SpinWheelTypes.BronzeWheel;
        private Tween _spinTween;
        public void Initialize()
        {
            for (int i = 0; i < _spinData.SpinRewardEntries.Length; i++)
            {
                if (i < _spinContentParents.Count)
                {
                    SpinRewardItem newItem = PoolManager.Instance.SpawnObject(_spinRewardItemPrefab);
                    newItem.transform.SetParent(_spinContentParents[i], false);
                    newItem.SetReward(_spinData.SpinRewardEntries[i]);
                    _currentRewardItems.Add(newItem);
                }
            }
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _spinButton.onClick.AddListener(OnSpinButtonClicked);
        }
        private void SetSpinWheelTypeVisuals()
        {
            _spinNameText.text = _spinData.SpinNames[(int)_currentSpinWheelType];
            _spinNameText.color = _spinData.SpinColors[(int)_currentSpinWheelType];
            _spinMultiplierText.color = _spinData.SpinColors[(int)_currentSpinWheelType];
            _spinWheelImage.sprite = _spinData.SpinWheelSprites[(int)_currentSpinWheelType];
            _spinIndicatorImage.sprite = _spinData.SpinIndicatorSprites[(int)_currentSpinWheelType];
        }
        private void OnPlayButtonClicked()
        {
            OpenSpinPanel();
            _playButton.gameObject.SetActive(false);
        }
        private void OpenSpinPanel()
        {
            _currentSpinIndex = 0;
            _currentSpinType = SpinTypes.NormalSpin;
            _currentSpinWheelType = SpinWheelTypes.BronzeWheel;
            SetSpinWheelTypeVisuals();
            _spinPanel.SetActive(true);
            _rewardsPanel.SetActive(true);
            _spinButton.interactable = true;
            CheckAndSetRewardsForSpinType();
        }
        public void CheckAndSetRewardsForSpinType()
        {
            switch (_currentSpinType)
            {
                case SpinTypes.NormalSpin:
                    _currentRewardItems[Random.Range(0, _currentRewardItems.Count)].SwitchToBomb();
                    break;
                case SpinTypes.FreeSpin:
                    break;
            }
        }
        private int GetRandomRewardIndex()
        {
            return Random.Range(0, _currentRewardItems.Count);
        }
        private void OnSpinButtonClicked()
        {
            SpinTheWheel().Forget();
        }
        public async UniTask SpinTheWheel()
        {
            _spinButton.interactable = false;
            _spinTween?.Kill();
            int rewardIndex = GetRandomRewardIndex();
            float anglePerItem = 360f / _currentRewardItems.Count;
            float spinDuration = Random.Range(3f, 5f);
            int extraSpins = Random.Range(2, 5);
            float targetAngle = rewardIndex * anglePerItem;
            float finalAngle = 360f - targetAngle;
            float totalRotation = (extraSpins * 360f) + finalAngle;
            _spinTween = _spinRotateParent
                .DORotate(new Vector3(0, 0, totalRotation), spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic);
            await _spinTween.AsyncWaitForCompletion();
            _spinButton.interactable = true;
        }
    }
}