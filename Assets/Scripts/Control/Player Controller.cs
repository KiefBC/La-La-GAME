using Attributes;
using UnityEngine;
using Movement;
using Combat;

namespace Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CursorMapping[] cursorMappings = null;
        
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        
        public enum CursorState
        {
            Combat,
            Movement,
            None
        }

        [System.Serializable]
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
            if (_health.IsDead) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            SetCursorState(CursorState.None);
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;
                
                if (!_fighter.CanAttack(target.gameObject)) continue;
                
                if (Input.GetMouseButton(0))
                {
                    _fighter.Attack(target.gameObject);
                }
                SetCursorState(CursorState.Combat);
                return true;
            }
            return false;
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