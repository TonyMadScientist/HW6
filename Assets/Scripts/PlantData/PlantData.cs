using UnityEngine;

[CreateAssetMenu(fileName = "New Plant Data", menuName = "Farming/Plant Data")]
public class PlantData : ScriptableObject
{
    public string plantName;
    public GameObject plantPrefab;

    public DayNightControl.Season[] plantingSeasons;

    public int seedAmount = 1;
    public float growthSpeed = 1f;
    public int daysPerGrowthStage = 2;

    public bool CanBePlantedInSeason(DayNightControl.Season currentSeason)
    {
        foreach (DayNightControl.Season season in plantingSeasons)
        {
            if (season == currentSeason)
            {
                return true;
            }
        }

        return false;
    }
}