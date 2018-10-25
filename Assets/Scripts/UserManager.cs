using UnityEngine;

public class UserManager : MonoBehaviour {

    public Account account;

    public delegate void OnXPChanged(int newXP);
    public static event OnXPChanged onXPChanged;

    public void IncrementXP(int amount) {
        account.xp += amount;
        onXPChanged(account.xp);
    }
}
