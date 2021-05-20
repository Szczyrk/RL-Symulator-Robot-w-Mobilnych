using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class FuzzyLogic
{
    private float _minRange;
    private float _maxRange;
    private int _count;
    private float _bitDistance;
    private float _halfBitDistance;
    List<float[]> ImplicationList = new List<float[]>();
    float[] membership;
    float center, minRange, maxRange;

    public FuzzyLogic(float minRange, float maxRange, int count)
    {
        _minRange = minRange;
        _maxRange = maxRange;
        _count = count;
        membership = new float[_count];
        _bitDistance = math.abs(_maxRange - _minRange) / _count;
        _halfBitDistance = _bitDistance / 2;
    }

    private float TriangularMembershipFunction(float value, int linguistic)
    {
        if (linguistic == 0)
        {
            minRange = _minRange;
            maxRange = _maxRange / 2;
            center = math.abs(maxRange + minRange) / 2;
            if (value <= minRange)
                return 0;
            if (value >= maxRange)
                return 0;
            if (value > minRange && value < center)
                return 1;
            return (maxRange - value) / (maxRange - center);
        }
        else if (linguistic == 1)
        {
            minRange = math.abs(_minRange + _maxRange / 2) / 2;
            maxRange = math.abs(_maxRange / 2 + _maxRange) / 2;
            center = math.abs(maxRange + minRange) / 2;
            if (value <= minRange)
                return 0;
            if (value >= maxRange)
                return 0;
            if (value > minRange && value < center)
                return (value - minRange) / (center - minRange);
            return 1;
        }
        else
        {
            minRange = _maxRange / 2;
            maxRange = _maxRange;
            center = math.abs(maxRange + minRange) / 2;
            if (value <= minRange)
                return 0;
            if (value >= maxRange)
                return 0;
            if (value > minRange && value < center)
                return (value - minRange) / (center - minRange);
            return (maxRange - value) / (maxRange - center);
        }
    }

    public float[] ImplicationMIN(float alfa, float linguistic)
    {
        if (linguistic == 0)
        {
            minRange = _minRange;
            maxRange = _maxRange / 2;
            center = math.abs(maxRange + minRange) / 2;

            return new float[]
            {
                minRange, 1, (center - minRange) * alfa + minRange, alfa, -(maxRange - center) * alfa + maxRange, alfa
            };
        }
        else if (linguistic == 1)
        {
            minRange = math.abs(_minRange + _maxRange / 2) / 2;
            maxRange = math.abs(_maxRange / 2 + _maxRange) / 2;
            center = math.abs(maxRange + minRange) / 2;
            return new float[]
            {
                (center - minRange) * alfa + minRange, alfa, -(maxRange - center) * alfa + maxRange, alfa
            };
        }
        else
        {
            minRange = _maxRange / 2;
            maxRange = _maxRange;
            center = math.abs(maxRange + minRange) / 2;
            return new float[]
            {
                (center - minRange) * alfa + minRange, alfa, -(maxRange - center) * alfa + maxRange, alfa,
                maxRange, 1
            };
        }
    }

    public float DefuzzifyByTheCenterOfGravityMethod(float value)
    {
        
        for (int i = 0; i < _count; i++)
        {
            membership[i] = TriangularMembershipFunction(value, i);
        }

        
        for (int i = 0; i < _count; i++)
        {
            if (membership[i] != 0)
                ImplicationList.Add(ImplicationMIN(membership[i], i));
        }


        float l = 0, m = 0;
        foreach (var implication in ImplicationList)
        {
            for (int i = 0; i < implication.Length; i = i + 2)
            {
                l += implication[i] * implication[i + 1];
                m += implication[i + 1];
            }
        }
        ImplicationList.Clear();
        return l / m;
    }

    public float Fuzzy(float value)
    {
        return DefuzzifyByTheCenterOfGravityMethod(value);
    }
}