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
        
        public PawnController PlayerPawn => _player;

        private void Awake()
        {
            Instance = this;
            _inputManagerUI = FindObjectOfType<InputManagerUI>();
            _cameraManager = FindObjectOfType<CameraManager>();
        }

        private void Start()
        {
            StartCoroutine(InitGame());
        }

        private IEnumerator InitGame()
        {
            var levelList = GlobalManager.Instance.GlobalData.LevelList;
            var currLevelPref = levelList.GetLevel(0);
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
            
            StartCoroutine(GameCoroutine());
        }

        private IEnumerator GameCoroutine()
        {
            yield return new WaitUntil(_gameArea.IsInitialized);
            
            while (IsGameRunning())
            {
                foreach (var participant in _turnParticipants)
                {
                    if (!participant.CanTakeTurn()) continue;
                    
                    participant.StartTurn();
                    yield return new WaitWhile(() => participant.IsTakingTurn);
                }
            }
        }

        private bool IsGameRunning()
        {
            return _player.IsAlive;
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
