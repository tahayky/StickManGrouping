using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Centurion.CGSHC01.Game
{
    public class CameraControl : MonoBehaviour
    {
        public float camera_damping = 10;
        private void Update()
        {
            Vector3 target = GroupManager.Instance.largest_group_center;
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * camera_damping);
        }
    }
}
