using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        //textName = texts[1];
        //textDesc = texts[2];
        //textName.text = data.itemName;
    }


    void LateUpdate()
    {
        textLevel.text = "Lv. " + (level + 1);
    }


    void OnEnable()
    {
        textLevel.text = "Lv. " + (level + 1);


        //switch (data.itemType)
        //{
        //    case ItemData.ItemType.Melee:
        //    case ItemData.ItemType.Range:
        //        textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
        //        break;

        //    case ItemData.ItemType.Glove:
        //    case ItemData.ItemType.Shoe:
        //        textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
        //        break;

        //    default:
        //        textDesc.text = string.Format(data.itemDesc);
        //        break;
        //}
    }

    public void OnClick()
    {
        if (GameManager.instance.player.statPoints <= 0)
            return;


        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:

                if (level == 0)
                {
                    // 서버에서 무기 생성 및 Init
                    GameManager.instance.weapon.Init(data);
                    // 이 무기는 미리 생성되어 있어야 하며, NetworkServer.Spawn도 서버에서 호출되어야 함



                }
                else
                {
                    // 서버에게 레벨업 요청
                    GameManager.instance.weapon.CmdLevelUp(
                        data.baseDamage + data.baseDamage * data.damages[level],
                        data.counts[level]);

                }

                level++;
                break;



            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();

                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];

                    gear.LevelUp(nextRate);
                }

                level++;

                break;

            case ItemData.ItemType.Heal:
                GameManager.instance.player.health = GameManager.instance.maxHealth;
                break;
        }

        GameManager.instance.player.statPoints--;


        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }


}

