using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class CompleteOrderArgsComplex : CompleteOrderArgsBase
    {
        public readonly List<CompleteOrderArgsBase> CompleteOrderArgsList = new List<CompleteOrderArgsBase>();
    }
}
