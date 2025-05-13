using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public void CheckRespawn()
    {
        GetComponentInParent<PlayerRespawn>()?.CheckRespawn();
    }
}
