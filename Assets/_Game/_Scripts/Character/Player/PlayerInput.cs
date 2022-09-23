using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Centurion.CGSHC01.Game
{
    public class PlayerInput : MonoBehaviour
    {
        public float vertical {
            get 
            { 
                return floating_joystick.Vertical; 
            }
        }
        public float horizontal
        {
            get
            {
                return floating_joystick.Horizontal;
            }
        }

        [SerializeField] FloatingJoystick floating_joystick;
    }
}
