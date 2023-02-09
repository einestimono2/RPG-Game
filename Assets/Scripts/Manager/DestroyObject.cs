using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float timeUntilDestroyed = 1.5f;
    
    private void Awake() {
        Destroy(gameObject, timeUntilDestroyed);
    }
}
