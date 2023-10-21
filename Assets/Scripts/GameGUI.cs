using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGUI : MonoBehaviour
{
    [SerializeField] private float _minMaxFpsUpdateInterval = 2;

    private readonly List<float> _frameDeltas = new();
    private int _minMaxUpdateTimer;
    
    private void Start()
    {
        StartCoroutine(UpdateMinMax());
    }

    private IEnumerator UpdateMinMax()
    {
        while (this)
        {
            yield return new WaitForSeconds(_minMaxFpsUpdateInterval);
            _frameDeltas.Clear();
        }
    }

    private void Update()
    {
        _frameDeltas.Add(Time.deltaTime);
    }

    private void OnGUI()
    {
        GUI.skin.textArea.fontSize = 24;
        var fps = FPS(Time.deltaTime);
        var minMaxFps = GetMinMaxFPS();
        GUI.TextArea(new Rect(100, 100, 300, 50), StringCache<int>.Get("FPS: {0}", fps));
        GUI.TextArea(new Rect(100, 150, 300, 50), StringCache<int>.Get("MIN FPS: {0}", minMaxFps.Min));
        GUI.TextArea(new Rect(100, 200, 300, 50), StringCache<int>.Get("MAX FPS: {0}", minMaxFps.Max));
    }

    private (int Min, int Max) GetMinMaxFPS()
    {
        if (_frameDeltas.Count == 0)
        {
            var fps = FPS(Time.deltaTime);
            return (fps, fps);
        }

        var (minDelta, maxDelta) = GetMinMax(_frameDeltas);
        return (FPS(maxDelta), FPS(minDelta));
    }

    private static int FPS(float deltaTime)
    {
        return Mathf.FloorToInt(1 / deltaTime);
    }

    private static (float Min, float Max) GetMinMax(List<float> elements)
    {
        var min = float.MaxValue;
        var max = float.MinValue;

        for (int i = 0, end = elements.Count; i < end; i++)
        {
            if (elements[i] > max)
            {
                max = elements[i];
            }
            else if (elements[i] < min)
            {
                min = elements[i];
            }
        }

        return (min, max);
    }
}