using Unity.Mathematics;

public class ClassicalLogic
{
    private float _minRange;
    private float _maxRange;
    private int _count;
    private float _bitDistance;

    public ClassicalLogic(float minRange, float maxRange, int count)
    {
        _minRange = minRange;
        _maxRange = maxRange;
        _count = count;
        _bitDistance = math.abs(_maxRange - _minRange) / _count;
    }

    public int Fuzzy(float value)
    {
        return (int) math.round(value / _bitDistance);
    }
}
