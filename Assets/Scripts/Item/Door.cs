using UnityEngine;

public class Door : Interactable
{
    public bool openOnStart;
    public bool isOpen;
    Animator anim;

    public string keyID = "";

    private void Awake() {
        anim = GetComponent<Animator>();

        if(openOnStart){
            anim.SetLayerWeight(1, 1);
        }
    }

    public override void Interact(PlayerInteract playerInteract){
        base.Interact(playerInteract);
        
        if(isOpen) CloseDoor();
        else OpenDoor();
    }

    public void OpenDoor(){
        isOpen = true;

        anim.SetBool("isOpen", isOpen);
    }

    public void CloseDoor(){
        isOpen = false;

        anim.SetBool("isOpen", isOpen);
    }
}
