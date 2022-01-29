﻿using System;
using Cyberultimate.Unity;
using Scoreboard;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoSingleton<PlayerHealth>
    {
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;
        public float MaxHealth => maxHealth;
        public float Health
        {
            get => health;
            set
            {
                health = Math.Min(value, maxHealth);
                PercentageOverlay.Get(OverlayType.Health).UpdateAmount(1 - (health / maxHealth));

                if (health <= 0) Die();
            }
        }

        private void Die()
        {
            if (PauseManager.Current.IsDead)
            {
                return;
            }

            GameScoreboard.Current.levelData.deaths++;

            PauseManager.Current.SwitchDeath();
        }

        // todo - cool UI
    }
}
