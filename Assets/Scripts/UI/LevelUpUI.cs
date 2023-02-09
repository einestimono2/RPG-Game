using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUp : MonoBehaviour
{
    [Header("Level")]
    public int currentLevel;
    public int projectedLevel;
    public TMP_Text currentLevelText;
    public TMP_Text projectedLevelText;

    [Header("EXP")]
    public int currentEXP;
    public int projectedEXP;
    public TMP_Text currentEXPText;
    public TMP_Text projectedEXPText;

    [Header("Health")]
    public Slider healthSlider;
    public TMP_Text currentHealthText;
    public TMP_Text projectedHealthText;

    void OnEnable(){
        
    }
}
