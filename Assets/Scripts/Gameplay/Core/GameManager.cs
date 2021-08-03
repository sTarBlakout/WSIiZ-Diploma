using System.Collections;
using System.Collections.Generic;
using Gameplay.Controls;
using UnityEngine;

namespace Gameplay.Core
{
    public class GameManager : MonoBehaviour
    {
        private List<OrderManagerBase> _turnParticipants = new List<OrderManagerBase>();
        private GameArea _gameArea;

        private void Start()
        {
            StartCoroutine(InitGame());
        }

        private IEnumerator InitGame()
        {
            _gameArea = FindObjectOfType<GameArea>();
            yield return new WaitUntil(() => _gameArea.IsInitialized());
            
            _turnParticipants.Clear();
            foreach (var pawn in _gameArea.pawns) _turnParticipants.Add(pawn.GetComponent<OrderManagerBase>());
            StartCoroutine(GameCoroutine());
        }

        private IEnumerator GameCoroutine()
        {
            while (CheckIsGameRunning())
            {
                foreach (var participant in _turnParticipants)
                {
                    participant.StartTurn();
                    yield return new WaitWhile(() => participant.IsTakingTurn);
                }
            }
        }

        private bool CheckIsGameRunning()
        {
            return true;
        }
    }
}
