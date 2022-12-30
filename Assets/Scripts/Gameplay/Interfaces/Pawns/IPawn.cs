using System;
using System.Collections.Generic;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IPawn
    {
        void Init();
        PawnRelation RelationTo(IPawn pawn);
        
        bool IsBlockingTile { get; }
        Vector3 WorldPosition { get; }
        IPawnData PawnData { get; }
        Action<GameObject> OnDestroyed { get; set; }
    }
}
