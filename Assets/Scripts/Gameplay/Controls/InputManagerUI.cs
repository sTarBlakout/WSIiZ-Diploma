using System;
using Doozy.Engine.UI;
using UnityEngine;

namespace Gameplay.Controls
{
    public class InputManagerUI : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private UIView playerTurnView;

        [Header("Buttons")] 
        [SerializeField] private UIButton endTurnButton;
        
        private OrderManagerPlayer _player;

        private void Awake()
        {
            _player = FindObjectOfType<OrderManagerPlayer>();
        }

        private void Start()
        {
            playerTurnView.Hide(true);
        }

        private void OnEnable()
        {
            _player.OnTakingTurn += ShowPlayerTurnView;
        }

        private void OnDisable()
        {
            _player.OnTakingTurn -= ShowPlayerTurnView;
        }

        private void ShowPlayerTurnView(bool show)
        {
            if (show) playerTurnView.Show();
            else playerTurnView.Hide();
        }

        public void CompleteTurn()
        {
            _player.CompleteTurn();
        }
    }
}
