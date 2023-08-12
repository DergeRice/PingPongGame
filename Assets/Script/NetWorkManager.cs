using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetWorkManager : MonoBehaviourPunCallbacks
{

    public static NetWorkManager instance;

    public PhotonView PV;

    
    private void Awake()
    {
        instance = this;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        //Connect();
        //nConnectedToMaster();

        
    }

    public void ExitCanvas()
    {
        PV.RPC("ExitCanvasToAll", RpcTarget.All);
    }

    [PunRPC]
    void ExitCanvasToAll()
    {

    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnJoinedRoom()
    {
        Debug.Log("JoinedRoom");
        GameManager.instance.ShowCanvas();
    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();
    }

    public override void OnConnected()
    {
        
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("EnterLobby");
        PhotonNetwork.JoinOrCreateRoom("Default", new RoomOptions { MaxPlayers = 2 }, null);
        
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("CreatedRoom");
        GameManager.instance.Master = true;

    }
}
