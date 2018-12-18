using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour {
 
	public string mapName;

    private LobbyNetwork lobbyNetwork;
    private ScaleOnHover scaleOnHover;
    private Button button;

    void Start() {
        lobbyNetwork = LobbyNetwork.Instance;
        button = GetComponent<Button>();
        scaleOnHover = GetComponent<ScaleOnHover>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick() {
        LobbyNetwork.Instance.Play(mapName);
    }

    void Update() {
        button.interactable = scaleOnHover.enabled = (lobbyNetwork.lobbyState == LobbyNetwork.LobbyStates.none);
        if(lobbyNetwork.lobbyState == LobbyNetwork.LobbyStates.searching) {
            scaleOnHover.ResetScale();
        }
    }
}
