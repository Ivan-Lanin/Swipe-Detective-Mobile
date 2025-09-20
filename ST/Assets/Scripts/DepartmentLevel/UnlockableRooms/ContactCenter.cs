using UnityEngine;

public class ContactCenter : BaseUnlockableRoom
{
    public override void ShowBlueprint()
    {
        base.ShowBlueprint();

        buildPriceText.text = '-' + DataManager.Instance.GameData.contactCenterPrice.ToString();
    }
}
