using DevCase.Pooling;
using DevCase.Utility;
using UnityEngine;

namespace DevCase.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private PoolManager _poolManager;
        [SerializeField] private GameSpriteData _gameSpriteData;
        [SerializeField] private SpinManager _spinManager;
        private void Awake()
        {
            _poolManager.Initialize();
            _gameSpriteData.Initialize();
            _spinManager.Initialize();
        }
    }
}
