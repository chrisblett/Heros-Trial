using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Item;
using Entity.Player;
using Entity.Stats;
public class PotionData
{
    private IConsumable potion;

    public int Count { get; private set; }
    public string Name { get; private set; }

    public PotionData(IConsumable potion, string name)
    {
        this.potion = potion;
        Count = 0;
        Name = name;
    }

    public void Use()
    {
        Count--;
        potion.Consume();
    }

    public void Add()
    {
        Count++;
    }
}


public class PlayerPotionManager : MonoBehaviour
{
    Dictionary<PotionType, PotionData> potions;
    public PotionType SelectedPotion { get; private set; }
    
    [field : SerializeField] public bool IsTimeLimitedPotionActive { get; private set; }
    public PotionType activePotion;

    void Start()
    {
        SelectedPotion = 0;
        Debug.LogWarning("PlayerPotionManager: Selected Potion: " + SelectedPotion);

        //Debug.Log("Selected potion: " + selectedPotion);

        // Keeps track of potion data of different types
        potions = new Dictionary<PotionType, PotionData>();

        // Add the potions
        PotionData weaponDamagePotionData = new PotionData(GetComponent<WeaponDamagePotion>(), "Potion of Strength");
        potions.Add(PotionType.WeaponDamage, weaponDamagePotionData);
        potions.Add(PotionType.Health, new PotionData(new HealthPotion(), "Potion of Healing"));
    }

    void Update()
    {
        bool potionInputKeysDown = Input.GetKeyDown(KeyCode.C) ||
            Input.GetKeyDown(KeyCode.F);

        if(potionInputKeysDown)
        {
            // Do not allow the use of potions when dead
            if (GameController.Instance.CanPlayerTarget())
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    CyclePotion();
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    UsePotion();
                }
            }
        }   
    }

    public PotionData AddPotion(PotionType type)
    {
        PotionData potionData = null;

        if(potions.ContainsKey(type))
        {
            PotionData potion = potions[type];
            potion.Add();
            potionData = potion;
           
            //Debug.Log("You now have " + potion.Count + " " + type + " potions");           
        }
        else
        {
            Debug.LogWarning("No potion of type " + type + " exists in the dictionary!");            
        }

        return potionData;
    }


    public void SetTimeLimitedPotionActive(bool active)
    {
        IsTimeLimitedPotionActive = active;
    }
      
    public PotionData GetSelectedPotion()
    {
        return potions[SelectedPotion];
    }

    private void UsePotion()
    {
        if(potions.ContainsKey(SelectedPotion))
        {
            PotionData potion = potions[SelectedPotion];
            if (potion.Count > 0)
            {
                // Specific case for checking whether the player can use the health potion
                if(SelectedPotion == PotionType.Health)
                {
                    float currentHealth = GameController.Instance.Player.GetComponent<PlayerCombat>().Health;

                    // Check to see if they are full health already
                    if (currentHealth == PlayerStats.Instance.maximumHealth)
                    {
                        GameController.Instance.UpdatePlayerInfoUIText("You are already at full health");

                        return;
                    }
                }

                activePotion = SelectedPotion;
                potion.Use();

                //Debug.Log("You now have " + potion.Count + " " + selectedPotion + " potions");

                GameController.Instance.OnPotionConsumed(potion);                
            }
            else
            {
                //Debug.Log("No potions available");

                //GameController.Instance.OnPotionsUnavailable();
                GameController.Instance.UpdatePlayerInfoUIText("No potions available");
            }
        }
        else
        {
            Debug.LogWarning("No potion of type " + SelectedPotion + " exists in the dictionary!");
        }
    }

    private void CyclePotion()
    {
        SelectedPotion = (PotionType)((int)(SelectedPotion + 1) % (int)PotionType.MaxPotionTypes);

        //Debug.Log("Selected potion is " + selectedPotion);

        GameController.Instance.OnSelectedPotionChanged(GetSelectedPotion());        
    }
}
