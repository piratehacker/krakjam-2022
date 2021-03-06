using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Enemy;
using Game;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    public Animator anim;
    private EnemyAI ai;

    public bool dead = false;
    public bool gotHit = false;

    public float startingHealth = 100f;
    private float hp = 100;
    [SerializeField] private GameObject myHead = null;
    [SerializeField] private GameObject myBody = null;

    [SerializeField]
    private AudioSource enemySource = null;
    public AudioSource EnemySource => enemySource;

    [SerializeField] private FootstepSoundController damageSounds;
    [SerializeField] private AudioClip headshotSound;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");

    private void Start()
    {
        ai = GetComponent<EnemyAI>();
        hp = startingHealth;
    }

    bool firstHpChange = true;
    
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Damage = Animator.StringToHash("Damage");

    public void HPrefresh()
    {
        if (!firstHpChange) return;
        firstHpChange = true;
        hp = startingHealth;
    }

    public void GotHit(float amount)
    {
        if (dead) return;

        hp -= amount;
        ai.agent.SetDestination(transform.position);
        if (hp <= 0)
        {
            Scoreboard.GameScoreboard.Current.levelData.kills++;
            LevelManager.Current.Score += LevelManager.Current.killScore;

            hp = 0;
            dead = true;
            myHead.tag = "Untagged";
            myBody.tag = "Untagged";

            anim.SetBool(IsWalking, false);
            anim.SetBool(IsRunning, false);

            anim.SetTrigger(Die);

            // myBody.GetComponent<Collider>().isTrigger = true;
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = false;
            }

            return;
        }

        gotHit = true;
        anim.SetTrigger(Damage);
    }

    public void PlaySound(bool isHeadshot)
    {
        EnemySource.PlayOneShot(isHeadshot ? headshotSound : damageSounds.GetRandomSoundFromRange());
    }

    public void HitEnd()
    {
        gotHit = false;
    }
}
