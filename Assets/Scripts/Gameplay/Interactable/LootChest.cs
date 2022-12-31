using System;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Interfaces;
using Gameplay.Items.Weapons;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Interactable
{
    public class LootChest : MonoBehaviour, IPawnInteractable, IPawnInteractableData
    {
        [SerializeField] private List<GameObject> itemObjects = new List<GameObject>();

        private List<IItem> items = new List<IItem>();
        private GameArea _gameArea;
        private bool _hasLoot = true;

        #region IPawn Implementation

        public void Init()
        {
            _gameArea = FindObjectOfType<GameArea>();
            _gameArea.GetTileInPos(WorldPosition).Enter(this);
            foreach (var itemObj in itemObjects) items.Add(itemObj.GetComponent<IItem>());
        }
        
        public PawnRelation RelationTo(IPawn pawn)
        {
            return _hasLoot ? PawnRelation.Interactable : PawnRelation.Unknown;
        }
        
        public bool IsBlockingTile => true;
        public Vector3 WorldPosition => transform.position;
        public Action<GameObject> OnDestroyed { get; set; }
        public IPawnInteractableData PawnData => this;
        IPawnData IPawn.PawnData => PawnData;


        #endregion
        
        #region IPawnInteractable Implementation

        private IPawnNormal _interactor;

        public void PreInteract(IPawnNormal interactor, Action onPreInteract)
        {
            _interactor = interactor;
            onPreInteract?.Invoke();
        }

        public void Interact(Action onInteract)
        {
            _interactor.Inventory.AddItems(items);
            onInteract?.Invoke();
        }

        public void PostInteract(Action onPostInteract)
        {
            _hasLoot = false;
            onPostInteract?.Invoke();
        }

        #endregion
    }
}
