using System;
using System.Collections;
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

        public void Init()
        {
            if (_player != null)
            {
                _player.OnTakingTurn -= ProcessPlayerTakingTurn;
                _player.OnTileClicked -= ProcessClickedTile;
                _player.OnPawnClicked -= ProcessClickedPawn;
            }
            
            _player = FindObjectOfType<OrderManagerPlayer>();
            _player.OnTakingTurn += ProcessPlayerTakingTurn;
            _player.OnTileClicked += ProcessClickedTile;
            _player.OnPawnClicked += ProcessClickedPawn;
        }

        #region Unity Events

        private void Start()
        {
            playerTurnView.Hide(true);
            clickedTileView.Hide(true);
            clickedEnemyView.Hide(true);
            clickedInteractableView.Hide(true);
        }

        #endregion

        #region Player Callbacks

        private void ProcessPlayerTakingTurn(bool takingTurn)
        {
            ShowView(takingTurn, playerTurnView);
        }
        
        private void ProcessClickedTile(GameAreaTile tile)
        {
            ShowView(true, clickedTileView);
        }
        
        private void ProcessClickedPawn(IPawn pawn)
        {
            if (pawn.RelationTo(_player.Player) == PawnRelation.Enemy) ShowView(true, clickedEnemyView);
            if (pawn.RelationTo(_player.Player) == PawnRelation.Interactable) ShowView(true, clickedInteractableView);
        }
        
        #endregion

        #region UI Managment

        private void ShowView(bool show, UIView view)
        {
            if (show) view.Show();
            else
            {
                view.Hide();
                _player.inputBlocked = true;
                StartCoroutine(WaitForViewToHide(view));
            }
        }

        private IEnumerator WaitForViewToHide(UIView view)
        {
            yield return new WaitUntil(() => view.IsHidden);
            _player.inputBlocked = false;
        }

        private void HideAll()
        {
            if (clickedTileView.IsVisible) ShowView(false, clickedTileView);
            if (clickedEnemyView.IsVisible) ShowView(false, clickedEnemyView);
            if (clickedInteractableView.IsVisible) ShowView(false, clickedInteractableView);
        }
        
        #endregion

        #region UI Events

        public void ButtonMoveOrder()
        {
            ShowView(false, clickedTileView);
            _player.StartOrder(OrderType.Move);
        }
        
        public void ButtonAttackOrder()
        {
            ShowView(false, clickedEnemyView);
            _player.StartOrder(OrderType.Attack);
        }
        
        public void ButtonInteractOrder()
        {
            ShowView(false, clickedInteractableView);
            _player.StartOrder(OrderType.Interact);
        }

        public void ButtonCancelOrder()
        {
            HideAll();
            _player.ResetOrder();
        }
        
        public void ButtonEndTurn()
        {
            HideAll();
            _player.CompleteTurn();
        }
        
        #endregion
    }
}
