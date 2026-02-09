using System;
using UnityEngine;

public interface IObservableStat
{
    public event Action<float> OnValueChanged;
    public event Action<float> OnMaxValueChanged;
}
