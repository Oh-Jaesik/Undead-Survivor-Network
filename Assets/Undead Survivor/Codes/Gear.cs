using UnityEngine;
using UnityEngine.TextCore.Text;
using Mirror;

public class Gear : NetworkBehaviour
{
    public ItemData.ItemType type;
    public float rate;
    public int id;

    public override void OnStartLocalPlayer()
    {
        if (id == 2)
            GameManager.instance.gear0 = this;
        if (id == 3)
            GameManager.instance.gear1 = this;
    }

    //public void Init(ItemData data)
    //{
    //    //Basic Set
    //    name = "Gear " + data.itemId;
    //    transform.parent = GameManager.instance.player.transform;
    //    transform.localPosition = Vector3.zero;

    //    //Property Set
    //    type = data.itemType;
    //    rate = data.damages[0];
    //    ApplyGear();
    //}

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    weapon.speed += 500 * rate;
                    break;

                default:
                    weapon.speed -= rate;     // 확인용 코드 수정
                    break;
            }
        }
    }

    void SpeedUp()
    {
        GameManager.instance.player.speed += rate;
    }
}
