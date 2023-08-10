using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool DebugMode;
    [SerializeField] GameObject MasterCanvas, Ball;

    public bool ImBlue;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    public void SetMyPlayerBlue(bool AmIblue)
    {
        if (AmIblue) ImBlue = true;
        else ImBlue = false;

        MasterCanvas.SetActive(false);
    }

    public void SetBallPos(GameObject Target)
    {
        Ball.transform.position =  Target.GetComponent<BarScript>().BallPos;
    }

    public void ResetGame()
    {

    }
}
