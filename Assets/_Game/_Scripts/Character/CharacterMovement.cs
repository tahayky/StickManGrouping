using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Centurion.CGSHC01.Game
{
    [RequireComponent(typeof(Character))]
    public class CharacterMovement : MonoBehaviour
    {
        [HideInInspector]public bool active = false;

        public float speed = 1;

        public float damping =1;

        [HideInInspector] public CharacterController character_controller;

        [HideInInspector] public Character character;

        protected void Awake()
        {
            character_controller = GetComponent<CharacterController>();
            character= GetComponent<Character>();
        }
        protected void MoveForward(float factor)
        {
            character_controller.Move(transform.forward * speed* factor * Time.deltaTime);
        }
        protected void MoveForward()
        {
            character_controller.Move(transform.forward * speed * Time.deltaTime);
        }
        protected void Move(Vector3 dir)
        {
            character_controller.Move(dir * speed * Time.deltaTime);
        }
        protected void LookAt(Transform target)
        {
            var lookPos = target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        protected void LookAt(Vector3 target_position)
        {
            var lookPos = target_position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        protected void LookRotation(Vector3 direction)
        {
            var rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }

    }
}
