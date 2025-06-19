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
                    weapon.speed = 100 + 300 * rate;        // �������� ȸ���ӵ� ����
                    break;

                case 1:
                    // ���Ÿ����� �⺻��(speed) 1000 (�� ��Ÿ���� �ſ� ��� �߻� ����)
                    if (GameManager.instance.weapon1.speed < 10)        // ���Ÿ����� ���� 1�̻����� Ȯ��, ���Ÿ����� ���� 0���� ���Ӿ��� ���Ÿ����� �۵� ����
                        weapon.speed = 1.2f - rate;        // ���Ÿ����� �߻� �� ����, �� ���Ӿ�
                    break;
            }
        }
    }

    void SpeedUp()
    {
        GameManager.instance.player.speed += rate;
    }
}