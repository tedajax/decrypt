using UnityEngine;

public class FirstPersonUnitCamera : MonoBehaviour
{
    public IUnit attachedUnit;

    void Update()
    {
        if (attachedUnit != null)
        {
            transform.rotation = attachedUnit.HeadTransform.rotation;
            transform.position = attachedUnit.HeadTransform.position;
        }
    }
}