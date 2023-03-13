using System;
using UnityEngine;

public interface IAttacker
{
    public BaseDamageProperty DamageProperty();
}

public class NotAttackerException : SystemException
{
    private GameObject gameObject;

    public string ErrorMessage;
    public NotAttackerException(GameObject gameObject)
    {
        this.gameObject = gameObject;
        ErrorMessage = "GameObject " + gameObject.name + " isn't a valid Attacker. Check is it an instance of IAttacker";
    }
}

