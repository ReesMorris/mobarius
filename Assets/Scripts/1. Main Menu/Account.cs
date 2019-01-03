public class Account {

    public string id;
    public string username;
    public string email;
    public string created_at;
    public int xp;
    public string lastGameID;
    public string sessionToken;

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
