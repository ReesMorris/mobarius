/*
    Stores variables relating to a user's Account, typically filled with values from the database after authentication
*/
/// <summary>
/// Stores variables relating to a user's Account, typically filled with values from the database after authentication.
/// </summary>
public class Account {

    // Public variables
    public string id;
    public string username;
    public string email;
    public string created_at;
    public int xp;
    public string lastGameID;
    public string sessionToken;

    /// <summary>
    /// The constructor for the Account class
    /// </summary>
    /// <param name="_id">The user ID</param>
    /// <param name="_username">The username</param>
    /// <param name="_email">The users email address</param>
    /// <param name="_created_at">The timestamp that the account was created on</param>
    /// <param name="_xp">The users XP amount</param>
    /// <param name="_lastGameID">[Redundant] The ID of the most recent game played by the user </param>
    /// <param name="_sessionToken">The session token to authenticate the user for this connection</param>
    public Account(string _id, string _username, string _email, string _created_at, int _xp, string _lastGameID, string _sessionToken) {
        id = _id;
        username = _username;
        email = _email;
        created_at = _created_at;
        xp = _xp;
        lastGameID = _lastGameID;
        sessionToken = _sessionToken;
    }
}
