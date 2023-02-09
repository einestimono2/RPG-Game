using System.Collections.Generic;
using UnityEngine;

public class BootsModelChanger : MonoBehaviour
{
    public List<GameObject> bootsModels;

    private void Awake() {
        GetAllModels();
    }

    void GetAllModels(){
        for (int i = 0; i < transform.childCount; i++){
            bootsModels.Add(transform.GetChild(i).gameObject);
        }
    }

    public void UnEquipAllModels(){
        foreach (GameObject bootsModel in bootsModels){
            bootsModel.SetActive(false);
        }
    } 

    public void EquipModelByName(string modelName){
        if(modelName == ""){
            bootsModels[0].SetActive(true);
        }else{
            for (int i = 0; i < bootsModels.Count; i++){
                if(bootsModels[i].name == modelName){
                    bootsModels[i].SetActive(true);
                }
            }
        }
    }
}
