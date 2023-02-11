using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Pawns;
using Gameplay.Controls;
using Gameplay.Environment;
using Global;
using UnityEngine;

namespace Gameplay.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private List<OrderManagerBase> _turnParticipants = new List<OrderManagerBase>();
        private PawnController _player;
        private CameraManager _cameraManager;
        private InputManagerUI _inputManagerUI;
        private GameArea _gameArea;
        private Coroutine _gameCor;

        public PawnController PlayerPawn => _player;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            _inputManagerUI = FindObjectOfType<InputManagerUI>();
            _cameraManager = FindObjectOfType<CameraManager>();
        }

        private void Start()
        {
            StartCoroutine(InitNextLevel());
        }

        private IEnumerator InitNextLevel()
        {
            if (_gameArea != null) Destroy(_gameArea.gameObject);
            
            var levelList = GlobalManager.Instance.GlobalData.LevelList;
            var currLevelPref = levelList.GetLevel(GlobalManager.Instance.GlobalData.CurrLevel);
            var currLevel = Instantiate(currLevelPref);

            _gameArea = currLevel.GetComponent<GameArea>();
            yield return new WaitUntil(() => _gameArea.IsInitialized());

            _turnParticipants.Clear();
            foreach (var pawn in _gameArea.pawnsGameObjects)
            {
                var orderManager = pawn.GetComponent<OrderManagerBase>();
                if (orderManager != null) _turnParticipants.Add(orderManager);
            }
            _player = _gameArea.pawnsGameObjects.First(pawn => pawn.gameObject.CompareTag("Player")).GetComponent<PawnController>();
            _inputManagerUI.Init();
            
            _gameCor = StartCoroutine(GameCoroutine());
        }

        private IEnumerator GameCoroutine()
        {
            while (_player.IsAlive)
            {
                foreach (var participant in _turnParticipants)
                {
                    if (!participant.CanTakeTurn()) continue;

                    participant.StartTurn();
                    yield return new WaitWhile(() => participant.IsTakingTurn);
                    if (!_player.IsAlive) break;
                }
            }
            
            FinishLevel(false);
        }

        public void FinishLevel(bool win)
        {
            if (win)
            {
                GlobalManager.Instance.GlobalData.CurrLevel++;
                GlobalManager.Instance.GlobalData.SaveGlobalPrefs();
            }
            StopCoroutine(_gameCor);
            StartCoroutine(InitNextLevel());
            if (AdsController.Instance.InterstitialAdLoaded) AdsController.Instance.ShowInterstitialAd();
        }

        #region Uitilities

        public void PlayParticle(ParticleSystem particle, bool activate)
        {
            if (activate) particle.Play();
            else particle.Stop();
        }

        #endregion
    }
}
