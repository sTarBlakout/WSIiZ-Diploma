using System;
using UnityEngine;

namespace Gameplay.Controls
{
    public class InputManagerUI : MonoBehaviour
    {
        private OrderManagerPlayer _player;

        private void Awake()
        {
            _player = FindObjectOfType<OrderManagerPlayer>();
        }

        public void CompleteTurn()
        {
            _player.CompleteTurn();
        }
    }
}
