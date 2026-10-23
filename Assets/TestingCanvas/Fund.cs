using TMPro;
using UnityEngine;

public class Fund : MonoBehaviour
{
    public int fund=0;
    public void AddFund(int amount)
    {
        fund += amount;
        gameObject.GetComponent<TMP_Text>().text = "Fund: " + fund.ToString();
    }
    public void DeductFund(int amount)
    {
        fund -= amount;
        gameObject.GetComponent<TMP_Text>().text = "Fund: " + fund.ToString();
    }
}
