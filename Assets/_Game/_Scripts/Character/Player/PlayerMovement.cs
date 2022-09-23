using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
namespace Centurion.CGSHC01.Game
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Pawn))]
    public class PlayerMovement : CharacterMovement
    {
        Pawn pawn;
        PlayerInput player_input;

        bool seperated_from_group=true;

        Character enemy_target =null;
        new void Awake()
        {
            pawn = GetComponent<Pawn>();
            player_input = GetComponent<PlayerInput>();
            base.Awake();
        }
        private void Start()
        {
            pawn.small_fov.on_trigger_entered += FovEnterUpdate;
            pawn.small_fov.on_trigger_leaved += FovExitUpdate;
        }
        public void FovEnterUpdate(string tag)
        {
            if (tag == "Pawn") GroupManager.Instance.Clustering();
            if (pawn.small_fov.detected_objects.TryGetFirstValue("Enemy", out GameObject enemy))
            {
                enemy_target = enemy.GetComponent<Character>();
            }
            else
            {
                enemy_target = null;
            }
        }
        public void FovExitUpdate(string tag)
        {
            if (tag == "Pawn") GroupManager.Instance.Clustering();
            if (pawn.small_fov.detected_objects.TryGetFirstValue("Enemy", out GameObject enemy))
            {
                enemy_target = enemy.GetComponent<Character>();
            }
            else
            {
                enemy_target = null;
            }
        }
        void Update()
        {
            if (!active) return;

            if (enemy_target != null)
            {
                LookAt(enemy_target.transform);
                enemy_target.Damage(Time.deltaTime * 40);
                MoveForward();
                character.SetMelee(true);
                return;
            }
            else
            {
                character.SetMelee(false);
            }

            float joystick_magnitude = new Vector2(player_input.horizontal, player_input.vertical).magnitude;

            if (joystick_magnitude == 0)
            {
                character.ChangeState(CharacterState.Idle);
                return;
            }

            if (GroupManager.Instance.largest_group != pawn.group && seperated_from_group)
            {
                Vector3 largest_center = GroupManager.Instance.largest_group_center;
                LookAt(largest_center);
                character.ChangeState(CharacterState.Running);
                MoveForward(2);
            }
            else
            {
                Vector3 direction = Vector3.forward * player_input.vertical + Vector3.right * player_input.horizontal;
                LookRotation(direction);
                character.ChangeState(CharacterState.Running);
                //MoveForward();
                Move(new Vector3(player_input.horizontal, 0, player_input.vertical));
            }
            
        }

    }
}
