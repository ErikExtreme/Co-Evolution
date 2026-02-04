using UnityEngine;

public class ShipHealth : MonoBehaviour
{
    //Stats
    int hullHP;
    int armor;
    int shieldCapacity;
    int shieldRegen;
    int powerCapacity;
    int powerRegen;

    //Current
    private int health;
    private int shield;
    private int power;


    void Start()
    {
        health = hullHP;
        shield = shieldCapacity;
        power = powerCapacity;
    }

    void Update()
    {


        if (shield < shieldCapacity)
            shield += shieldRegen;

        if (power < powerCapacity)
            power += powerRegen;
    }

    public void TakeDamage(int damage)
    {
        //Shield damage
        if (shield >= damage)
        {
            shield -= damage;
            return;
        }

        damage -= shield;
        damage -= armor;
        shield = 0;

        //Hull damage
        if (damage > 0)
            health -= damage;
        else
            health -= 1;

        if (health <= 0)
            Debug.Log("RAN OUT OF HEALTH");//lose in some way
    }
    public void RegainHealth(int regainAmount)
    {
        health += regainAmount;
        if (health > hullHP)
            health = hullHP;
    }
}
