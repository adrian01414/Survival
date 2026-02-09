using System;

public class Stat : IObservableStat
{
    public event Action<float> OnValueChanged;
    public event Action<float> OnMaxValueChanged;

    private float _value;
    private float _maxValue;

    public float Value
    {
        get => _value;
        set {
            _value = Math.Clamp(value, 0f, _maxValue);
            OnValueChanged?.Invoke(_value);
        }
    }

    public float MaxValue
    {
        get => _maxValue;
        set
        {
            if (value < 0f)
            {
                Value = 0f;
                _maxValue = 0f;
                OnMaxValueChanged?.Invoke(0f);
                return;
            } else if (value < _value)
            {
                Value = value;
            }
            _maxValue = value;
            OnMaxValueChanged?.Invoke(value);
        }
    }

    public Stat(float value, float maxValue)
    {
        _maxValue = maxValue < 0f ? throw new ArgumentOutOfRangeException(nameof(maxValue)) : maxValue;
        Value = value;
    }
}
