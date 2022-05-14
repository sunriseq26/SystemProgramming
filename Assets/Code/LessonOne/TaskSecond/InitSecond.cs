using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace LessonOne
{
    public class InitSecond : MonoBehaviour
    {
        private void Start()
        {
            Task<Task<bool>> taskResult;
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancel = cancelTokenSource.Token;
            
            Task<bool> task1 = Task1(cancel);
            Task<bool> task2 = Task2(cancel);
            taskResult = Task.WhenAny(task1, task2);
            
            if (taskResult.Result != null)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
                Debug.Log($"Finish");
            }
        }

        private async Task<bool> Task1(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
            {
                Debug.Log($"Task1 - Canceled");
                return false;
            }

            await Task.Delay(1000);
            Debug.Log($"Task1 - Finish");
            return true;
        }

        private async Task<bool> Task2(CancellationToken cancel)
        {
            int count = 0;
            if (cancel.IsCancellationRequested)
            {
                Debug.Log($"Task2 - Canceled");
                return false;
            }

            for (int i = 0; i < 58; i++)
            {
                count++;
            }

            if (count == 59)
            {
                Debug.Log($"Task2 - Finish");
            }
            
            return true;
        }
    }
}