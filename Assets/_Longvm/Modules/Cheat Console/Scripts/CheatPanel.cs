using UnityEngine;

namespace Toolkit
{
    public class CheatPanel : MonoBehaviour
    {
        [SerializeField] CheatConsole cheatConsole;

        #region UI Events
        public void OnClickClose()
        {
            cheatConsole.CloseCheatPanel();
        }
        #endregion
    }
}
