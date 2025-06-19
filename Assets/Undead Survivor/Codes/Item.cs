using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using System.Collections;
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

        // 플레이어별 고유 능력 구현!
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:


                switch (GameManager.instance.playerId)
                {
                    case 2:
                        GameManager.instance.weapon.CmdLevelUp(data.damages[level] + 10, data.counts[level]);     // 데미지 증가
                        break;

                    case 3:
                        GameManager.instance.weapon.CmdLevelUp(data.damages[level], data.counts[level] + 1);      // 카운트 증가
                        break;

                    default:
                        GameManager.instance.weapon.CmdLevelUp(data.damages[level], data.counts[level]);
                        break;
                }
                level++;

                break;

            // Melee와 동일!
            case ItemData.ItemType.Range:

                switch (GameManager.instance.playerId)
                {
                    case 2:
                        GameManager.instance.weapon1.CmdLevelUp(data.damages[level], data.counts[level]);
                        break;

                    case 3:
                        GameManager.instance.weapon1.CmdLevelUp(data.damages[level], data.counts[level] + 1);
                        break;

                    default:
                        GameManager.instance.weapon1.CmdLevelUp(data.damages[level], data.counts[level]);
                        break;
                }
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

                GameManager.instance.player.maxHealth += data.damages[level];       // Heal 레벨업시 이제 최대 체력 증가!
                GameManager.instance.player.health = GameManager.instance.player.maxHealth;
                level++;
                break;
        }

        GameManager.instance.player.statPoints--;       // 레벨업 버튼 클릭시 스탯 감소

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}