using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Layouts;
using Doozy.Engine.UI;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Gameplay.Controls
{
    public class InputManagerUI : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private UIView playerTurnView;
        [SerializeField] private UIView inventoryView;
        [SerializeField] private UIView clickedTileView;
        [SerializeField] private UIView clickedEnemyView;
        [SerializeField] private UIView clickedInteractableView;

        [Header("Buttons")] 
        [SerializeField] private UIButton inventoryButton;
        
        [Header("Prefabs")] 
        [SerializeField] private UIButton inventorySlotPrefab;
        [SerializeField] private GameObject selectedSlotFrame;

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

            if (_player.GetItems(ItemType.Weapon).Count == 0)
            {
                inventoryButton.gameObject.SetActive(false);
                StartCoroutine(EnableInventoryButtonWhenCan());
            }
        }

        private IEnumerator EnableInventoryButtonWhenCan()
        {
            while (_player.GetItems(ItemType.Weapon).Count == 0)
            {
                Debug.Log(_player.GetItems(ItemType.Weapon).Count);
                yield return new WaitForSeconds(1);
            }
            
            inventoryButton.gameObject.SetActive(true);
        }

        #region Unity Events

        private void Awake()
        {
            playerTurnView.Hide(true);
            inventoryView.Hide(true);
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
            if (show)
            {
                if (view.IsHiding)
                    StartCoroutine(WaitForViewToHide(view, () => view.Show()));
                else
                    view.Show();
            }
            else
            {
                view.Hide();
                _player.inputBlocked = true;
                StartCoroutine(WaitForViewToHide(view, () => _player.inputBlocked = false));
            }
        }

        private IEnumerator WaitForViewToHide(UIView view, Action onViewHidden)
        {
            yield return new WaitUntil(() => view.IsHidden);
            onViewHidden?.Invoke();
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

        public void ButtonOpenInventory()
        {
            ShowView(false, playerTurnView);
            ShowView(true, inventoryView);
            StartCoroutine(InitInventoryPanel());
        }
        
        public void ButtonInventoryAccept()
        {
            ShowView(false, inventoryView);
            ShowView(true, playerTurnView);
        }
        
        #endregion
        
        #region Inventory Panel Managment
        
        private GameObject _selectedSlotFrame;

        private IEnumerator InitInventoryPanel()
        {
            var radialMenu = inventoryView.GetComponentInChildren<RadialLayout>();
            _selectedSlotFrame = Instantiate(selectedSlotFrame, inventoryView.transform, false);

            foreach (Transform child in radialMenu.transform) 
            {
                Destroy(child.gameObject);
            }
            
            var items = _player.GetItems(ItemType.Weapon);
            foreach (var item in items)
            {
                var slot = Instantiate(inventorySlotPrefab, radialMenu.transform, false);
                slot.transform.Find("Icon").GetComponent<Image>().sprite = item.Item1.ItemData.ItemIcon;
                slot.GetComponent<Button>().onClick.AddListener(() => ProcessItemClick(item.Item1, slot.gameObject));

                if (!item.Item2) continue;
                yield return new WaitForEndOfFrame();
                ProcessItemClick(item.Item1, slot.gameObject, true);
            }
        }

        private void ProcessItemClick(IItem item, GameObject slot, bool justUpdateSelected = false)
        {
            _selectedSlotFrame.transform.localPosition = slot.transform.localPosition;
            if (!justUpdateSelected) _player.EquipItem(item);
        }
        
        #endregion 
    }
}
