using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Heatlth, Stat };
    public InfoType type;

    Text myText;
    Slider mySlider;

    public GameObject targetObject;     // 토글용 오브젝트

    public void ToggleActive()      // 토글 함수 (UI 껐다 켰다)
    {
        if (targetObject != null)
            targetObject.SetActive(!targetObject.activeSelf);
    }

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;

            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level + 1);
                break;

            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;

            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2} : {1:D2}", min, sec);
                break;

            case InfoType.Heatlth:
                float curHealth = GameManager.instance.player.health;
                float maxHealth = GameManager.instance.player.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;

            case InfoType.Stat:
                myText.text = string.Format("{0:F0}P", GameManager.instance.player.statPoints);
                break;
        }
    }
}
