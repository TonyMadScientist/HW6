using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Plant Configuration")]
    public PlantData plantData;

    private GameObject plantedModel;
    private GameObject growingModel;
    private GameObject matureModel;

    private int daysInStage = 0;
    private int stage = 0;

    private DayNightControl dayNight;

    private void Awake()
    {
        plantedModel =
            transform.Find("Stage1_Planted")?.gameObject;

        growingModel =
            transform.Find("Stage2_Growing")?.gameObject;

        matureModel =
            transform.Find("Stage3_Mature")?.gameObject;

        if (plantedModel == null ||
            growingModel == null ||
            matureModel == null)
        {
            Debug.LogError(
                name + " is missing a plant stage child.",
                this
            );
        }

        if (plantData == null)
        {
            Debug.LogError(
                name + " does not have PlantData assigned.",
                this
            );
        }
    }

    private void Start()
    {
        ShowStage(0);

        GameObject dayNightObject =
            GameObject.Find("DayNightController");

        if (dayNightObject != null)
        {
            dayNight =
                dayNightObject.GetComponent<DayNightControl>();

            if (dayNight != null)
            {
                dayNight.dayPassedEvent.AddListener(ADayPassed);
                dayNight.seasonChangedEvent.AddListener(ASeasonChanged);
            }
        }
    }

    private void OnDestroy()
    {
        if (dayNight != null)
        {
            dayNight.dayPassedEvent.RemoveListener(ADayPassed);
            dayNight.seasonChangedEvent.RemoveListener(ASeasonChanged);
        }
    }

    public void ADayPassed()
    {
        if (plantData == null)
            return;

        if (stage >= 2)
            return;

        daysInStage++;

        if (daysInStage >= plantData.daysPerGrowthStage)
        {
            daysInStage = 0;
            ShowStage(stage + 1);
        }

        Debug.Log(
            name +
            " grew. Stage: " +
            stage +
            " | Days in stage: " +
            daysInStage
        );
    }

    public void ASeasonChanged()
    {
        if (dayNight == null || plantData == null)
            return;

        DayNightControl.Season currentSeason =
            dayNight.GetCurrentSeason();

        if (!plantData.CanBePlantedInSeason(currentSeason))
        {
            Debug.Log(
                name +
                " was removed because it cannot grow during " +
                currentSeason
            );

            Destroy(gameObject);
        }
    }

    private void ShowStage(int newStage)
    {
        stage = newStage;

        if (plantedModel != null)
            plantedModel.SetActive(stage == 0);

        if (growingModel != null)
            growingModel.SetActive(stage == 1);

        if (matureModel != null)
            matureModel.SetActive(stage == 2);
    }

    public bool IsMature()
    {
        return stage == 2;
    }
}