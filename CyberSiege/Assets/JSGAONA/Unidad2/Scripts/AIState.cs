using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.JSGAONA.Unidad2.Scripts
{
    public abstract class AIState : ScriptableObject
    {
        public EnemyAI EnemyAI { get; set; }

        public abstract void Initialize(EnemyAI enemyAI);

        public abstract void Enter();

        public abstract void Execute();
        public abstract void Exit();
  
    }
}