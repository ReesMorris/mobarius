public class Account {

    public string username;
    public string email;
    public string created_at;
    public int xp;

    public delegate void OnXPChanged(int newXP);
    public static event OnXPChanged onXPChanged;

    public Account(string _username, string _email, string _created_at, int _xp) {
        username = _username;
        email = _email;
        created_at = _created_at;
        xp = _xp;
    }

    public void IncrementXP(int amount) {
        xp += amount;
        onXPChanged(xp);
    }

    // TBC ...
}
