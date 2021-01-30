using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{

    public List<int> skills = new List<int>();//create the list

    public int Health = 0;
    public int Fortitude = 0;
    public int Spirit = 0;
    public int minValue = 0;
    public int maxValue = 999;

   

    public void addSkills(int Health,int Fortitude,int Spirit)
    {
        skills.Add(Health);
        skills.Add(Fortitude);
        skills.Add(Spirit);

    }



    public int healthIncrease(int Health, int amount)
    {
        if (Health > maxValue)//sets health to 999 incase of getting above set limits
        {
            Health = maxValue;
        } else if (Health < minValue)//sets health to 0 incase of getting below set limits
        {
            Health = minValue;
        }

        for (int i = 0; i < skills.Count; i++)
        {
            if (Health == skills[i])
            {
                Health = Health + amount;

            }
        }
        return Health;
    }

    public int healthDecrease(int Health, int amount)
    {
        if (Health > maxValue)//sets health to 999 incase of getting above set limits
        {
            Health = maxValue;
        }
        else if (Health < minValue)//sets health to 0 incase of getting below set limits
        {
            Health = minValue;
        }

        for (int i = 0; i < skills.Count; i++)
        {
            if (Health == skills[i])
            {
                Health = Health - amount;

            }
        }
        return Health;
    }

    public int fortitudeIncrease(int Fortitude, int amount)
    {
        if (Fortitude > maxValue)//sets fortitude to 999 incase of getting above set limits
        {
            Fortitude = maxValue;
        }
        else if (Fortitude < minValue)//sets fortitude to 0 incase of getting below set limits
        {
            Fortitude = minValue;
        }

        for (int i = 0; i < skills.Count; i++)
        {
            if (Fortitude == skills[i])
            {
                Fortitude = Fortitude + amount;

            }
        }
        return Fortitude;
    }

    public int fortitudeDecrease(int Fortitude, int amount)
    {
        if (Fortitude > maxValue)//sets fortitude to 999 incase of getting above set limits
        {
            Fortitude = maxValue;
        }
        else if (Fortitude < minValue)//sets fortitude to 0 incase of getting below set limits
        {
            Fortitude = minValue;
        }

        for (int i = 0; i < skills.Count; i++)
        {
            if (Fortitude == skills[i])
            {
                Fortitude = Fortitude - amount;

            }
        }
        return Fortitude;
    }

    public int spiritIncrease(int Spirit, int amount)
    {
        if (Spirit > maxValue)//sets spirit to 999 incase of getting above set limits
        {
            Spirit = maxValue;
        }
        else if (Spirit < minValue)//sets spirit to 0 incase of getting below set limits
        {
            Spirit = minValue;
        }

        for (int i = 0; i < skills.Count; i++)
        {
            if (Spirit == skills[i])
            {
                Spirit = Spirit + amount;

            }
        }
        return Spirit;
    }

    public int spiritDecrease(int Spirit, int amount)
    {
        if (Spirit > maxValue)//sets spirit to 999 incase of getting above set limits
        {
            Spirit = maxValue;
        }
        else if (Spirit < minValue)//sets spirit to 0 incase of getting below set limits
        {
            Spirit = minValue;
        }

        for (int i = 0; i < skills.Count; i++)
        {
            if (Spirit == skills[i])
            {
                Spirit = Spirit + amount;

            }
        }
        return Spirit;
    }

}
