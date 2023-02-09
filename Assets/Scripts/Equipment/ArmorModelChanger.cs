using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorModelChanger : MonoBehaviour
{
    public List<GameObject> armorModels;

    private void Awake() {
        GetAllModels();
    }

    void GetAllModels(){
        for (int i = 0; i < transform.childCount; i++){
            armorModels.Add(transform.GetChild(i).gameObject);
        }
    }

    public void UnEquipAllModels(){
        foreach (GameObject armorModel in armorModels){
            armorModel.SetActive(false);
        }
    } 

    public void EquipModelByName(string modelName){
        if(modelName == ""){
            armorModels[0].SetActive(true);
        }else{
            for (int i = 0; i < armorModels.Count; i++){
                if(armorModels[i].name == modelName){
                    armorModels[i].SetActive(true);
                }
            }
        }
    }
}
