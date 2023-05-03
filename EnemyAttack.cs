
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    /*This script is shared between 4 different enemies with their own behaviours, sprites and animations.
     * It was done for the project Short on: Affection, which you can see at my portfolio at www.lucastormin.com*/
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;
    [SerializeField] private float distance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float chaseSpeed, attackSpeed;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject attackChargingFX;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private float offsetX, offsetY; //Offsets for enemy position when chasing. Offset X is also attack distance.
    public bool isAttacking, isChasing;
    [SerializeField] private string walkAnimName, attackAnimName, idleAnimName;
    [SerializeField] private float attackStartup, timeBetweenAttacks;
    public bool canAttack = true;
    private bool canChase = true;
    private Vector2 attackTarget;
    public float minPitch, maxPitch; //Pitch between attack sounds
    private bool canDamage;
    [SerializeField] private int scaleModifier; //For bosses.
    [SerializeField] BossesCameraShake bossShake;
    [SerializeField] private AudioSource skeletonVoice;
    [SerializeField] private float voicePitchmin,VoicePithMax;
    [SerializeField] bool isBoss;
    [SerializeField] float skeletonBossAttackSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (enemyHealth.health <= 0 && attackChargingFX.activeInHierarchy)
        {
            attackChargingFX.SetActive(false); //Disables attack charing effect upon death
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (spriteRenderer.isVisible && enemyHealth.health > 0)
        {
            distance = Vector2.Distance(player.position, transform.position);
            if (distance < chaseDistance && !isAttacking && canChase) //Triggers the behavior that chases the player and stops movement in player movement script.
            {
                isChasing = true;
                if (transform.position.x < player.transform.position.x) //Flips the enemy accordingly and moves it towards the player with an offset
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x - offsetX, player.transform.position.y - offsetY), chaseSpeed);
                    if (transform.localScale.x < 0)
                    {
                        transform.localScale = new Vector3(1 * scaleModifier, 1 *scaleModifier, 1*scaleModifier);
                    }
                } else if (transform.position.x > player.transform.position.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x + offsetX, player.transform.position.y - offsetY), chaseSpeed);
                    if (transform.localScale.x > 0)
                    {
                        transform.localScale = new Vector3(-1 * scaleModifier, 1 * scaleModifier, 1 * scaleModifier);
                    }
                }
                anim.Play(walkAnimName);
            }
            if (distance < attackDistance && canAttack)
            {
                isChasing = false;
                canAttack = false;
                canChase = false;
                attackTarget = player.transform.position;
                attackChargingFX.SetActive(true);
                Invoke("Attack", attackStartup); //Invokes function that will move to assigned position and plays attacking animation
            }
            if (isAttacking)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(attackTarget.x, attackTarget.y - offsetY), attackSpeed);
            }
            if (distance > chaseDistance) //Triggers slime behavior to return to their scouting points
            {
                isChasing = false;
            }
        }
    }

    private void Attack()
    {
        if (enemyHealth.health > 0)
        {
            isAttacking = true;
            anim.Play(attackAnimName);
        }
    }
    public void EnableDamage()
    {
        canDamage = true; //Toggles ability to deal damage under specific attacking circumstances. Called on animation event.
    }
    public void EndAttack() //Called on the end of attacking animation. 
    {
        isAttacking = false;
        anim.Play(idleAnimName);
        Invoke("RenableAttack", timeBetweenAttacks); //Creates a cooldown between attacks.
        canDamage = false;
    }
    public void TurnOffAttackEffect() //Called on animation keyframe, turns the attack charging effect off
    {
        attackChargingFX.SetActive(false);
    }

    public void RenableAttack()
    {
        canAttack = true;
        canChase = true;
    }
    public void AttackSound()
    {
        attackSound.pitch = Random.Range(minPitch, maxPitch);
        attackSound.Play();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && enemyHealth.health > 0 && canDamage)
        {
            collision.collider.GetComponent<PlayerHealth>().TakeDamage();
        }
    }

    public void BossShake()
    {
        if (isBoss)
        {
            bossShake.ShakeCamera();
        }
    }
    public void PlaySkeletonVoice()
    {
        skeletonVoice.Play();
        skeletonVoice.pitch = Random.Range(voicePitchmin, VoicePithMax);
    }
    public void slowSkeletonAttackSpeed()
    {
        if (isBoss)
        {
            if (anim.speed == 1)
            {
                anim.speed = skeletonBossAttackSpeed;
            }
            else if (anim.speed != 1)
            {
                anim.speed = 1;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
