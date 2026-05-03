using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class Account
{
    public int Id { get;private set; }

    public string Name { get;private set; }

    public AccountType Type { get;private set; }

    //constructor
    public Account(string name, AccountType type)
    {
        Name = name;
        Type = type;
    }
}
