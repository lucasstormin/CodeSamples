
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    /*This script is shared between 4 different enemies with their own behaviours, sprites and animations.
     * It was done for the project Short on: Affection, which you can see at my portfolio at www.lucastormin.com*/
    public float scoutRadius;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float minScoutTime, maxScoutTime; //Time that stays in idle animation before choosing another location to go to.
    [SerializeField] private Transform scoutLocation;
    [SerializeField] ScoutPoint scoutPoint;
    private float scoutCooldown;
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private AudioSource enemyFootstep;
    [SerializeField] private EnemyAttack enemyAttack;
    public float minFootstepPitch, maxFootstepPitch;
    [SerializeField] private string walkAnimName, idleAnimName;
    [SerializeField] SpriteRenderer sRend;
    [SerializeField] private int scaleModifier;
    private Vector3 iniTransform;

    // Start is called before the first frame update
    void Start()
    {
        scoutLocation.SetParent(null); //The radius around this object will be used to set the enemy scouting range. If he leaves this range (in combat scenarios), will return to it upon disengage.
        scoutPoint.PickScoutPoint();
        iniTransform = transform.position; //Used to reset this object position upon game over
        
    }

    void FixedUpdate()
    {
        if (enemyHealth.health > 0 && !enemyAttack.isChasing && enemyAttack.canAttack) //these booleans conditions ensures a tight behavior between attack and movement.
        {
            scoutCooldown -= Time.deltaTime;
            if (scoutCooldown <= 0) //Chooses a new point to go to after this scoutCooldown interval.
            {
                GoToScoutPoint();
            }
        }
    }
    private void GoToScoutPoint()
    {
        if (enemyHealth.health > 0 && !enemyAttack.isChasing && enemyAttack.canAttack) //these booleans conditions ensures a tight behavior between attack and movement.
        {
            if (transform.position == scoutPoint.scoutPoint)
            {
                scoutCooldown = Random.Range(minScoutTime, maxScoutTime); //Time before picking a new scout point and going towards it.
                scoutPoint.PickScoutPoint(); //Chooses a new Vector2 point inside a sphere radius.
                anim.Play(idleAnimName);
            }
            else
            {
                anim.Play(walkAnimName);
                if(transform.position.x > scoutPoint.scoutPoint.x)
                {
                    transform.localScale = new Vector3(-1 * scaleModifier, 1 * scaleModifier, 1 * scaleModifier);
                } else if (transform.position.x < scoutPoint.scoutPoint.x)
                {
                    transform.localScale = new Vector3(1 * scaleModifier, 1 * scaleModifier, 1* scaleModifier);
                }
                transform.position = Vector2.MoveTowards(transform.position, scoutPoint.scoutPoint, moveSpeed); //Goes to a random point inside the child sphere. Child's parent is set to null, so the radius is static.  
            
            }
        }
    }

    public void playEnemyFootstep()
    {
        if (sRend.isVisible) //Stop sounds from playing if not visible.
        {
            enemyFootstep.pitch = Random.Range(minFootstepPitch, maxFootstepPitch);
            enemyFootstep.Play();
        }
    }

    public void ResetPosition()
    {
        transform.position = iniTransform; //Called on GameOver
    }
  
}
