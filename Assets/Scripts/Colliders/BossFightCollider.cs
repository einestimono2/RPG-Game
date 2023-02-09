using UnityEngine;

public class BossFightCollider : MonoBehaviour
{
    public GameObject[] walls;

    private void Awake() {
        foreach (var wall in walls) {
            wall.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if(playerStats != null){
                EventManager.BossFight(this, playerStats);
            }
        }
    }
}
