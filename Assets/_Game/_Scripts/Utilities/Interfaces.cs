using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Centurion.CGSHC01.Game
{
    public interface IDamageable
    {
        float health { get; set; }

        void Damage(float amount);

    }
    public interface IDevelopable
    {
        int stamina { get; set; }

        void StaminaUpdate(int amount);

    }
    public interface ITouchable
    {
        GameObject game_object { get; }

    }
}