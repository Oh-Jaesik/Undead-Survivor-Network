using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    //public Weapon weapon;
    //public Gear gear;

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
    }


    void LateUpdate()
    {
        textLevel.text = "Lv. " + level;
    }

    public void OnClick()
    {
        if (GameManager.instance.player.statPoints <= 0)
            return;


        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:


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


            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    // 서버에서 무기 생성 및 Init
                    GameManager.instance.weapon1.Init(data);
                    // 이 무기는 미리 생성되어 있어야 하며, NetworkServer.Spawn도 서버에서 호출되어야 함

                    //GameManager.instance.weapon1.speed /= 1000;


                }
                else
                {
                    // 서버에게 레벨업 요청
                    GameManager.instance.weapon1.CmdLevelUp(data.baseDamage + data.baseDamage * data.damages[level], data.counts[level]);

                    //GameManager.instance.weapon1.speed /= 2;

                }

                level++;
                break;

            case ItemData.ItemType.Glove:

                GameManager.instance.gear0.LevelUp(data.damages[level]);
                level++;
                break;

            case ItemData.ItemType.Shoe:
                //if (level == 0)
                //{
                //    GameObject newGear = new GameObject();
                //    gear = newGear.AddComponent<Gear>();

                //    gear.Init(data);
                //}
                
                {
     

                    GameManager.instance.gear1.LevelUp(data.damages[level]);
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

