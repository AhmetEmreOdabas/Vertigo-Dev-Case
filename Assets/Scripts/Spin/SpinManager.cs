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
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _safeLeaveButton;
        [SerializeField] private GameObject _leaveButtonConfirmationParent;
        [SerializeField] private Button _leaveButtonConfirm;
        [SerializeField] private Button _leaveButtonCancel;
        [SerializeField] private GameObject _bombFailScreen;
        [SerializeField] private Button _bombFailScreenCloseButton;
        [SerializeField] private Transform _rewardsParent;
        [SerializeField] private List<Transform> _spinContentParents = new List<Transform>();
        [SerializeField] private SpinRewardItem _spinRewardItemPrefab;
        [SerializeField] private SpinRewardItem _rewardsPanelItemPrefab;
        [SerializeField] private TextMeshProUGUI _spinNameText;
        [SerializeField] private TextMeshProUGUI _spinMultiplierText;
        [SerializeField] private Image _spinWheelImage;
        [SerializeField] private Image _spinIndicatorImage;
        private int _currentSpinIndex = 0;
        private int _currentRewardMultiplier = 1;
        private List<SpinRewardItem> _currentRewardItems = new List<SpinRewardItem>();
        private List<SpinRewardItem> _currentRewardsPanelItems = new List<SpinRewardItem>();
        private SpinTypes _currentSpinType = SpinTypes.NormalSpin;
        private SpinWheelTypes _currentSpinWheelType = SpinWheelTypes.BronzeWheel;
        private Tween _spinTween;
        public void Initialize()
        {
            for (int i = 0; i < _spinData.SpinRewardEntries.Count; i++)
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
            _leaveButton.onClick.AddListener(OnLeaveButtonClicked);
            _leaveButtonConfirm.onClick.AddListener(OnLeaveButtonConfirmed);
            _leaveButtonCancel.onClick.AddListener(OnLeaveButtonCancelled);
            _safeLeaveButton.onClick.AddListener(OnSafeLeaveButtonClicked);
            _bombFailScreenCloseButton.onClick.AddListener(BombFailScreenCloseClicked);
            _playButton.gameObject.SetActive(true);
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
            _currentSpinIndex = 1;
            _currentSpinType = SpinTypes.NormalSpin;
            _currentSpinWheelType = SpinWheelTypes.BronzeWheel;
            _spinPanel.SetActive(true);
            _rewardsPanel.SetActive(true);
            _spinButton.interactable = true;
            _leaveButtonConfirmationParent.SetActive(false);
            CheckAndSetupWheelForSpinType();
        }
        public void CheckAndSetupWheelForSpinType()
        {
            if (_currentSpinIndex % _spinData.GoldZoneSpinCount == 0)
            {
                _currentSpinWheelType = SpinWheelTypes.GoldWheel;
                _currentSpinType = SpinTypes.FreeSpin;
            }
            else if (_currentSpinIndex % _spinData.SilverZoneSpinCount == 0)
            {
                _currentSpinType = SpinTypes.FreeSpin;
                _currentSpinWheelType = SpinWheelTypes.SilverWheel;
            }
            else
            {
                _currentSpinType = SpinTypes.NormalSpin;
                _currentSpinWheelType = SpinWheelTypes.BronzeWheel;
            }
            for (int i = 0; i < _currentRewardItems.Count; i++)
            {
                _currentRewardItems[i].SwitchToReward();
            }
            _spinMultiplierText.text = $"Up To x{_currentRewardMultiplier} Rewards";
            switch (_currentSpinType)
            {
                case SpinTypes.NormalSpin:
                    _currentRewardItems[Random.Range(0, _currentRewardItems.Count)].SwitchToBomb();
                    _safeLeaveButton.gameObject.SetActive(false);
                    _leaveButton.gameObject.SetActive(true);
                    _safeLeaveButton.interactable = false;
                    _leaveButton.interactable = true;
                    break;
                case SpinTypes.FreeSpin:
                    _safeLeaveButton.gameObject.SetActive(true);
                    _leaveButton.gameObject.SetActive(false);
                    _safeLeaveButton.interactable = true;
                    _leaveButton.interactable = false;
                    break;
            }
            SetSpinWheelTypeVisuals();
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
            _safeLeaveButton.interactable = false;
            _leaveButton.interactable = false;
            _spinTween?.Kill();
            _spinRotateParent.localEulerAngles = Vector3.zero;
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
            await RegisterReward(_currentRewardItems[rewardIndex]);
            _currentSpinIndex++;
            _currentRewardMultiplier++;
            if(_currentRewardItems[rewardIndex].IsBomb)
            {
                return;
            }
            CheckAndSetupWheelForSpinType();
            _spinButton.interactable = true;
            _safeLeaveButton.interactable = true;
            _leaveButton.interactable = true;
        }
        public async UniTask RegisterReward(SpinRewardItem spinRewardItem)
        {
            if (spinRewardItem.IsBomb)
            {
                _spinButton.interactable = false;
                _safeLeaveButton.interactable = false;
                _leaveButton.interactable = false;
                _bombFailScreen.SetActive(true);
                return;
            }
            SpinRewardItem rewardItem = _currentRewardsPanelItems.Find(item => item.RewardType == spinRewardItem.RewardType);
            if (rewardItem == null)
            {
                SpinRewardItem newItem = PoolManager.Instance.SpawnObject(_rewardsPanelItemPrefab);
                newItem.transform.SetParent(_rewardsParent, false);
                newItem.SetReward(spinRewardItem.SpinRewardEntry);
                newItem.transform.localScale = Vector3.one;
                int rewardAmount = (spinRewardItem.RewardAmount * _currentRewardMultiplier) - spinRewardItem.RewardAmount;
                _currentRewardsPanelItems.Add(newItem);
                await newItem.AddRewardAmount(rewardAmount);
                return;
            }
            else if (rewardItem != null)
            {
                int rewardAmount = spinRewardItem.RewardAmount * _currentRewardMultiplier;
                await rewardItem.AddRewardAmount(rewardAmount);
                return;
            }
        }
        public void ResetSpin()
        {
            _currentSpinIndex = 1;
            _currentRewardMultiplier = 1;
            _currentSpinType = SpinTypes.NormalSpin;
            _currentSpinWheelType = SpinWheelTypes.BronzeWheel;
            SetSpinWheelTypeVisuals();
            foreach (var item in _currentRewardItems)
            {
                item.SwitchToReward();
            }
            foreach (var item in _currentRewardsPanelItems)
            {
                PoolManager.Instance.DespawnObject(item.gameObject);
            }
            _currentRewardsPanelItems.Clear();
            _playButton.gameObject.SetActive(true);
            _spinPanel.SetActive(false);
            _rewardsPanel.SetActive(false);
        }

        private void OnSafeLeaveButtonClicked()
        {
            //Reward player with current rewards
            ResetSpin();
        }

        private void OnLeaveButtonCancelled()
        {
            _leaveButtonConfirmationParent.gameObject.SetActive(false);
        }

        private void OnLeaveButtonConfirmed()
        {
            _leaveButtonConfirmationParent.gameObject.SetActive(false);
            ResetSpin();
        }

        private void OnLeaveButtonClicked()
        {
            _leaveButtonConfirmationParent.gameObject.SetActive(true);
        }
        private void BombFailScreenCloseClicked()
        {
            _bombFailScreen.SetActive(false);
            ResetSpin();
        }
    }
}