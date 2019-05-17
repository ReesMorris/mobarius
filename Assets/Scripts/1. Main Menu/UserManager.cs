using UnityEngine;

/*
    The class used to handle user variables; currently limited to XP only
*/
/// <summary>
/// The class used to handle user variables; currently limited to XP only.
/// </summary>
public class UserManager : MonoBehaviour {

    // Public variables
    public static UserManager Instance;

    public Account account;

    public delegate void OnXPChanged(int newXP);
    public static event OnXPChanged onXPChanged;

    // Allow this script to be accessed as an instance on game start.
    void Start() {
        Instance = this;
    }

    /// <summary>
    /// Increments the user's XP by a given amount.
    /// </summary>
    /// <param name="amount">The amount of XP to increase by</param>
    /// <remarks>
    /// Is not connected to the external database at this time.
    /// </remarks>
    public void IncrementXP(int amount) {
        account.xp += amount;
        onXPChanged(account.xp);
    }
}
