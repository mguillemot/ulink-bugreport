using UnityEngine;


public class EnemyScript : uLink.MonoBehaviour
{

    public MeshRenderer Model;

    void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info)
    {
        Debug.Log("Instantiated enemy " + networkView.viewID + " in group " + networkView.group);
    }

    void Start()
    {
        if (networkView.isMine) // I'm the server
        {
            InvokeRepeating("ChangeGroup", .5f, .5f);
        }
    }

    private void ChangeGroup()
    {
        var newGroup = (networkView.group % 10) + 1;
        //Debug.Log("Change group of enemy #" + networkView.viewID + " from " + networkView.group + " to " + newGroup);
        var manager = FindObjectOfType(typeof(ManagerScript)) as ManagerScript;
        transform.position = manager.GetRandomPositionForGroup(newGroup);
        networkView.group = newGroup;
    }

}
