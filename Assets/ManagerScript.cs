using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public class PlayerIdentity
{
    public Color color;
    public string name;
}

public class ManagerScript : uLink.MonoBehaviour
{

    public Rect Rect = new Rect(300, 0, 250, 250);
    public GameObject PlayerPrefab;
    public GameObject[] Groups;

    public bool isServer { get; private set; }
    public bool isClient { get; private set; }

    void OnGUI()
    {
        GUILayout.BeginArea(Rect);
        if (uLink.Network.status == uLink.NetworkStatus.Disconnected)
        {
            if (GUILayout.Button("Start server"))
            {
                uLink.Network.InitializeServer(32, 7000);
            }
            if (GUILayout.Button("Start client"))
            {
                uLink.Network.Connect("127.0.0.1", 7000);
            }
        }
        else if (uLink.Network.status == uLink.NetworkStatus.Connected)
        {
            if (isClient)
            {
                foreach (PlayerScript playerScript in FindObjectsOfType(typeof(PlayerScript)))
                {
                    if (playerScript.networkView.isMine)
                    {
                        GUILayout.Label("MINE! (" + playerScript.identity.name + ") in " + playerScript.networkView.group);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Goto 1"))
                        {
                            playerScript.RequestChangeGroup(1);
                        }
                        if (GUILayout.Button("Goto 2"))
                        {
                            playerScript.RequestChangeGroup(2);
                        }
                        if (GUILayout.Button("Goto 3"))
                        {
                            playerScript.RequestChangeGroup(3);
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Label(playerScript.networkView.owner + " (" + playerScript.identity.name + " ) in " + playerScript.networkView.group);
                    }
                }
            }
            if (isServer)
            {
                GUILayout.Label("Clients:");
                foreach (PlayerScript playerScript in FindObjectsOfType(typeof(PlayerScript)))
                {
                    GUILayout.Label(playerScript.networkView.owner + " (" + playerScript.identity.name + ") in group " + playerScript.networkView.group);
                }
            }
        }
        GUILayout.EndArea();
    }

    public Vector3 GetRandomPositionForGroup(int group)
    {
        var position = Groups[@group - 1].transform.position;
        position.x += Random.Range(-300f, 300f);
        position.y += Random.Range(-300f, 300f);
        position.z = 0;
        return position;
    }

    #region Server

    private static readonly List<PlayerIdentity> availableIdentities = new List<PlayerIdentity>
                                                                           {
                                                                               new PlayerIdentity { color = Color.blue, name = "Blue" },
                                                                               new PlayerIdentity { color = Color.cyan, name = "Cyan" },
                                                                               new PlayerIdentity { color = Color.gray, name = "Gray" },
                                                                               new PlayerIdentity { color = Color.green, name = "Green" },
                                                                               new PlayerIdentity { color = Color.magenta, name = "Magenta" },
                                                                               new PlayerIdentity { color = Color.white, name = "White" },
                                                                               new PlayerIdentity { color = Color.yellow, name = "Yellow" },
                                                                               new PlayerIdentity { color = Color.black, name = "Black" }
                                                                           };
    void uLink_OnServerInitialized()
    {
        Debug.Log("Server started on port " + uLink.Network.listenPort);
        isServer = true;

        uLink.Network.SetGroupFlags(1, uLink.NetworkGroupFlags.HideGameObjects);
        uLink.Network.SetGroupFlags(2, uLink.NetworkGroupFlags.HideGameObjects);
        uLink.Network.SetGroupFlags(3, uLink.NetworkGroupFlags.HideGameObjects);
    }

    void uLink_OnPlayerApproval(uLink.NetworkPlayerApproval approval)
    {
        approval.Approve();
    }

    void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
    {
        var identity = availableIdentities[0];
        availableIdentities.RemoveAt(0);

        var initialGroup = 2;
        Debug.Log("Instantiating color " + identity.name + " for player " + player + " in group " + initialGroup);

        uLink.Network.Instantiate(player, PlayerPrefab, GetRandomPositionForGroup(initialGroup), Quaternion.identity, initialGroup, identity);
    }

    void uLink_OnPlayerDisconnected(uLink.NetworkPlayer player)
    {
        Debug.Log(player + " disconnecting");
        uLink.Network.DestroyPlayerObjects(player);
    }

    #endregion

    #region Client

    void uLink_OnConnectedToServer(IPEndPoint server)
    {
        Debug.Log("Connected to server on " + server);
        isClient = true;
    }

    #endregion

}
