using UnityEngine;


public class PlayerScript : uLink.MonoBehaviour
{

    public MeshRenderer Model;

    public PlayerIdentity identity { get; private set; }

    private Vector3 rotationAxis;

    void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info)
    {
        identity = info.networkView.initialData.Read<PlayerIdentity>();
        Model.material.color = identity.color;
        Debug.Log("Instantiated player " + identity.name + " owned by #" + networkView.owner);
    }

    void Start()
    {
        rotationAxis = Random.onUnitSphere;
    }

    void Update() 
    {
        if (networkView.isMine)
        {
            Model.transform.RotateAround(rotationAxis, 5f * Time.deltaTime);
        }
    }

    public void RequestChangeGroup(int group)
    {
        networkView.RPC("ChangeGroup", uLink.RPCMode.Server, group);
    }

    [RPC] // SERVER ONLY
    private void ChangeGroup(int group, uLink.NetworkMessageInfo info)
    {
        Debug.Log("Player #" + networkView.owner.id + " changing group from " + networkView.group + " to " + group);

        networkView.RPC("Msg", networkView.owner, "Before group change");
        //uLink.Network.AddPlayerToGroup(networkView.owner, group); // This is the line pat's taking about
        networkView.group = group;
        networkView.RPC("Msg", networkView.owner, "After group change");

        var manager = FindObjectOfType(typeof(ManagerScript)) as ManagerScript;
        var newPosition = manager.GetRandomPositionForGroup(group);
        networkView.RPC("SetPosition", uLink.RPCMode.All, newPosition);
    }

    [RPC]
    private void SetPosition(Vector3 pos, uLink.NetworkMessageInfo info)
    {
        Debug.Log(string.Format("{0} set position to {1}", identity.name, pos));
        transform.position = pos;
    }

    [RPC] // CLIENT ONLY
    private void Msg(string msg, uLink.NetworkMessageInfo info)
    {
        Debug.Log("Server says: " + msg);
    }

}
