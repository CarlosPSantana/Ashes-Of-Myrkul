using UnityEngine;

public class Cartel : DialogueInteractuableBase
{
    protected override void Awake()
    {
        autoOpenOnPlayerEnter = false;
        closeOnPlayerExit = false;
        hidePortrait = false;
        hideName = false;

        if (string.IsNullOrWhiteSpace(displayNameOverride))
        {
            displayNameOverride = "Tutorial del sistema";
        }

        base.Awake();
    }

    protected override void Reset()
    {
        autoOpenOnPlayerEnter = false;
        closeOnPlayerExit = false;
        hidePortrait = true;
        hideName = true;
        displayNameOverride = "Cartel";
    }
}
