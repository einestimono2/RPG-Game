using UnityEngine;

public class EscortDestinationCollider : MonoBehaviour
{
    private void OnTriggerExit(Collider other) {
        if(other.tag == "NPC"){
            EscortManager em = other.GetComponent<EscortManager>();

            if(em != null){
                em.CompleteEscort();
            }
        }
    }
}
