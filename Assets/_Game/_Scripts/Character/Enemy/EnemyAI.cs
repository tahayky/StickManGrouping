using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Centurion.CGSHC01.Game
{
    [RequireComponent(typeof(Character))]
    public class EnemyAI : CharacterMovement
    {
        Enemy enemy;
        private new void Awake()
        {
            enemy = GetComponent<Enemy>();
            base.Awake();
        }
        void Update()
        {
            if (!active) return;

            if (enemy.large_fov.detected_objects.TryGetFirstValue("Pawn", out GameObject large_area_go))
            {

                if (enemy.small_fov.detected_objects.TryGetFirstValue("Pawn", out GameObject small_area_go))
                {
                    //small_area_gos[0].GetComponent<Character>().Damage(Time.deltaTime * character.stamina);
                    LookAt(small_area_go.transform);
                    character.ChangeState(CharacterState.Running);
                    character.SetMelee(true);

                }
                else
                {
                    LookAt(large_area_go.transform);
                    character.ChangeState(CharacterState.Running);
                    MoveForward();
                    character.SetMelee(false);
                }
            }
            
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!hit.gameObject.CompareTag("Enemy")) return;

            CharacterController cc = hit.collider.GetComponent<CharacterController>();

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            cc.Move(pushDir * Time.deltaTime * 10);
        }

    }
}
