using System;
using Doozy.Engine.UI;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Controls
{
    public class InputManagerUI : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private UIView playerTurnView;
        [SerializeField] private UIView clickedTileView;
        [SerializeField] private UIView clickedEnemyView;
        [SerializeField] private UIView clickedInteractableView;

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
            clickedEnemyView.Hide(true);
            clickedInteractableView.Hide(true);
        }

        private void OnEnable()
        {
            _player.OnTakingTurn += ProcessPlayerTakingTurn;
            _player.OnTileClicked += ProcessClickedTile;
            _player.OnPawnClicked += ProcessClickedPawn;
        }

        private void OnDisable()
        {
            _player.OnTakingTurn -= ProcessPlayerTakingTurn;
            _player.OnTileClicked -= ProcessClickedTile;
            _player.OnPawnClicked -= ProcessClickedPawn;
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
        
        private void ProcessClickedPawn(IPawn pawn)
        {
            if (pawn.RelationTo(_player.Player) == PawnRelation.Enemy) ShowClickedEnemyView(true);
            if (pawn.RelationTo(_player.Player) == PawnRelation.Interactable) ShowClickedInteractableView(true);
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
        
        private void ShowClickedEnemyView(bool show)
        {
            if (show) clickedEnemyView.Show();
            else clickedEnemyView.Hide();
        }

        private void ShowClickedInteractableView(bool show)
        {
            if (show) clickedInteractableView.Show();
            else clickedInteractableView.Hide();
        }
        
        #endregion

        #region UI Events

        public void ButtonMoveOrder()
        {
            ShowClickedTileView(false);
            _player.StartOrder(OrderType.Move);
        }
        
        public void ButtonAttackOrder()
        {
            ShowClickedEnemyView(false);
            _player.StartOrder(OrderType.Attack);
        }
        
        public void ButtonInteractOrder()
        {
            ShowClickedInteractableView(false);
            _player.StartOrder(OrderType.Interact);
        }

        public void ButtonCancelOrder()
        {
            ShowClickedTileView(false);
            ShowClickedEnemyView(false);
            ShowClickedInteractableView(false);
            _player.ResetOrder();
        }
        
        public void ButtonEndTurn()
        {
            ShowClickedTileView(false);
            ShowClickedEnemyView(false);
            _player.CompleteTurn();
        }
        
        #endregion
    }
}
