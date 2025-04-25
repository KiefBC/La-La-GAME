using System;
using Attributes;
using UnityEngine;
using Movement;
using Combat;
using UnityEngine.EventSystems;

namespace Control
{
    /// <summary>
    /// Handles player input and control, including movement, combat targeting, and cursor management.
    /// Provides auto-targeting functionality for nearby enemies and manages cursor states based on interactions.
    /// </summary>
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

        /// <summary>
        /// Represents a mapping between cursor states and their corresponding textures and hotspots.
        /// </summary>
        [Serializable]
        public struct CursorMapping
        {
            public CursorState state;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        /// <summary>
        /// Initializes required components on awake.
        /// </summary>
        void Awake()
        {
            InitializeComponents();
        }

        /// <summary>
        /// Initializes and validates required components (Mover, Fighter, Health).
        /// Logs errors if any required components are missing.
        /// </summary>
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

        /// <summary>
        /// Handles player input and updates game state each frame.
        /// Processes UI interactions, auto-targeting, movement, and combat.
        /// </summary>
        void Update()
        {
            if (Time.timeScale == 0f) return;
            
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

        /// <summary>
        /// Updates auto-targeting system to find and engage nearest valid enemy target.
        /// Only updates at intervals defined by _autoTargetUpdateRate for performance.
        /// </summary>
        private void AutoTargetUpdate()
        {
            if (Time.time - _lastAutoTargetTime < _autoTargetUpdateRate) return;
            _lastAutoTargetTime = Time.time;

            if (_fighter.GetTarget() != null && !_fighter.GetTarget().IsDead) return;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, autoTargetRange, enemyLayers);
            
            float closestDistance = Mathf.Infinity;
            GameObject closestEnemy = null;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == gameObject) continue;
                
                Health targetHealth = hitCollider.GetComponent<Health>();
                if (targetHealth == null || targetHealth.IsDead) continue;
                
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance && _fighter.CanAttack(hitCollider.gameObject))
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.gameObject;
                }
            }

            if (closestEnemy != null)
            {
                _fighter.Attack(closestEnemy);
            }
        }

        /// <summary>
        /// Handles interaction with raycast-able components in the game world.
        /// </summary>
        /// <returns>True if interaction was handled, false otherwise.</returns>
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

        /// <summary>
        /// Performs a raycast and returns all hits sorted by distance.
        /// </summary>
        /// <returns>Array of RaycastHit objects sorted by distance from closest to farthest.</returns>
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

        /// <summary>
        /// Checks if the mouse is interacting with UI elements.
        /// </summary>
        /// <returns>True if mouse is over UI, false otherwise.</returns>
        private bool InteractWithUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            SetCursorState(CursorState.UI);
            return true;
        }

        /// <summary>
        /// Sets the cursor state and updates cursor appearance.
        /// </summary>
        /// <param name="state">The desired cursor state to set</param>
        public void SetCursorState(CursorState state)
        {
            CursorMapping mapping = GetCursorMapping(state);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        
        /// <summary>
        /// Retrieves the cursor mapping for the specified state.
        /// </summary>
        /// <param name="state">The cursor state to look up</param>
        /// <returns>The corresponding cursor mapping, or the first mapping if not found</returns>
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

        /// <summary>
        /// Handles player movement based on mouse input.
        /// </summary>
        /// <returns>True if movement was initiated, false otherwise.</returns>
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

        /// <summary>
        /// Creates a ray from the camera to the mouse position.
        /// </summary>
        /// <returns>Ray from camera through mouse position</returns>
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}