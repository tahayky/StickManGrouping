using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Centurion.CGSHC01.Game
{
    [RequireComponent(typeof(SphereCollider))]
    public class CharacterFov : MonoBehaviour
    {
        public delegate void OnTriggerEntered(string tag);
        public event OnTriggerEntered on_trigger_entered;

        public delegate void OnTriggerLeaved(string tag);
        public event OnTriggerLeaved on_trigger_leaved;

        public LayerMask raycast_mask;
        public bool enable_raycast = false;
        public Pack<string, GameObject> detected_objects = new Pack<string, GameObject>();
        public GameObject[] obj;
        float radius=0;
        SphereCollider scollider;
        private void Awake()
        {
            scollider = GetComponent<SphereCollider>();
            radius = GetComponent<SphereCollider>().radius;
        }
        public void RemoveDeads(CharacterState character_state,ITouchable touchable)
        {
            if (character_state == CharacterState.Dead)
            {
                detected_objects.Remove(touchable.game_object.tag,touchable.game_object);
            }
            obj = detected_objects.AllValues();
            touchable.game_object.GetComponent<Character>().on_state_changed -= RemoveDeads;
            if (on_trigger_leaved != null)
                on_trigger_leaved(touchable.game_object.tag);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (detected_objects.Contains(other.gameObject)) return;

            if (enable_raycast)
            {
                Vector3 center = scollider.bounds.center;
                RaycastHit hit;
                if (Physics.Raycast(center, other.bounds.center- center, out hit, radius, raycast_mask))
                {
                    if(hit.collider.gameObject == other.gameObject)
                    {
                        detected_objects.Add(other.tag, other.gameObject);
                        obj = detected_objects.AllValues();
                        other.gameObject.GetComponent<Character>().on_state_changed += RemoveDeads;
                        if (on_trigger_entered != null)
                            on_trigger_entered(other.tag);
                        
                    }
                }
                return;
            }

            detected_objects.Add(other.tag, other.gameObject);
            obj = detected_objects.AllValues();
            other.gameObject.GetComponent<Character>().on_state_changed += RemoveDeads;
            if (on_trigger_entered != null)
                on_trigger_entered(other.tag);
        }
        private void OnTriggerExit(Collider other)
        {
            if (detected_objects.Contains(other.gameObject)) 
                detected_objects.Remove(other.tag,other.gameObject);
            obj = detected_objects.AllValues();
            other.gameObject.GetComponent<Character>().on_state_changed -= RemoveDeads;
            if (on_trigger_leaved != null)
                on_trigger_leaved(other.tag);
        }
    }
}
