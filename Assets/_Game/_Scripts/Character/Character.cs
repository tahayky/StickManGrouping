using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Centurion.CGSHC01.Game
{
    public class Character : MonoBehaviour, IDamageable, IDevelopable, ITouchable
    {
        public delegate void OnDamageReceived(float current_health);
        public event OnDamageReceived on_damage;

        public delegate void OnStateChanged(CharacterState current_stat, ITouchable touchable);
        public event OnStateChanged on_state_changed;

        [HideInInspector] public CharacterState character_state = CharacterState.Insensitive;

        [SerializeField] private float start_health;

        public float health { get; set; }
        public int stamina { get; set; }

        private bool melee = false;

        [HideInInspector] public CharacterMovement character_movement;

        public GameObject current_armature { get; private set; }

        public GameObject game_object { get { return this.gameObject; } }

        [SerializeField] private GameObject armature_s4;

        [SerializeField] private GameObject armature_s3;

        [SerializeField] private GameObject armature_s2;

        [SerializeField] private GameObject armature_s1;

        protected void Awake()
        {
            if (GetComponent<CharacterMovement>())
            {
                character_movement = GetComponent<CharacterMovement>();
            }
            SetArmature(armature_s4);
        }
        protected void Start()
        {
            SetHealth(start_health);
            on_damage += delegate
            {
                if (health <= 0)
                {
                    ChangeState(CharacterState.Dead);
                }
            };
        }

        public void SetHealth(float amount)
        {
            health += amount;
            if (on_damage != null)
                on_damage(health);
        }

        public void Damage(float amount)
        {
            SetHealth(-amount);
        }
        public void StaminaUpdate(int amount)
        {
            stamina += amount;
            switch (stamina)
            {
                case <0:
                    SetArmature(armature_s1);
                    break;
                case var expression when (stamina >= 0 && stamina < 10):
                    SetArmature(armature_s2);
                    break;
                case var expression when (stamina >= 10 && stamina < 20):
                    SetArmature(armature_s3);
                    break;
                case var expression when (stamina >= 20 && stamina < 30):
                    SetArmature(armature_s4);
                    break;
            }
        }

        void SetArmature(GameObject armature)
        {
            if (current_armature == armature)
                return;

            armature.SetActive(true);
            current_armature = armature;
            try
            {
                if (armature != armature_s1) armature_s1.SetActive(false);
                if (armature != armature_s2) armature_s2.SetActive(false);
                if (armature != armature_s3) armature_s3.SetActive(false);
                if (armature != armature_s4) armature_s4.SetActive(false);
            }
            catch
            {

            }
        }

        public void ChangeState(CharacterState new_state)
        {
            if (new_state == character_state) return;
            character_state = new_state;
            switch (new_state)
            {
                case CharacterState.Idle:
                    HandleIdle();
                    break;
                case CharacterState.Running:
                    HandleRunning();
                    break;
                case CharacterState.Dead:
                    HandleDead();
                    break;
                case CharacterState.Insensitive:
                    HandleInsensitive();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(new_state), new_state, null);
            }
            if (on_state_changed != null)
                on_state_changed(character_state,this);
        }

        protected virtual void HandleIdle()
        {
            if (character_movement != null)
            {
                character_movement.active = true;
            }
            current_armature.GetComponent<Animator>().SetInteger("Random", UnityEngine.Random.Range(0,4));
            current_armature.GetComponent<Animator>().SetBool("Run", false);
            
        }
        protected virtual void HandleRunning()
        {
            if (character_movement != null)
            {
                character_movement.active = true;
            }
            current_armature.GetComponent<Animator>().SetBool("Run", true);
        }
        protected virtual void HandleDead()
        {
            if (character_movement != null)
            {
                character_movement.active = false;
            }
            current_armature.GetComponent<Animator>().SetTrigger("Die");
            gameObject.GetComponent<CharacterController>().enabled = false;
        }
        protected virtual void HandleInsensitive()
        {
            if (character_movement != null)
            {
                character_movement.active = false;
            }
        }

        public void SetMelee(bool melee)
        {
            if (this.melee == melee) return;
            this.melee = melee;
            current_armature.GetComponent<Animator>().SetBool("Melee", melee);
        }


    }
}
