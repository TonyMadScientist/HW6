using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FarmTileControl:MonoBehaviour
{
    public enum FarmTileCond //used for the switch states
    {
        Grass, Tilled, Watered
    }
    public FarmTileCond tileCond;

    //3 materials representing 3 different farm tile conditions
    public Material grassMat, tilledMat, wateredMat;
    private List<Material> curTileEdgeMaterial=new List<Material>();

    private AudioSource stepTileAudio;
    private AudioSource tillTileAudio;
    private AudioSource waterTileAudio;

    private int daysPassedSinceLastInteraction=0;
    [SerializeField]
    private GameObject dayNightControl;

    public void Start()
    {
        tileCond = FarmTileCond.Grass;//start with grass
        GameObject tileEdge;
        
        foreach (Transform edge in transform)
        {
            tileEdge = edge.gameObject;
            curTileEdgeMaterial.Add(tileEdge.GetComponent<MeshRenderer>().material);
        }
        //get access to the audio clips attached on the tile object
        AudioSource[] audios = gameObject.GetComponents<AudioSource>();
        stepTileAudio = audios[0];
        tillTileAudio = audios[1];
        waterTileAudio =audios[2];

        dayNightControl = GameObject.Find("DayNightController");
        dayNightControl.GetComponent<DayNightControl>().listenToDayPassEvent(this.gameObject);
    }

    public void UpdateTileMaterial()//called to update the tile appearance when the farm tile changes states
    {
        switch (tileCond)
        {
            case FarmTileCond.Grass:
                GetComponent<MeshRenderer>().material = grassMat;
                break;
            case FarmTileCond.Tilled:
                GetComponent<MeshRenderer>().material = tilledMat;
                break;
            case FarmTileCond.Watered:
                GetComponent<MeshRenderer>().material = wateredMat;
                break;
        }
    }

    //API prepared for the player to interact with tiles when steping on it and press the space key
    public void InteractWithFarmTile(ToolboxManager.Tools tool)
    {
        switch (tileCond){
            case FarmTileCond.Grass://interact with the grass tile tills it
                if (tool == ToolboxManager.Tools.hoe)
                {
                    Debug.Log("Player is tilling the tile " + transform.name);
                    tillTileAudio.Play();
                    tileCond = FarmTileCond.Tilled;
                    UpdateTileMaterial();
                    daysPassedSinceLastInteraction = 0;
                } else {
                    Debug.Log("Must equip Hoe to till the grass!");
                }
                break;
            case FarmTileCond.Tilled://interact with the tilled land waters it
                if (tool == ToolboxManager.Tools.wateringcan)
                {
                    Debug.Log("Player is watering the tile " + transform.name);
                    waterTileAudio.Play();
                    tileCond = FarmTileCond.Watered;
                    UpdateTileMaterial();
                    daysPassedSinceLastInteraction = 0;
                } else {
                    Debug.Log("Must equip Watering Can to water the soil!");
                }
                break;
            case FarmTileCond.Watered://repeated interaction on the watered tile does not change it to other conditions
                if (tool == ToolboxManager.Tools.wateringcan)
                {
                    Debug.Log("Tile is watered and ready for plants " + transform.name);
                    daysPassedSinceLastInteraction = 0;
                } else {
                    Debug.Log("Must equip Watering Can to water the soil!");
                }
                break;
        }
    }

    //API prepared for the player interaction point to select a tile
    //When the player's interaction point (a bit in front of the player) interact with a tile,
    //this tile's material gets brighter to show the current tile that the player is about to interact with
    public void EnterTile()
    {
        Debug.Log("Player entered tile "+transform.name);
        foreach(Material mat in curTileEdgeMaterial )
            mat.EnableKeyword("_EMISSION");


        //play the audio clip
        stepTileAudio.Play();
    }
    public void ExitTile()
    {
        Debug.Log("Player exited tile " + transform.name);
        foreach (Material mat in curTileEdgeMaterial)
            mat.DisableKeyword("_EMISSION");
    }

    //API provided for the day night counter to tell the tile that a day just passed
    public void ADayPassed()
    {
        Debug.Log("ADayPassed");
        daysPassedSinceLastInteraction++;
        Debug.Log(transform.name.ToString() + " days passed: " + daysPassedSinceLastInteraction.ToString());
        if (daysPassedSinceLastInteraction >= 2)//3days passed since last farm tile interaction
        {
            switch (tileCond)
            {
                case FarmTileCond.Watered: //soil dry after 2 days
                    tileCond = FarmTileCond.Tilled;
                    UpdateTileMaterial();
                    daysPassedSinceLastInteraction = 0;
                    break;
                case FarmTileCond.Tilled: //grass grows back after 2 days
                    tileCond = FarmTileCond.Grass;
                    UpdateTileMaterial();
                    daysPassedSinceLastInteraction = 0;
                    break;
            }
        }
    }

}
