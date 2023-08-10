using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] bool DebugMode;
    GameManager gameManager;
    [SerializeField] const int PowerRangeMin = 20;
    [SerializeField] const int PowerRangeMax = 30;

    Rigidbody rigid;
    // Start is called before the first frame update

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
       if(gameManager.DebugMode) DebugModeAction();
    }

    void DebugModeAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            rigid.AddForce(new Vector3(Random.Range(PowerRangeMin, PowerRangeMax), Random.Range(PowerRangeMin, PowerRangeMax), Random.Range(PowerRangeMin, PowerRangeMax)),ForceMode.VelocityChange);
        }
    }
}
