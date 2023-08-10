using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour
{
    [SerializeField] GameObject BallPosObj;
    public Vector3 BallPos;

    private void Start()
    {
        BallPosObj = transform.GetChild(0).gameObject;
        BallPos = BallPosObj.transform.position;
    }
}
