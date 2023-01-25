using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IPawnNormalData : IPawnData
    {
        #region General

        int TeamId { get; }
        int BloodLevel { get; }
        int ActionsPerTurn { get; }
        void ModifyBloodLevelBy(int value);

        #endregion

        #region Audio

        AudioClip HitSound { get; }
        AudioClip DeathSound { get; }

        #endregion
        
        #region Movement

        float RotationSpeed { get; }
        float MovementSpeed { get; }
        float WaitAfterRotate { get; }
        float WaitAfterMove { get; }
        int DistancePerTurn { get; }

        #endregion
        
        #region Attacking

        float AfterDamageDelay { get; }
        int AttackDistance { get; }

        #endregion
        
        #region Prefabs

        GameObject BloodVesselPrefab { get; }
        GameObject WayMoveLinePrefab { get; }
        GameObject WayAttackLinePrefab { get; }
        GameObject WayInteractLinePrefab { get; }

        #endregion
    }
}
