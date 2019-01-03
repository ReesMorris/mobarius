using UnityEngine;

public class UserManager : MonoBehaviour {

    public static UserManager Instance;

    public Account account;

    public delegate void OnXPChanged(int newXP);
    public static event OnXPChanged onXPChanged;

    void Start() {
        Instance = this;
    }

    public void IncrementXP(int amount) {
        account.xp += amount;
        onXPChanged(account.xp);
    }
}
