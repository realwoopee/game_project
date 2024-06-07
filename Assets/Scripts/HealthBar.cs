using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;

    [System.Obsolete]
    void Update()
    {
        //Level ends if health falls to 0
        if (healthAmount <= 0)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

       //Press Enter to damage the character
       if (Input.GetKeyDown(KeyCode.Return)) 
       {
            TakeDamage(10);
       }

        //Press Backspace to heal the character
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Heal(5);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100;   
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / 100;   
    }
}
