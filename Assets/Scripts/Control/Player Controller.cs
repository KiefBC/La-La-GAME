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
        
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;

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
            if (InteractWithUI()) return;
            if (_health.IsDead)
            {
                SetCursorState(CursorState.None);
                return;
            };
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursorState(CursorState.None);
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

        private void SetCursorState(CursorState state)
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