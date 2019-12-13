﻿using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public struct UpdateBoneMatrixJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<ArmBoneBuffer> BoneInfo;
    [ReadOnly] public NativeArray<ArmUpVectorBuffer> UpVectors;
    [ReadOnly] public NativeArray<ArmJointBuffer> JointPositions;
    
    [WriteOnly] public NativeArray<float4x4> BoneMatrices;   
    
    public void Execute(int boneIndex)
    {
        ArmBoneBuffer bone = BoneInfo[boneIndex];
        float3 delta = JointPositions[bone.EndIndex].pos - JointPositions[bone.StartIndex].pos;
        BoneMatrices[boneIndex] = 
            float4x4.TRS(
                JointPositions[bone.StartIndex].pos + delta * 0.5f, 
                quaternion.LookRotation(
                    delta, UpVectors[bone.upIndex].up),
                new float3(bone.Thickness, bone.Thickness, math.length(delta)));
    }
}