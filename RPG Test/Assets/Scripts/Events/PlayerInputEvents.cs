using System;
using UnityEngine;

public class PlayerInputEvents {
    #region Dungeon

    public static event Action<bool> OnSetMinimapView;

    public static void InvokeOnSetMinimapView(bool value) {
        OnSetMinimapView?.Invoke(value);
    }

    #endregion
}
