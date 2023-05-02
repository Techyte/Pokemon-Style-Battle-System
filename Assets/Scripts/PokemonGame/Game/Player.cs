namespace PokemonGame.Game
{
    using UnityEngine;

    public class Player : MonoBehaviour
    {
        public static Player Instance;
        
        private Quaternion _target;
        [SerializeField] private bool look;

        public Quaternion targetRot => _target;

        private void Awake()
        {
            Instance = this;
        }

        public void LookAtTarget(Vector3 trainerPos)
        {
            look = true;
            Quaternion currentRotation = transform.rotation;
            transform.LookAt(new Vector3(trainerPos.x, transform.position.y, trainerPos.z));
            _target = transform.rotation;
            transform.rotation = currentRotation;
            _target.x = 0;
        }

        private void FixedUpdate()
        {
            if(look)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _target, 0.04f);
            }

            if (transform.rotation == _target && look)
            {
                look = false;
            }
        }

        public void StopLooking()
        {
            look = false;
            _target = transform.rotation;
        }

        public void SetPosRot(Vector3 position, Quaternion rotation)
        {
            Debug.Log(position);
            transform.position = position;
            Debug.Log(transform.position);
            transform.rotation = rotation;
        }
    }
}