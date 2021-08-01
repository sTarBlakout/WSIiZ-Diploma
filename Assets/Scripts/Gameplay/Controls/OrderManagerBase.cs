using System.Collections.Generic;
using Gameplay;
using Gameplay.Interfaces;
using Gameplay.Сharacters;
using UnityEngine;

public abstract class OrderManagerBase : MonoBehaviour
{
    protected PawnController _pawnController;
    protected GameArea _gameArea;

    protected Order _order;
    protected IInteractable _interactable;
    protected IDamageable _damageable;
    
    private void Awake()
    {
        _pawnController = GetComponent<PawnController>();
        _gameArea = FindObjectOfType<GameArea>();
    }
    
    private void OnReachedDestination()
    {
        switch (_order)
        {
            case Order.Attack: OrderRotate(_damageable.Position); break;
            default: _order = Order.None; break;
        }
    }
    
    private void OnRotated()
    {
        switch (_order)
        {
            case Order.Attack: OrderAttack(_damageable); break;
            default: _order = Order.None; break;
        }
    }
    
    private void OnAttacked()
    {
        Debug.Log("Just Attacked somebody");
    }
            
    protected void OnPathGenerated(List<Vector3> path)
    {
        switch (_order)
        {
            case Order.Move: OrderSimpleMove(path); break;
            case Order.Attack: OrderMoveForAttacking(path); break;
            default: _order = Order.None; break;
        }
    }
    
    private void OrderRotate(Vector3 position)
    {
        _pawnController.RotateTo(position, OnRotated);
    }
    
    private void OrderSimpleMove(List<Vector3> path)
    {
        _pawnController.MovePath(path, OnReachedDestination);
    }
            
    private void OrderMoveForAttacking(List<Vector3> path)
    {
        path.RemoveAt(path.Count - 1);
        _pawnController.MovePath(path, OnReachedDestination);
    }
            
    private void OrderAttack(IDamageable _damageable)
    {
        _pawnController.AttackTarget(_damageable, OnAttacked);
    }
}
