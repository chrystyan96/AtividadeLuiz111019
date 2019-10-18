using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private int heath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeHeath(int value)
    {
        if(heath > 0) 
        {
            heath += value;
        } else 
        {
            Destroy(gameObject);
        }
    }
}
