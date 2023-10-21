using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleGUI : MonoBehaviour
{
    [SerializeField] private float _minMaxUpdateInterval = 2;

    private readonly List<float> _frames = new();
    private int _minMaxUpdateTimer;
    
    private void Start()
    {
        StartCoroutine(UpdateMinMax());
    }

    private IEnumerator UpdateMinMax()
    {
        while (this)
        {
            yield return new WaitForSeconds(_minMaxUpdateInterval);
            _frames.Clear();
        }
    }

    private void Update()
    {
        _frames.Add(Time.deltaTime);
    }

    private void OnGUI()
    {
        GUI.skin.textArea.fontSize = 24;
        GUI.TextArea(new Rect(100, 100, 300, 50), $"FPS: {FPS(Time.deltaTime)}");
        GUI.TextArea(new Rect(100, 150, 300, 50), $"MIN FPS: {MinFPS()}");
        GUI.TextArea(new Rect(100, 200, 300, 50), $"MAX FPS: {MaxFPS()}");
    }

    private int MinFPS()
    {
        if (_frames.Count == 0)
            return FPS(Time.deltaTime);
        return _frames.Min(FPS);
    }

    private int MaxFPS()
    {
        if (_frames.Count == 0)
            return FPS(Time.deltaTime);
        return _frames.Max(FPS);
    }

    private static int FPS(float deltaTime)
    {
        return Mathf.FloorToInt(1 / deltaTime);
    }
}