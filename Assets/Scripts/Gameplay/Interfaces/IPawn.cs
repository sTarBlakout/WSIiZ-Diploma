using System;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IPawn
    {
        PawnRelation RelationTo(IPawn pawn);
        
        bool IsBlockingTile { get; }
        Vector3 WorldPosition { get; }
        IPawnData PawnData { get; }

        void SetOnDestroyListener(Action<GameObject> listener);
    }
}
