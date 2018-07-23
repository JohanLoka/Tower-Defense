using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDLib : MonoBehaviour
{
    #region Auth

    public static bool LOGIN()
    {
        //  API-request
        return false;
    }

    public static bool LOGOUT()
    {
        //  API-request
        return false;
    }

    #endregion

    #region Settings

    public static PlayerSettings GET_PLAYER_SETTINGS()
    {
        PlayerSettings settings = new PlayerSettings();

        //  Load settings from api or storage and modify settings

        return settings;
    }
    #endregion


    #region Math



    #endregion
}
