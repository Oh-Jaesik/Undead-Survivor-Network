using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;

    Image icon;
    Text textLevel;
    //Text textName;
    //Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
    }


    void LateUpdate()
    {
        textLevel.text = "Lv. " + level;
    }

    public void OnClick()
    {
        if (GameManager.instance.player.statPoints <= 0)
            return;

        // ScriptObj의 damage, count 값을 그대로 읽어오도록 수정
        // 기존의 weapon,gear Init함수 삭제 (일관성을 위해)
        // weapon 0, 1의 경우 직접 bullet을 생성해서 동기화 해야하므로 Cmd 함수 (동기화용) 사용
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:

                GameManager.instance.weapon.CmdLevelUp(data.damages[level], data.counts[level]);
                level++;

                break;

            case ItemData.ItemType.Range:

                GameManager.instance.weapon1.CmdLevelUp(data.damages[level], data.counts[level]);
                level++;

                break;

            case ItemData.ItemType.Glove:

                GameManager.instance.gear0.LevelUp(data.damages[level]);
                level++;

                break;

            case ItemData.ItemType.Shoe:

                GameManager.instance.gear1.LevelUp(data.damages[level]);
                level++;

                break;

            case ItemData.ItemType.Heal:

                GameManager.instance.player.health = GameManager.instance.player.maxHealth;

                break;
        }

        GameManager.instance.player.statPoints--;       // 레벨업 버튼 클릭시 스탯 감소


        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}