using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Centurion.CGSHC01.Game
{
    [RequireComponent(typeof(CharacterFov))]
    [RequireComponent(typeof(Character))]
    public class FriendAI : CharacterMovement
    {
        public Transform target;

        CharacterFov character_fov;

        [HideInInspector] public bool chained_to_player = false;

        private new void Awake()
        {
            character_fov = GetComponent<CharacterFov>();
            base.Awake();
        }

        void Update()
        {
            if (!active) return;
            /*
            if (player.GetComponent<CharacterFov>().GetFovWithTag("LargeArea", out FOV large_fov) && large_fov.detected_objects.TryGetFirstValue("Enemy", out GameObject enemy))
            {
                target = enemy.transform;
                chained_to_player = false;
                if (character_fov.GetFovWithTag("SmallArea", out FOV small_fov) && small_fov.detected_objects.TryGetFirstValue("Enemy", out GameObject enemy_go))
                {
                    character.SetMelee(true);
                    enemy_go.GetComponent<Character>().Damage(Time.deltaTime*40);
                }
            }
            else
            {
                character.SetMelee(false);
                target = player.transform;
                if (Vector3.Distance(player.transform.position, transform.position) < 1.5f)
                {
                    chained_to_player = true;
                    character.ChangeState(CharacterState.Idle);
                    return;
                }
                chained_to_player = false;
                if (character_fov.GetFovWithTag("SmallArea", out FOV small_fov) && small_fov.detected_objects.TryGetAllValues("Player", out List<GameObject> friend_gos))
                {
                    for (int i = 0; i < friend_gos.Count; i++)
                    {
                        if (friend_gos[i].GetComponent<FriendAI>()&&friend_gos[i].GetComponent<FriendAI>().chained_to_player)
                        {
                            character.ChangeState(CharacterState.Idle);
                            return;
                        }
                    }
                }
            }

            Vector3 direction = target.position - transform.position;
            LookRotation(direction);

            character.ChangeState(CharacterState.Running);
            MoveForward();

            */
        }
    }
}
