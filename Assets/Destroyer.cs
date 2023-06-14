using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    public void DestroyTarget()
    {
        Destroy(target);
    }
}
