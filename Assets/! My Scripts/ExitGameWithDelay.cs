using GuruLaghima;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
    public class ExitGameWithDelay: MonoBehaviour
    {
        [SerializeField] public float delay = 1f;
        // Start is called before the first frame update
        public void ExitWithDelay()
        {
            Invoke("Exit", delay);
        }

        void Exit()
        {
            MyDebug.Log("QUIT!");
            Application.Quit();
        }
    }
}
