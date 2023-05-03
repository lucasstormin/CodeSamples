using Dads.Actors;
using UnityEngine;

namespace Dads.Enemies
{
    public class Slammer : MonoBehaviour
    {
        //References
        [SerializeField] private Animator anim; //Animator of this gameobject
        [SerializeField] private GameObject slamAoe; //Shockwave AoE Prefab to be instantiated 
        [SerializeField] private Sprint sprint; //Sprint script that gives speed boost to enemies
        [SerializeField] private SlammerShockwave slamShockwaveScript; 
        private Actor actor;
        private Rigidbody rb;
        //Animation strings
        [SerializeField] private string SlamAnim, walkAnim, getUpAnim;
        //Behavioural values
        [SerializeField] private float distanceToAttack;
        [SerializeField] private float aoeDuration;
        [SerializeField] private float aoeSize;
        [SerializeField] private float slamCooldown;
        public int aoeDamage;
        //Conditions and lockers
        private bool isSlamming, canSlam;
        private float distance;

        // Start is called before the first frame update
        void Start()
        {
            canSlam = true;
            rb = GetComponent<Rigidbody>();
            actor = GetComponent<Actor>();
        }

        private void Update()
        {
            distance = Vector3.Distance(transform.position, actor.CurrentTarget.transform.position); //Finds distance between Enemy and player
            if (distance <= distanceToAttack && canSlam) //If that distance is smaller than attack distance and can perform the Slam attack, execute following code
            {
                anim.Play(SlamAnim); //Plays the slam animation. Can be replaced by triggers in the animator, for a game that demands more performance.
                rb.constraints = RigidbodyConstraints.FreezePositionY; //Prevents Y axis from getting messed up during the Slam animation
                Vector3 dir = actor.CurrentTarget.transform.position - transform.position; //Gets the direction from current position to actor
                isSlamming = true; //Sets conditions and lockers to prevent bugs
                canSlam = false;
            }
        }

        public void InstantiateSlam() 
        {
            GameObject slamClone = Instantiate(slamAoe, slamAoe.transform.position, slamAoe.transform.rotation); //Instantiates a prefab responsible for the shockwave attack
            slamClone.transform.localScale = new Vector3(aoeSize, aoeSize, aoeSize);//Sets the size of the shockwave attack according to designer values in inspector
            slamClone.transform.SetParent(null); //Sets the shockwave parent to null
            slamClone.SetActive(true); //Acitavtes the shockwave
            HaltMovement(); //Stops all movement to prevent bugs
            Destroy(slamClone, aoeDuration); //Destroys the shockwave effect after aoeDuration 
            rb.constraints = RigidbodyConstraints.FreezeAll; //Freezes all axis to prevent bugs caused by animation bones
        }


        private void AllowSlam()
        {
            canSlam = true; //Can attack again
        }

        public void ApplyForce()
        {
            sprint.isSprinting = true; //Used as a speed boost, called by an event in the animation itself
        }

        public void HaltMovement() //Stops all movement, also can be called by an event in the animation itself
        {
            sprint.isSprinting = false;
            sprint.StopSprint();
            actor.Physics.CanMove = false;
        }

        public void AllowMovement() //Allows all movement, also can be called by an event in the animation itself
        {
            anim.Play(walkAnim); //Plays walking animation. Can be replaced by animator triggers if game needs more performance.
            actor.Physics.CanMove = true;
            Invoke(nameof(AllowSlam), slamCooldown); //Allows enemy to attack again, after slamCooldown time, set by designers in inspector.
            rb.constraints = RigidbodyConstraints.None; //Allows enemy to move freely once again.
        }

        public void GetUp() //Gets up, called by an event on the animation itself
        {
            anim.Play(getUpAnim); //Plays getting up animation. Can be replaced by animator triggers if game needs more performance.
            Invoke( nameof(AllowMovement), 5.5f); //Allows moving again after a stun delay after perfoming his shockwave attack. This could be exposed in the inspector to give designers more freedom, however this value was fine during playtests.
        }
    }
}
