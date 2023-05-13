using Gooyes.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cubic
{
    public class Thrower : Singleton<Thrower>
    {
        private Cuboid _current;

        private Vector3 _throwPos;

        [SerializeField] private ThrowLine _throwLine;

        private Plane _plane;

        [SerializeField] private float _switchCooldown = 0.5f;

        [SerializeField] private LayerMask _UIMask;

        private void Start()
        {
            _plane = new Plane(Vector3.up, _throwLine.transform.position);
            _throwPos = _throwLine.Center;
            SetNewCuboid(_switchCooldown);
        }

        private void Update()
        {
            if (_current == null) return;
            if (Input.GetMouseButton(0))
            {
                if (CheckPointerOverUI()) return;
                _throwPos = GetPosOnLine(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (CheckPointerOverUI()) return;
                Throw();
            }
        }

        private void FixedUpdate()
        {
            if (_current == null) return;
            _current.transform.position = _throwPos;
        }

        private void SetNewCuboid(float delay)
        {
            StartCoroutine(SetCuboidWithDelay(delay));
        }

        private IEnumerator SetCuboidWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnConfig spawnConfig = new SpawnConfig
            {
                position = _throwLine.Clamp(Vector3.zero),
                rotation = Quaternion.identity,
                spawnSource = SpawnSource.Sequence,
                power = Random.Range(1, 3)
            };
            _current = CuboidSpawner.Instance.SpawnCuboid(spawnConfig);
            _current.SetInteractive(false);
        }

        private void Throw()
        {
            _current.SetInteractive(true);
            _current.ThrowFromSpawn();
            _current = null;
            SetNewCuboid(_switchCooldown);
        }

        private Vector3 GetPosOnLine(Vector3 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (_plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                return _throwLine.Clamp(hitPoint);
            }
            Debug.LogWarning("Not found point");
            return Vector3.zero;
        }

        private bool CheckPointerOverUI()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            foreach (RaycastResult raycastResult in raycastResults)
            {
                if (1 << raycastResult.gameObject.layer == _UIMask.value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
