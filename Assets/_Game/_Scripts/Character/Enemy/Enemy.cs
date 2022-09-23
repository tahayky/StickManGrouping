using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Centurion.CGSHC01.Game
{
    public class Enemy : Character
    {
        public CharacterFov large_fov;
        public CharacterFov small_fov;
        new void Start()
        {
            ChangeState(CharacterState.Idle);
            base.Start();
        }
    }
}
