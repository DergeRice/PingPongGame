using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool DebugMode;
    public bool Master;
    [SerializeField] GameObject MasterCanvas,ClientCanvas,CommonCanvas, Ball;

    public bool ImBlue;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    public void ShowCanvas()
    {
        if (Master) MasterCanvas.SetActive(true);
        else ClientCanvas.SetActive(true);
    }

    public void SetMyPlayerBlue(bool AmIblue)
    {
        if (AmIblue) ImBlue = true;
        else ImBlue = false;

        NetWorkManager.instance.ExitCanvas();

    }


    public void EndSetting()
    {
        CommonCanvas.SetActive(true);
        if (ImBlue) CommonCanvas.transform.GetChild(0).GetComponent<Text>().text = "Blue";
        else CommonCanvas.transform.GetChild(0).GetComponent<Text>().text = "Red";
    }

    public void SetBallPos(GameObject Target)
    {
        Ball.transform.position =  Target.GetComponent<BarScript>().BallPos;
    }

    public void ResetGame()
    {

    }
}
