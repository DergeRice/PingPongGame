using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BarScript : MonoBehaviour
{
    [SerializeField] GameObject BallPosObj;
    public Vector3 BallPos;

    public PhotonView PV;
    public bool Movable;

    private void Awake()
    {
        Movable = PV.IsMine ? true :false;
    }
    private void Start()
    {
        BallPosObj = transform.GetChild(0).gameObject;
        BallPos = BallPosObj.transform.position;
    }

    private void Update()
    {
        
    }
}
