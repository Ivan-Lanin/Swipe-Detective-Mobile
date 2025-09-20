using UnityEngine.UI;

public interface IUnlockable
{
    Button BuildButton { get; set; }
    void ShowBlueprint();
    void ShowRoom(int level);
}
