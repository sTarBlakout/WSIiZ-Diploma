using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Controls;
using Gameplay.Environment;
using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => _instance;
        private static GameManager _instance;
        
        private List<OrderManagerBase> _turnParticipants = new List<OrderManagerBase>();
        private PawnController _player;
        private CameraManager _cameraManager;
        private GameArea _gameArea;
        
        public PawnController PlayerPawn => _player;

        private void Start()
        {
            _instance = this;
            StartCoroutine(InitGame());
        }

        private IEnumerator InitGame()
        {
            _gameArea = FindObjectOfType<GameArea>();
            _cameraManager = FindObjectOfType<CameraManager>();
            yield return new WaitUntil(() => _gameArea.IsInitialized());

            _turnParticipants.Clear();
            foreach (var pawn in _gameArea.pawns) _turnParticipants.Add(pawn.GetComponent<OrderManagerBase>());
            _player = _gameArea.pawns.First(pawn => pawn.gameObject.CompareTag("Player"));
            StartCoroutine(GameCoroutine());
        }

        private IEnumerator GameCoroutine()
        {
            while (IsGameRunning())
            {
                Debug.Log("NEW TURN");
                foreach (var participant in _turnParticipants)
                {
                    if (!participant.CanTakeTurn()) continue;
                    
                    participant.StartTurn();
                    yield return new WaitWhile(() => participant.IsTakingTurn);
                }
            }
            Debug.Log("Game Over");
        }

        private bool IsGameRunning()
        {
            return _player.IsAlive();
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
