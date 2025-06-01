//using Mirror;
//using Mirror.BouncyCastle.Asn1.X509;
//using UnityEngine;
//using UnityEngine.UI;

//public class Item : MonoBehaviour
//{
//    public ItemData data;
//    public int level;       // 각 아이템의 레벨
//    public Weapon weapon;
//    //public Gear gear;

//    Image icon;
//    Text textLevel;
//    Text textName;
//    Text textDesc;

//    void Awake()
//    {
//        icon = GetComponentsInChildren<Image>()[1];
//        icon.sprite = data.itemIcon;

//        Text[] texts = GetComponentsInChildren<Text>();
//        textLevel = texts[0];
//        textName = texts[1];
//        textDesc = texts[2];
//        textName.text = data.itemName;
//    }

//    void OnEnable()
//    {
//        textLevel.text = "Lv. " + (level + 1);


//        switch (data.itemType)
//        {
//            case ItemData.ItemType.Melee:
//            case ItemData.ItemType.Range:
//                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
//                break;

//            case ItemData.ItemType.Glove:
//            case ItemData.ItemType.Shoe:
//                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
//                break;

//            default:
//                textDesc.text = string.Format(data.itemDesc);
//                break;
//        }
//    }

//    //public void OnClick()
//    //{
//    //    switch (data.itemType)
//    //    {
//    //        case ItemData.ItemType.Melee:
//    //        case ItemData.ItemType.Range:




//    //            if (level == 0)
//    //            {
//    //                //GameObject newWeapon = new GameObject();
//    //                //weapon = newWeapon.AddComponent<Weapon>();

//    //                //weapon.Init(data);



//    //                GameManager.instance.weapon.Init(data);     // gamemanager 수정
//    //            }
//    //            else
//    //            {
//    //                float nextDamage = data.baseDamage;
//    //                int nextCount = 0;

//    //                nextDamage += data.baseDamage * data.damages[level];
//    //                nextCount += data.counts[level];

//    //                GameManager.instance.weapon.LevelUp(nextDamage, nextCount);     // gamemanager 수정
//    //            }





//    //            level++;
//    //            break;

//    //        case ItemData.ItemType.Glove:
//    //        //case ItemData.ItemType.Shoe:
//    //        //    if (level == 0)
//    //        //    {
//    //        //        GameObject newGear = new GameObject();
//    //        //        gear = newGear.AddComponent<Gear>();

//    //        //        gear.Init(data);
//    //        //    }
//    //        //    else
//    //        //    {
//    //        //        float nextRate = data.damages[level];

//    //        //        gear.LevelUp(nextRate);
//    //        //    }

//    //        //    level++;
//    //        //    break;

//    //        case ItemData.ItemType.Heal:
//    //            GameManager.instance.health = GameManager.instance.maxHealth;
//    //            break;
//    //    }


//    //    if (level == data.damages.Length)
//    //    {
//    //        GetComponent<Button>().interactable = false;
//    //    }
//    //}


//    public void OnClick()
//    {
//        switch (data.itemType)
//        {
//            case ItemData.ItemType.Melee:
//            case ItemData.ItemType.Range:

//                if (level == 0)
//                {
//                    // 서버에서 무기 생성 및 Init


//                        GameManager.instance.weapon.Init(data);

//                    // 이 무기는 미리 생성되어 있어야 하며, NetworkServer.Spawn도 서버에서 호출되어야 함
//                }
//                else
//                {
//                    // 서버에게 레벨업 요청

//                        GameManager.instance.weapon.CmdLevelUp(
//                            data.baseDamage + data.baseDamage * data.damages[level],
//                            data.counts[level]);

//                }

//                level++;
//                break;

//            case ItemData.ItemType.Heal:
//                GameManager.instance.health = GameManager.instance.maxHealth;
//                break;
//        }

//        if (level == data.damages.Length)
//        {
//            GetComponent<Button>().interactable = false;
//        }
//    }


//}




using Mirror;
using Mirror.Examples.Common.Controllers.Player;
using UnityEngine;
using UnityEngine.UI;

public class Item : NetworkBehaviour
{
    public ItemData data;
    public Weapon weapon;

    [SyncVar(hook = nameof(OnLevelChanged))]
    public int level;

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
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    void OnEnable()
    {
        UpdateUI();
    }

    void OnLevelChanged(int oldLevel, int newLevel)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (textLevel == null || textDesc == null) return;

        textLevel.text = "Lv. " + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;

            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }

        if (level >= data.damages.Length)
        {
            var btn = GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }
    }

    
    public void OnClick()
    {

        CmdUpgrade();
    }

    
    public void CmdUpgrade()
    {

        if (level >= data.damages.Length)
            return;

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameManager.instance.weapon.Init(data); // 서버에서 무기 초기화
                }
                else
                {
                    float nextDamage = data.baseDamage + data.baseDamage * data.damages[level];
                    int nextCount = data.counts[level];

                    GameManager.instance.weapon.CmdLevelUp(nextDamage, nextCount); // 서버에서 무기 레벨업
                }
                break;

            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        level++;
    }
}
