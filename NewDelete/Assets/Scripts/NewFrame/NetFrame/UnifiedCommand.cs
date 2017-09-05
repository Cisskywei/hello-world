using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnifiedCommand {

    public static UnifiedCommand getInstance()
    {
        return Singleton<UnifiedCommand>.getInstance();
    }
}
