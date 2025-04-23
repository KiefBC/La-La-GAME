using System;
using Attributes;
using UnityEngine;
using Movement;
using Combat;
using UnityEngine.EventSystems;

namespace Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float autoTargetRange = 8f; // Range to detect enemies
        [SerializeField] LayerMask enemyLayers; // Layer mask for enemies
        [SerializeField] bool autoTargetEnabled = true; // Toggle for auto-targeting
        
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        private float _autoTargetUpdateRate = 0.2f; // How often to check for targets
        private float _lastAutoTargetTime;

        [Serializable]
        public struct CursorMapping
        {
            public CursorState state;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            _mover = GetComponent<Mover>();
            if (_mover == null)
            {
                Debug.LogError($"Missing Mover component on {gameObject.name}");
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

        void Update()
        {
            if (Time.timeScale == 0f) return; // Skip input handling when game is paused
            
            if (InteractWithUI()) return;
            if (_health.IsDead)
            {
                SetCursorState(CursorState.None);
                return;
            }

            if (autoTargetEnabled)
            {
                AutoTargetUpdate();
            }
            
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursorState(CursorState.None);
        }

        private void AutoTargetUpdate()
        {
            // Only update periodically to save performance
            if (Time.time - _lastAutoTargetTime < _autoTargetUpdateRate) return;
            _lastAutoTargetTime = Time.time;

            // If already has a valid target, don't search for new one
            if (_fighter.GetTarget() != null && !_fighter.GetTarget().IsDead) return;

            // Find nearest enemy
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, autoTargetRange, enemyLayers);
            
            float closestDistance = Mathf.Infinity;
            GameObject closestEnemy = null;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == gameObject) continue; // Skip self
                
                Health targetHealth = hitCollider.GetComponent<Health>();
                if (targetHealth == null || targetHealth.IsDead) continue;
                
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance && _fighter.CanAttack(hitCollider.gameObject))
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.gameObject;
                }
            }

            // Attack closest enemy if found
            if (closestEnemy != null)
            {
                _fighter.Attack(closestEnemy);
            }
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRayCastable raycastable = hit.transform.GetComponent<IRayCastable>();
                if (raycastable == null) continue;
                
                if (!raycastable.HandleRaycast(this)) continue;
                SetCursorState(raycastable.GetCursorState());
                return true;
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            // if (EventSystem.current == null) return false;
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            SetCursorState(CursorState.UI);
            return true;
        }

        public void SetCursorState(CursorState state)
        {
            CursorMapping mapping = GetCursorMapping(state);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        
        private CursorMapping GetCursorMapping(CursorState state)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.state == state)
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }

        private bool InteractWithMovement()
        {
            bool hasHit = Physics.Raycast(GetMouseRay(), out var hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(hit.point, 1f);
                }
                SetCursorState(CursorState.Movement);
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}