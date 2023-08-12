using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionScript : MonoBehaviour
{
    [SerializeField] GameObject ConnectedBar;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "ball")
        {
            ConnectedBar.GetComponent<BarScript>().Hit();
        }
    }
}
