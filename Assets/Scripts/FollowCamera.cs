using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] GameObject thingToFollow;


    void Start()
    {
        
    }

    void Update()
    {
        CamFollower();
    }

    void CamFollower()
    {
        transform.position = thingToFollow.transform.position;
    }
}
