using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameGUI : MonoBehaviour
{
    [SerializeField] private float _minMaxFpsExpirationTime = 2f;

    private readonly List<(float Timestamp, float DeltaTime)> _frameDeltas = new();
    private int _minMaxUpdateTimer;

    private void Update()
    {
        RemoveExpiredFrameDeltas();
        _frameDeltas.Add((Timestamp: Time.time, DeltaTime: Time.deltaTime));
    }

    private void RemoveExpiredFrameDeltas()
    {
        using var pooledList = ListPool<(float Time, float DeltaTime)>.Get(out var frameDeltasToRemove);
        var minValidTime = Time.time - _minMaxFpsExpirationTime;
        
        foreach (var pair in _frameDeltas)
        {
            if (pair.Timestamp < minValidTime)
                frameDeltasToRemove.Add(pair);
        }

        foreach (var frameDelta in frameDeltasToRemove)
        {
            _frameDeltas.Remove(frameDelta);
        }
    }

    private void OnGUI()
    {
        GUI.skin.textArea.fontSize = 24;
        var fps = Fps(Time.deltaTime);
        var minMaxFps = GetMinMaxFps();
        GUI.TextArea(new Rect(100, 100, 300, 50), StringCache<int>.Get("FPS: {0}", fps));
        GUI.TextArea(new Rect(100, 150, 300, 50), StringCache<int>.Get("MIN FPS: {0}", minMaxFps.Min));
        GUI.TextArea(new Rect(100, 200, 300, 50), StringCache<int>.Get("MAX FPS: {0}", minMaxFps.Max));
    }

    private (int Min, int Max) GetMinMaxFps()
    {
        if (_frameDeltas.Count == 0)
        {
            var fps = Fps(Time.deltaTime);
            return (fps, fps);
        }

        var minDelta = float.MaxValue;
        var maxDelta = float.MinValue;

        for (int i = 0, end = _frameDeltas.Count; i < end; i++)
        {
            var dt = _frameDeltas[i].DeltaTime;
            if (dt > maxDelta)
            {
                maxDelta = dt;
            }
            else if (dt < minDelta)
            {
                minDelta = dt;
            }
        }
        
        return (Fps(maxDelta), Fps(minDelta));
    }

    private static int Fps(float deltaTime)
    {
        return Mathf.FloorToInt(1 / deltaTime);
    }
}