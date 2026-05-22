using UnityEngine;

namespace Toolkit
{
    public class CheatBehaviour : MonoBehaviour
    {
        [SerializeReference] CheatAction action;

        private void OnEnable()
        {
            action?.Init();
        }

        public void OnClick()
        {
            action?.Execute();
        }
    }
}
