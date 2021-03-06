using Cyberultimate.Unity;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Enemy;
using Game;
using Scoreboard;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : MonoSingleton<Boss>, IEnemy
{
    private bool battle = false;
    private bool dead = false;
    private float health;
    private bool punching = false;

    public float startingHealth = 600;
    public Image healthBar;
    public Canvas BossUI;
    public Transform gunRotate;
    public Transform gunPoint;
    public CharacterController playerCharacter;

    public GameObject bossBeforeFight;
    public GameObject bossFighting;

    public Animator anim;
    private Transform player;

    public string endCutsceneName = "Ending";

    [Header("Pipes")]
    public int pipeOverHealth = 3;
    public float pipeDmg = 150;


    [Header("Shooting")]
    public string bulletPoolTag = "EnemyBullet";
    public int burstSizeMin = 30;
    public int burstSizeMax = 45;
    public float damage = 10f;
    public float burstFireRate = 0.05f;
    public float fireRate = 2f;
    public float dispersion = 40;
    public float disperionY = 20;
    public float bulletSpeed = 300f;

    public bool shootInFront = true;
    public float inFrontMultiplierMin = 0.7f;
    public float inFrontMultiplierMax = 1.3f;

    [Header("Punch")]
    public float punchDamage=20;
    public float punchKnock = 5;
    public float rangeToPunch=5;
    public MeleeAttack attack;

    [SerializeField]
    private AudioSource bossSource;

    public AudioSource BossSource => bossSource;
    
    [SerializeField] private FootstepSoundController damageSounds;
    [SerializeField] private AudioClip headshotSound;


    public void StartBattle()
    {

        health = startingHealth;
        dead = false;
        BossUI.enabled = true;
        healthBar.fillAmount = startingHealth / health;
        EnemySpawner.Current.KillAll();

        bossBeforeFight.SetActive(false);
        bossFighting.SetActive(true);
        battle = true;

        GameMusic.Current.BossMusic();
    }

    private void Start()
    {
        BossUI.enabled = false;
        player = PlayerInstance.Current.transform;
        attack.damage = punchDamage;
        attack.knockback = punchKnock;
        attack.myPos = transform;
        healthToNextPipe = startingHealth - (startingHealth / (pipeOverHealth + 1));
        bossBeforeFight.SetActive(true);
        bossFighting.SetActive(false);
    }

    private int currBurst = 0;
    private float burstTimer = 0;
    private float shootingTimer = 0;
    private bool pipeAnim = false;
    private void Update()
    {
        if (dead || !battle || punching || pipeAnim) return;

        var dist = Vector3.Distance(transform.position, player.position);
        if (dist <= rangeToPunch)
        {
            anim.SetBool("isShooting", false);
            punching = true;
            Punch();
            return;
        }

        var look = playerCharacter.transform.position;
        if(gunRotate) gunRotate.transform.LookAt(look);
        look.y = transform.position.y;
        transform.LookAt(look);

        if (currBurst <= 0)
        {
            anim.SetBool("isShooting", false);
            burstTimer -= Time.deltaTime;
            if (burstTimer <= 0)
            {
                burstTimer = fireRate;
                currBurst = Random.Range(burstSizeMin,burstSizeMax);
                shootingTimer = fireRate;
            }
        } else
        {
            shootingTimer -= Time.deltaTime;
            if (shootingTimer <= 0)
            {
                anim.SetBool("isShooting", true);
                shootingTimer = burstFireRate;
                currBurst--;
                Shoot();
            }
        }
    }

    float healthToNextPipe;
    public void GotHit(float amount)
    {
        if (dead || !battle) return;

        health -= amount;
        if (health <= 0)
        {
            dead = true;
            health = 0;
            healthBar.fillAmount = health / startingHealth;

            anim.SetTrigger("Die");

            try
            {
                _ = GameScoreboard.Current.PostLevelData();
            }
            catch
            {
                // ignore
            }

            // todo end cutscene

            //ObjectivesUI.Current.SetObjective("THE BOSS HAS BEEN DESTROYED", "UGH... NO! THAT'S IMPOSSIBLE.");
            //Invoke(nameof(LoadCredits), 3f);
            Invoke(nameof(LoadCutscene),0.5f);
            
            return;
        }

        while (health < healthToNextPipe)
        {
            healthToNextPipe -= startingHealth / (pipeOverHealth + 1);
            BossPipe.Current.NextPipe();
        }
        healthBar.fillAmount = health/ startingHealth;
    }

    public void PlaySound(bool isHeadshot)
    {
        BossSource.PlayOneShot(isHeadshot ? headshotSound : damageSounds.GetRandomSoundFromRange());
    }

    private void LoadCutscene()
    {
        SceneManager.LoadScene(endCutsceneName);
    }
    
    public void PipeHit()
    {
        pipeAnim = true;
        anim.SetTrigger("Electro");
    }

    public void EndPipe()
    {
        Debug.Log("Pipe broken - boss damaged " + pipeDmg);
        GotHit(pipeDmg);
        pipeAnim = false;
    }

    void Punch()
    {
        var look = playerCharacter.transform.position;
        look.y = transform.position.y;
        transform.LookAt(look);

        burstTimer = fireRate;
        shootingTimer = burstFireRate;
        currBurst = Random.Range(burstSizeMin, burstSizeMax); ;
        anim.SetTrigger("Punch");
        attack.attacking = true;
    }

    public void EndPunching()
    {
        punching = false;
        attack.attacking = false;
    }

    void Shoot()
    {
        GameObject obj = ObjectPooler.Current.SpawnPool(bulletPoolTag, gunPoint.position, Quaternion.identity);

        var vel = playerCharacter.velocity;
        Vector3 inFront = player.position + vel * Random.Range(inFrontMultiplierMin,inFrontMultiplierMax) * Vector3.Distance(transform.position,player.position)/17;

        
        Vector3 dir = ((shootInFront ? inFront : player.position) - gunPoint.position).normalized * bulletSpeed;
        Vector3 dispDir = new Vector3(Random.Range(-dispersion, dispersion), Random.Range(-disperionY, disperionY), Random.Range(-dispersion, dispersion));


        obj.transform.forward = dir+dispDir;
        obj.GetComponent<Rigidbody>().AddForce(dir+dispDir);
        obj.GetComponent<EnemyBullet>().damage = damage;
    }
}
