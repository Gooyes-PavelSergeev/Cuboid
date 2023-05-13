using Cubic.Audio;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Cubic
{
    public class Cuboid : MonoBehaviour
    {
        [SerializeField] private TextMeshPro[] _texts;
        [SerializeField] private int _power;
        [SerializeField] private ParticleSystem _stackEffect;
        public int Power { get; private set; }
        public int Number { get => (int)Mathf.Pow(2, Power); }
        private Rigidbody _rb;
        private MeshRenderer _mr;
        private BoxCollider _bc;
        public bool AbleToStack { get; private set; } = false;
        [SerializeField] float _speedToStack = 2;
        [SerializeField] float _delayToStackOnSpawn = 0.2f;
        [SerializeField] float _delayToStackOnStack = 0.3f;
        [SerializeField] float _forceOnSpawn = 6f;
        [SerializeField] float _forceOnStack = 60f;
        private Coroutine _ableSetCoroutine;
        public static event Action<Cuboid> SpawnEvent;
        [SerializeField] private bool _builtIn = false;

        private IEnumerator Start()
        {
            if (_builtIn)
            {
                yield return null;
                AbleToStack = true;
                Power = _power;
                _rb = GetComponent<Rigidbody>();
                _mr = GetComponent<MeshRenderer>();
                _bc = GetComponent<BoxCollider>();
                _mr.material.color = CuboidColorData.Instance.GetColor(Power);
                SetTexts();
            }
        }

        public void Init(SpawnConfig config)
        {
            AbleToStack = false;
            Power = config.power;
            _rb = GetComponent<Rigidbody>();
            _mr = GetComponent<MeshRenderer>();
            _bc = GetComponent<BoxCollider>();
            _mr.material.color = CuboidColorData.Instance.GetColor(Power);
            SetTexts();
            transform.SetPositionAndRotation(config.position, config.rotation);
            if (config.spawnSource == SpawnSource.OtherCuboid)
            {
                _rb.AddForce(Vector3.up * _forceOnStack, ForceMode.Impulse);
                _rb.AddForce(config.force, ForceMode.Impulse);
                SetAbleToStack(true, _delayToStackOnStack);
            }
            SpawnEvent?.Invoke(this);
        }

        private void SetAbleToStack(bool able, float delay)
        {
            if (_ableSetCoroutine != null)
            {
                StopCoroutine(_ableSetCoroutine);
            }
            _ableSetCoroutine = StartCoroutine(SetAbleToStackDelay(able, delay));
        }
        private IEnumerator SetAbleToStackDelay(bool able, float delay)
        {
            yield return new WaitForSeconds(delay);
            AbleToStack = able;
            _ableSetCoroutine = null;
        }

        private void Stack(Cuboid other)
        {
            if (!AbleToStack || other.AbleToStack) return;
            Cuboid cuboid = CuboidSpawner.Instance.SpawnCuboid(new SpawnConfig
            {
                position = transform.position,
                rotation = transform.rotation,
                force = new Vector3((_rb.velocity.x + other._rb.velocity.x) / 2, 0, (_rb.velocity.z + other._rb.velocity.z) / 2),
                power = Power + 1,
                spawnSource = SpawnSource.OtherCuboid,
            });
            AudioPlayer.Instance.Play(SoundType.Stack);
            ParticleSystem effect = Instantiate(_stackEffect, cuboid.transform);
            Destroy(effect.gameObject, 0.3f);

            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        public void ThrowFromSpawn()
        {
            _rb.AddForce(transform.forward * _forceOnSpawn, ForceMode.Impulse);
            SetAbleToStack(true, _delayToStackOnSpawn);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Cuboid cuboid))
            {
                if (Power != cuboid.Power || !cuboid.AbleToStack || !AbleToStack) return;
                float selfSpeed = _rb.velocity.magnitude;
                float collSpeed = cuboid._rb.velocity.magnitude;
                if (selfSpeed + collSpeed >= _speedToStack)
                {
                    cuboid.AbleToStack = false;
                    Stack(cuboid);
                }
            }
        }

        public void SetInteractive(bool active)
        {
            _bc.enabled = active;
            _rb.isKinematic = !active;
        }

        private void SetTexts()
        {
            foreach (var text in _texts)
            {
                text.text = Number.ToString();
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            Power = _power;
            SetTexts();
        }
    }
}
