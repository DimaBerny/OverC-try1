using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCupboardVisual : MonoBehaviour
{
    // A reference to the PlateCupboard script
    [SerializeField] private PlateCupboard plateCupboard;
    //top point - place where plate visuals will spawn
    //using only visual not script attached. Real prop will be only if we interact with this cupboard
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform plateVisual;

    // A list of the spawned plate visuals
    private List<GameObject> plateVisualList;

    private void Awake()
    {
        plateVisualList = new List<GameObject>();
    }

    private void Start()
    {
        plateCupboard.OnPlateSpawned += PlateCupboard_OnPlateSpawned;
        plateCupboard.OnPlateDespawned += PlateCupboard_OnPlateDespawned;
    }
    //object sender - parameter represents the object that raised the event(PlateCupboard)
    //object sender represents the object that raised the event and EventArgs p represents the event data.
    private void PlateCupboard_OnPlateSpawned(object sender, System.EventArgs p)
    {
        // Spawn a new plate visual and store its transform
        Transform plateVisualTransform = Instantiate(plateVisual, topPoint);

        //here we spawn visual 0.1 higher each plate
        plateVisualTransform.localPosition = new Vector3(0, 0.1f * plateVisualList.Count, 0);
        plateVisualList.Add(plateVisualTransform.gameObject);

    }
    private void PlateCupboard_OnPlateDespawned(object sender, System.EventArgs p)
    {
        // Get a reference to the last spawned plate visual in the list
        GameObject plate = plateVisualList[plateVisualList.Count - 1];
        // Remove the visual from LIST
        plateVisualList.Remove(plate);
        //Despawn / destroy Visual game object
        Destroy(plate);
    }
}
