using UnityEngine;

public class FirstPersonFlyCamera : MonoBehaviour
{
    [System.Serializable]
    public class FirstPersonFlyCameraData
    {
        public float maxSpeed = 10.0f;
        public float acceleration = 15.0f;
    }
}