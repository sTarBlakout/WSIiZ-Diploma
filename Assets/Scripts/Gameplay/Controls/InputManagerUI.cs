using System;
using Doozy.Engine.UI;
using Gameplay.Core;
using Gameplay.Environment;
using UnityEngine;

namespace Gameplay.Controls
{
    public class InputManagerUI : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private UIView playerTurnView;
        [SerializeField] private UIView clickedTileView;

        [Header("Buttons")] 
        [SerializeField] private UIButton endTurnButton;
        
        private OrderManagerPlayer _player;

        #region Unity Events
        
        private void Awake()
        {
            _player = FindObjectOfType<OrderManagerPlayer>();
        }

        private void Start()
        {
            playerTurnView.Hide(true);
            clickedTileView.Hide(true);
        }

        private void OnEnable()
        {
            _player.OnTakingTurn += ProcessPlayerTakingTurn;
            _player.OnTileClicked += ProcessClickedTile;
        }

        private void OnDisable()
        {
            _player.OnTakingTurn -= ProcessPlayerTakingTurn;
            _player.OnTileClicked -= ProcessClickedTile;
        }
        
        #endregion

        #region Player Callbacks

        private void ProcessPlayerTakingTurn(bool takingTurn)
        {
            ShowPlayerTurnView(takingTurn);
        }
        
        private void ProcessClickedTile(GameAreaTile tile)
        {
            ShowClickedTileView(true);
        }
        
        #endregion

        #region UI Managment
        
        private void ShowPlayerTurnView(bool show)
        {
            if (show) playerTurnView.Show();
            else playerTurnView.Hide();
        }
        
        private void ShowClickedTileView(bool show)
        {
            if (show) clickedTileView.Show();
            else clickedTileView.Hide();
        }
        
        #endregion

        #region UI Events

        public void ButtonMoveOrder()
        {
            ShowClickedTileView(false);
            _player.StartOrder(OrderType.Move);
        }

        public void ButtonCancelOrder()
        {
            ShowClickedTileView(false);
            _player.ResetOrder();
        }
        
        public void ButtonEndTurn()
        {
            _player.CompleteTurn();
        }
        
        #endregion
    }
}
