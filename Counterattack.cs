
using UnityEngine;
using UnityEngine.VFX;

public class Counterattack : MonoBehaviour
{
    /*This is a script that allows players to grab a projectile, triggered by button press. After grabbing, players can either release the grab button to throw the projectile away immediately 
     * or keep holding the button until a  timer expires, automatically throwing the projectile away upon expiration
     Projectiles can be countered multiple times by multiple players, and every time that happens, its speed is increased*/
    [SerializeField] private GameObject playerParent;
    [SerializeField] private Transform counterPoint;
    [SerializeField] private ShakeCaller camShake;
    [Header("Duration & Timing")]
    [SerializeField, Tooltip("How many seconds is the counter active for")] private float counterWindow;
    [SerializeField, Tooltip("How many seconds player can hold the bomberang before it releases automatically")] private float holdTimeLimit;
    private float iniWindow;
    private float iniHoldTime;
    [Header ("Force & Modifier")]
    [SerializeField] private float counterForce;
    [SerializeField, Tooltip("How much extra force in % will be applied for each counter. 0.2 means 20% per counter")] private float counterModifier; //Added to the boomerang itself, on its script.
    private bool caughtABomberang;
    private Boomerang bomberang;
    private Rigidbody bomberangRb;
    private Countertrigger countertrigger;
    private Controller controller;

    [Header ("Sounds")]
    public FMODUnity.EventReference ReleaseSound;
    [Header("VFX")]
    [SerializeField] private GameObject successCounterVFX;
    [SerializeField] private VisualEffect counterHoldVFX;

    // Start is called before the first frame update
    private void Start()
    {
        countertrigger = GetComponentInParent<Countertrigger>();
        controller = GetComponentInParent<Controller>();
        iniWindow = counterWindow;
        iniHoldTime = holdTimeLimit;
    }
    // Update is called once per frame
    void Update()
    {
        if (caughtABomberang) //means countered successfully
        {
            if (bomberangRb != null)
            {
                bomberangRb.velocity = new Vector3(0, 0, 0); //Stops boomerang from moving
                bomberang.transform.position = counterPoint.transform.position; //Adjusts boomerang to its counter point
            }
            if (controller.isHolding == false) //Prevents scenario where players would just tap counter and still hold the boomerang
            {
                Throw();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!caughtABomberang) //Means missed a counter
        {
            counterWindow -= Time.deltaTime; 
            if (counterWindow <= 0) //How long the counter window will last. How long the counter mesh and triggers are active. 
            {
                DisableCounter(); 
            }
        } else if (caughtABomberang) //Means got a counter
        {
            holdTimeLimit -= Time.deltaTime; //How long players can hold it before throwing automatically
            if (holdTimeLimit <= 0)
            {
                Throw();
            }
        }
    }
    void DisableCounter() //Disables the counter visuals and behaviours
    {
        counterWindow = iniWindow;
        holdTimeLimit = iniHoldTime;
        gameObject.SetActive(false);
    } 
   
    
    public void Throw() //Called on button release or when holding time limit is over
    {
        if (caughtABomberang) //Only works if countered successsfully
        {
            caughtABomberang = false;
            counterHoldVFX.Stop();
            camShake.ShakeCamera();
            float force = counterForce * bomberang.counterModifer; //Multiplies counter force by a modifier
            if (bomberangRb != null) 
            { 
                bomberangRb.velocity = transform.forward.normalized * force; //Sets the counter boomerang velocity
                bomberang.currentPlayer = playerParent; //Sets the new target to return to and owner of the boomerang.
               bomberang.AllowExplosion(); //Allos boomerang to explode
                GameObject vfxClone = Instantiate(successCounterVFX, successCounterVFX.transform.position, successCounterVFX.transform.rotation);
                vfxClone.SetActive(true);
                vfxClone.transform.SetParent(null);
                bomberang.CounterTrailVFXActivate();
            }
            countertrigger.ResetCooldown(); //resets cooldown on successful counter
            FMODUnity.RuntimeManager.PlayOneShot(ReleaseSound);
            DisableCounter(); //Disables counter after throwing
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bomberang") && !caughtABomberang) 
        {
            if (other.GetComponent<Boomerang>().counterImmunityTime <= 0) //stops from countering boomerang when throwing it right away
            {
                counterHoldVFX.Play();
                camShake.ShakeCamera();
                bomberang = other.GetComponent<Boomerang>();
                bomberang.dangerRang = true;
                bomberang.BlockExplosion(); //Prevents boomerang from exploding in your hand
                bomberangRb = other.GetComponent<Rigidbody>();
                bomberang.counterModifer += counterModifier; //Increases the modifier to boost speed even further every counter
                bomberang.ResetMasterFuse(); //Resets master fuse when countering
                caughtABomberang = true;
            }
        }
    }
}
