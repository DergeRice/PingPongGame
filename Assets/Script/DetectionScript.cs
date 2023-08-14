using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DetectionScript : MonoBehaviour
{
    [SerializeField] GameObject ConnectedBar;

    //private void  (Collision collision)
    //{
    //    if(collision.transform.tag == "Ball")
    //    {
    //        ConnectedBar.GetComponent<BarScript>().PV.RPC("Hit",RpcTarget.All);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            ConnectedBar.GetComponent<BarScript>().PV.RPC("Hit", RpcTarget.All);
        }
    }
}
