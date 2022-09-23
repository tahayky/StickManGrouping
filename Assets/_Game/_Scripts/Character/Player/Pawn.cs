using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Centurion.CGSHC01.Game
{
    public class Pawn : Character
    {
        public PawnGroup group;

        public CharacterFov small_fov;

        public Pawn[] detected_pawns { 
            get {
                small_fov.detected_objects.TryGetAllValues("Pawn",out GameObject[] pawn_gos);
                if(pawn_gos==null) return new Pawn[0];
                return pawn_gos.Select(item => item.GetComponent<Pawn>()).ToArray();
            } }

        new void Start()
        {
            Physics.IgnoreCollision(small_fov.GetComponent<Collider>(), gameObject.GetComponent<CharacterController>());
            ChangeState(CharacterState.Idle);
            base.Start();
        }


    }
}
