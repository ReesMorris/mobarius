using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account {

    public string username;
    public string email;
    public string created_at;

    public Account(string _username, string _email, string _created_at) {
        username = _username;
        email = _email;
        created_at = _created_at;
    }

    // TBC ...
}
