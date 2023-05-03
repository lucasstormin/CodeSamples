
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{/*This script is shared between 4 different enemies with their own behaviours, sprites and animations.
     * It was done for the project Short on: Affection, which you can see at my portfolio at www.lucastormin.com*/
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private int dropChance;
    private bool instantiated;
    // Start is called before the first frame update    
    public void InstantiateHeart() //Called on animation event
    {
        int dropRate = Random.Range(0, 100);
        if (dropRate < dropChance && !instantiated) //will instantiate the drop.
        {
            GameObject heartClone = Instantiate(heartPrefab, transform.position, transform.rotation);
            heartClone.GetComponent<HealthPickup>().MoveHeart();
            instantiated = true;
        }
    }
}
