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

        // Disable if button not interactable
        if (!button.interactable)
            this.enabled = false;
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
