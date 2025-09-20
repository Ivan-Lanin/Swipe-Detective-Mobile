using UnityEngine;

public class EvidenceRoom : BaseUnlockableRoom
{
    public override void ShowBlueprint()
    {
        base.ShowBlueprint();

        buildPriceText.text = '-' + DataManager.Instance.GameData.evidenceRoomPrice.ToString();
    }
}
