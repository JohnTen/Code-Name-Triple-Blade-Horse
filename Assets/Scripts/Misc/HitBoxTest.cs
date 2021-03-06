﻿using System.Text;
using TripleBladeHorse.Combat;
using UnityEngine;

namespace TripleBladeHorse.Test
{
    public class HitBoxTest : EnemyHitBox
    {
        [SerializeField] AttackResult returnType;

        public AttackResult ReceiveAttack(ref AttackPackage attack)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ID");
            builder.Append(attack._hashID.ToString());
            builder.Append("\nType");
            builder.Append(attack._attackType.ToString());
            builder.Append("\nHP Damage");
            builder.Append(attack._hitPointDamage.ToString());
            builder.Append("\nEP Damage");
            builder.Append(attack._enduranceDamage.ToString());
            builder.Append("\nIncoming Direction");
            builder.Append(attack._fromDirection.ToString());
            builder.Append("\nKnockback");
            builder.Append(attack._knockback.ToString());

            print(builder);

            return returnType;
        }
    }
}
