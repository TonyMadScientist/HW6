using UnityEngine;

public class Plant : MonoBehaviour
{
    private GameObject plantedModel;   // small
    private GameObject growingModel;   // medium
    private GameObject matureModel;    // large

    public int daysToGrowing = 2;      // days as a seedling before growing
    public int daysToMature = 2;       // days as growing before mature

    private int daysInStage = 0;
    private int stage = 0;             // 0 = planted, 1 = growing, 2 = mature
    private DayNightControl dayNight;

    void Awake()
    {
        plantedModel = transform.Find("Stage1_Planted")?.gameObject;
        growingModel = transform.Find("Stage2_Growing")?.gameObject;
        matureModel  = transform.Find("Stage3_Mature")?.gameObject;

        if (plantedModel == null || growingModel == null || matureModel == null)
            Debug.LogError(name + " is missing a stage child.", this);
    }

    void Start()
    {
        ShowStage(0);

        dayNight = GameObject.Find("DayNightController").GetComponent<DayNightControl>();
        dayNight.dayPassedEvent.AddListener(ADayPassed);
    }

    void OnDestroy()
    {
        if (dayNight != null)
            dayNight.dayPassedEvent.RemoveListener(ADayPassed);
    }

    public void ADayPassed()
    {
        Debug.Log(name + " day passed. stage=" + stage + " daysInStage=" + daysInStage);
        if (stage >= 2) return;   // mature: no more changes

        daysInStage++;

        if (stage == 0 && daysInStage >= daysToGrowing)
        {
            daysInStage = 0;
            ShowStage(1);
        }
        else if (stage == 1 && daysInStage >= daysToMature)
        {
            daysInStage = 0;
            ShowStage(2);
        }
    }

    void ShowStage(int s)
    {
        stage = s;
        if (plantedModel != null) plantedModel.SetActive(s == 0);
        if (growingModel != null) growingModel.SetActive(s == 1);
        if (matureModel  != null) matureModel.SetActive(s == 2);
    }
}