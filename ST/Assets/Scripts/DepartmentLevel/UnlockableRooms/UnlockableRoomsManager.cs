using UnityEngine;

public enum RoomType
{
    EvidenceRoom,
    ContactCenter
}

public class UnlockableRoomsManager : MonoBehaviour
{
    [SerializeField] private BaseUnlockableRoom evidenceRoom;
    [SerializeField] private BaseUnlockableRoom contactCenter;
    [SerializeField] private AudioSource constructionAudioManager;

    private void Awake()
    {
        evidenceRoom.buildButton.onClick.AddListener(() => BuildRoom(RoomType.EvidenceRoom));
        contactCenter.buildButton.onClick.AddListener(() => BuildRoom(RoomType.ContactCenter));
        TryShowRooms();
    }

    private void TryShowRooms()
    {         
        if (!TryShowEvidenceRoom()) return;
        if (!TryShowContactCenter()) return;
    }

    private bool TryShowEvidenceRoom()
    {
        if (DataManager.Instance.GameData.evidenceRoomLevel > 0)
        {
            evidenceRoom.ShowRoom(DataManager.Instance.GameData.evidenceRoomLevel);
            return true;
        }
        else
        {
            evidenceRoom.ShowBlueprint();
            return false;
        }
    }

    private bool TryShowContactCenter()
    {
        if (DataManager.Instance.GameData.contactCenterLevel > 0)
        {
            contactCenter.ShowRoom(DataManager.Instance.GameData.contactCenterLevel);
            return true;
        }
        else
        {
            contactCenter.ShowBlueprint();
            return false;
        }
    }

    private void BuildRoom(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.EvidenceRoom:
                if (DataManager.Instance.GameData.evidenceRoomPrice <= DataManager.Instance.GameData.gold)
                {
                    DataManager.Instance.Add(Data.EvidenceRoomLevel, 1);
                    DataManager.Instance.Add(Data.Gold, -DataManager.Instance.GameData.evidenceRoomPrice);
                    SFXManager.Instance.PlaySound(constructionAudioManager, SFXType.Construction);
                    evidenceRoom.BuildRoom();
                    TryShowContactCenter();
                }
                else
                {
                    Debug.Log("Not enough money to build Evidence Room.");
                }
                break;
            case RoomType.ContactCenter:
                if (DataManager.Instance.GameData.contactCenterPrice <= DataManager.Instance.GameData.gold)
                {
                    DataManager.Instance.Add(Data.ContactCenterLevel, 1);
                    DataManager.Instance.Add(Data.Gold, -DataManager.Instance.GameData.contactCenterPrice);
                    SFXManager.Instance.PlaySound(constructionAudioManager, SFXType.Construction);
                    contactCenter.BuildRoom();
                }
                else
                {
                    Debug.Log("Not enough money to build Communication Center.");
                }
                break;
            default:
                Debug.LogWarning("Unknown room type: " + roomType);
                break;
        }
    }

    private void OnDestroy()
    {
        evidenceRoom.buildButton.onClick.RemoveAllListeners();
        contactCenter.buildButton.onClick.RemoveAllListeners();
    }
}
