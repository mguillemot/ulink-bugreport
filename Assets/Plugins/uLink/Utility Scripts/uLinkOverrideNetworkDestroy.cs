// (c)2012 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

[AddComponentMenu("uLink Utilities/Override Network Destroy")]
public class uLinkOverrideNetworkDestroy : uLink.MonoBehaviour
{
	public string broadcastMessage = "uLink_OnNetworkDestroy";
	public bool autoDestroyAfterMessage = true;

	protected void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info)
	{
		// override the instance's NetworkInsantiator Destroyer delegate.
		info.networkView.instantiator.destroyer = Destroyer;
	}

	private void Destroyer(uLink.NetworkView instance)
	{
		Profiler.BeginSample("Destroy: " + networkView.ToPrefabString());

		if (autoDestroyAfterMessage)
		{
			instance.BroadcastMessage(broadcastMessage, SendMessageOptions.DontRequireReceiver);
			Destroy(networkView.gameObject);
		}
		else
		{
			// if we're relying on the message receiver for cleanup, then make sure there is one.
			instance.BroadcastMessage(broadcastMessage, SendMessageOptions.RequireReceiver);
		}

		Profiler.EndSample();
	}
}
