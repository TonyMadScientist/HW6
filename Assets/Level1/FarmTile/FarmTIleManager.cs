using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class FarmTIleManager : MonoBehaviour
{
    public GameObject FarmTile;
    public Transform FarmTileSpawnStartLocation;

    // UI
    public GameObject winMessage;
    public TMP_Text fundAmount;
    public GameObject nextLevelButton;

    // Money awarded for completing Level 1
    public int winningFund = 100;

    private Vector3 curFarmTileSpawnPos;

    public int numFarmTileX = 4;
    public int numFarmTileZ = 4;
    public float gapSizeBetweenTiles = 0.1f;

    private List<FarmTileControl> farmTiles = new List<FarmTileControl>();

    private bool levelWon = false;

    private void Start()
    {
        curFarmTileSpawnPos = FarmTileSpawnStartLocation.position;

        curFarmTileSpawnPos.y =
            -FarmTile.transform.localScale.y / 2 + 0.05f;

        int numTileGenerated = 0;

        for (int i = 0; i < numFarmTileZ; i++)
        {
            for (int j = 0; j < numFarmTileX; j++)
            {
                GameObject curFarmTile = Instantiate(
                    FarmTile,
                    curFarmTileSpawnPos,
                    Quaternion.identity
                );

                curFarmTileSpawnPos.x +=
                    FarmTile.transform.localScale.x +
                    gapSizeBetweenTiles;

                numTileGenerated++;

                curFarmTile.name =
                    "Farm Tile " + numTileGenerated.ToString();

                curFarmTile.transform.parent = transform;

                FarmTileControl tileControl =
                    curFarmTile.GetComponent<FarmTileControl>();

                if (tileControl != null)
                {
                    farmTiles.Add(tileControl);
                }
            }

            curFarmTileSpawnPos.z +=
                FarmTile.transform.localScale.z +
                gapSizeBetweenTiles;

            curFarmTileSpawnPos.x =
                FarmTileSpawnStartLocation.position.x;
        }

        // Hide win UI when the level starts
        if (winMessage != null)
        {
            winMessage.SetActive(false);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.SetActive(false);
        }

        // Start the level with the player acutal fund balance from FarmFund script 
        if (fundAmount != null)
        {
            fundAmount.text = "Fund: $" + FarmFund.Get().money;
        }
    }

    private void Update()
    {
        if (levelWon)
            return;

        CheckForWin();
    }

    private void CheckForWin()
    {
        if (farmTiles.Count == 0)
            return;

        if (FarmFund.Get().harvestedCount > 9)
            Win();

        foreach (FarmTileControl tile in farmTiles)
        {
            if (tile.tileCond != FarmTileControl.FarmTileCond.Watered)
            {
                return;
            }
        }
        Win();
    }

    private void Win()
    {
        levelWon = true;

        Debug.Log("YOU WIN! All farm tiles are watered!");

        if (winMessage != null)
        {
            winMessage.SetActive(true);
        }

        
        // give the player fund and show the new balance
        FarmFund.Get().AddMoney(winningFund);

        if (fundAmount != null)
        {
            fundAmount.text = "Fund: $" + FarmFund.Get().money;
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.SetActive(true);
        }
    }

    // Called by the Next Level button
    public void GoToStore()
    {
        SceneManager.LoadScene("Scene2-Store");
    }
}