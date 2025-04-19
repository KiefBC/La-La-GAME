using System;
using Combat;
using UnityEngine;
using Core;

namespace Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float attackDistance = 2f;
        private GameObject _player;
        private Fighter _fighter;
        private Health _health;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            if (_player == null)
            {
                Debug.LogError($"Missing Player object in the scene");
            }
            
            _fighter = GetComponent<Fighter>();
            if (_fighter == null)
            {
                Debug.LogError($"Missing Fighter component on {gameObject.name}");
            }
            
            _health = GetComponent<Health>();
            if (_health == null)
            {
                Debug.LogError($"Missing Health component on {gameObject.name}");
            }
        }

        private void Update()
        {
            if (_health.IsDead) return;
            
            if (InAttackRangeOfPlayer() && _fighter.CanAttack(_player))
            {
                _fighter.Attack(_player);
            }
            else
            {
                _fighter.Cancel();
            }
        }
        
        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
            return distanceToPlayer < attackDistance;
        }
    }
}
