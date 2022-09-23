using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Centurion.CGSHC01.Game
{
    public class Friend : Character
    {

        IEnumerator stop_running;
        new void Start()
        {
            ChangeState(CharacterState.Idle);
            base.Start();
        }

        protected override void HandleIdle()
        {
            if (character_movement != null)
            {
                character_movement.active = true;
            }
            stop_running = StopRunning();
            StartCoroutine(stop_running);
        }
        protected override void HandleRunning()
        {
            if (character_movement != null)
            {
                character_movement.active = true;
            }
            if(stop_running!=null) StopCoroutine(stop_running);
            current_armature.GetComponent<Animator>().SetInteger("Random", UnityEngine.Random.Range(0, 4));
            current_armature.GetComponent<Animator>().SetBool("Run", true);
        }
        IEnumerator StopRunning()
        {
            yield return new WaitForSeconds(0.1f);
            current_armature.GetComponent<Animator>().SetBool("Run", false);
        }
    }
}
