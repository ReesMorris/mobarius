using UnityEngine;
using UnityEngine.UI;

/*
    Handles the mechanics of the 'play' button
*/
/// <summary>
/// Handles the mechanics of the 'play' button.
/// </summary>
public class PlayButton : MonoBehaviour {
 
    // Public variables
	public string mapName;

    // Private variables
    private LobbyNetwork lobbyNetwork;
    private ScaleOnHover scaleOnHover;
    private Button button;

    // Fetch references and listeners once the game starts.
    void Start() {
        lobbyNetwork = LobbyNetwork.Instance;
        button = GetComponent<Button>();
        scaleOnHover = GetComponent<ScaleOnHover>();
        button.onClick.AddListener(OnClick);

        // Disable if button not interactable
        if (!button.interactable)
            this.enabled = false;
    }

    // Called when the button attached to this script is clicked
    void OnClick() {
        LobbyNetwork.Instance.Play(mapName);
    }

    // Disables the button if the user is searching for a match; re-enables it if not
    void Update() {
        button.interactable = scaleOnHover.enabled = (lobbyNetwork.lobbyState == LobbyNetwork.LobbyStates.none);
        if(lobbyNetwork.lobbyState == LobbyNetwork.LobbyStates.searching) {
            scaleOnHover.ResetScale();
        }
    }
}
