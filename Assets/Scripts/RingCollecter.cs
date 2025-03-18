using UnityEngine;
using TMPro;

public class RingCollecter : MonoBehaviour
{

    public static RingCollecter instance;

    public TMP_Text ringText;
    public int currentRings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        ringText.text = "" + currentRings.ToString();
    }

    public void IncreaseRings(int v)
    {
        currentRings += v;
        ringText.text = "" + currentRings.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
