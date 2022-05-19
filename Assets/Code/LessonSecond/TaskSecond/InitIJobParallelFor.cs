using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class InitIJobParallelFor : MonoBehaviour
{
    [SerializeField] private Vector3[] _incomingPositions;
    [SerializeField] private Vector3[] _incomingVelocities;

    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _finalPositions;
        
        
    void Start()
    {
        _positions = new NativeArray<Vector3>(_incomingPositions, Allocator.Persistent);
        _velocities = new NativeArray<Vector3>(_incomingVelocities, Allocator.Persistent);
        _finalPositions = new NativeArray<Vector3>(_incomingPositions.Length, Allocator.Persistent);

        SumJob newJob = new SumJob()
            { positions = _positions, velocities = _velocities, finalPositions = _finalPositions };
        JobHandle jobHandle = newJob.Schedule(_finalPositions.Length, 0);
        jobHandle.Complete();

        for (int i = 0; i < _finalPositions.Length; i++)
        {
            Debug.Log($"FinalPositions[{i}] = {_finalPositions[i]}");
        }
    }
    
    private void OnDestroy()
    {
        if (_positions.IsCreated)
            _positions.Dispose();
        else if (_velocities.IsCreated)
            _velocities.Dispose();
        else if (_finalPositions.IsCreated)
            _finalPositions.Dispose();
    }
}

public struct SumJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<Vector3> positions;
    [ReadOnly]
    public NativeArray<Vector3> velocities;
    [WriteOnly]
    public NativeArray<Vector3> finalPositions;
    
    
    public void Execute(int index)
    {
        finalPositions[index] = positions[index] + velocities[index];
    }
}
