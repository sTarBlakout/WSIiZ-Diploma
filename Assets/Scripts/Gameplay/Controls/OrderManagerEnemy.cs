using System;

namespace Gameplay.Controls
{
    public class OrderManagerEnemy : OrderManagerBase
    {
        private void Update()
        {
            if (isTakingTurn) CompleteTurn();
        }
    }
}
