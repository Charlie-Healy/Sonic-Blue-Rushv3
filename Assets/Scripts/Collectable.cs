using UnityEngine;

public class Collectable : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    public int value;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            source.PlayOneShot(clip);
            Destroy(gameObject);
            RingCollecter.instance.IncreaseRings(value);
            
        }
    }
}
