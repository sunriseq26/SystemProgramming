using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = System.Random;


public class InitIJob : MonoBehaviour
{
    private NativeArray<int> _nativeArrayInt;
    private Random _random;
    void Start()
    {
        _random = new Random();
        int[] array = new int[10];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = _random.Next(1, 33);

            Debug.Log($"Array[{i}] = {array[i]}");
        }
        
        
        _nativeArrayInt = new NativeArray<int>(array, Allocator.Persistent);
        
        JobInt newJob = new JobInt(){_nativeArrayInt = _nativeArrayInt};
        newJob.Schedule();
    }

    private void OnDestroy()
    {
        if (_nativeArrayInt.IsCreated)
        {
            _nativeArrayInt.Dispose();
        }
    }
}


public struct JobInt : IJob
{
    public NativeArray<int> _nativeArrayInt;

    public void Execute()
    {
        for (int i = 0; i < _nativeArrayInt.Length; i++)
        {
            if (_nativeArrayInt[i] > 10)
            {
                _nativeArrayInt[i] = 0;
            }
            
            Debug.Log($"NativeArray[{i}] = {_nativeArrayInt[i]}");
        }  
    }
}
