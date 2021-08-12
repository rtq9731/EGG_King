using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int crackedEggLimit = 30;
    public int eggMachineLimit = 30;

    public PlayerData(int crackedEggLimit, int eggMachineLimit)
    {
        this.crackedEggLimit = crackedEggLimit;
        this.eggMachineLimit = eggMachineLimit; 
    }

    public PlayerData()
    {
        this.crackedEggLimit = 30;
        this.eggMachineLimit = 30;
    }
}
