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
        switch (id)
        {
            case 2:
                GameManager.instance.gear0 = this;
                break;

            case 3:
                GameManager.instance.gear1 = this;
                break;
        }
    }

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
                    weapon.speed = 100 + 300 * rate;        // 근접무기 회전속도 증가
                    break;

                case 1:
                    // 원거리무기 기본쿨(speed) 1000 (즉 쿨타임이 매우 길어 발사 안함)
                    if (GameManager.instance.weapon1.speed < 10)        // 원거리무기 레벨 1이상인지 확인, 원거리무기 레벨 0에서 공속업시 원거리무기 작동 방지
                        weapon.speed = 1.2f - rate;        // 원거리무기 발사 쿨 감소, 즉 공속업
                    break;
            }
        }
    }

    void SpeedUp()
    {
        GameManager.instance.player.speed += rate;
    }
}