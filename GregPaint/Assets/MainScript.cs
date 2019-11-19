using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    private RenderTexture textureA;
    private RenderTexture textureB;

    public Material StrokeProcessingMat;
    public Material DisplayMaterial;
    public MeshCollider CanvasCollider;

    public Color CurrentColor;
    public float CurrentWeight;

    private StrokePoint lastStrokePoint;
    private static readonly int StartPosId = Shader.PropertyToID("_StartPos");
    private static readonly int EndPosId = Shader.PropertyToID("_EndPos");
    private static readonly int StartColorId = Shader.PropertyToID("_StartColor");
    private static readonly int EndColorId = Shader.PropertyToID("_EndColor");
    private static readonly int StartWeightId = Shader.PropertyToID("_StartWeight");
    private static readonly int EndWeightId = Shader.PropertyToID("_EndWeight");

    private void Start()
    {
        textureA = new RenderTexture(2048, 2048, 1);
        textureB = new RenderTexture(2048, 2048, 1);
        textureB.enableRandomWrite = true;
    }

    private void OnDestroy()
    {
        textureA.Release();
        textureB.Release();
    }

    private void Update()
    {
        StrokePoint strokePoint = GetStrokePoint();
        RenderStroke(strokePoint, lastStrokePoint);

        lastStrokePoint = strokePoint;
        DisplayMaterial.SetTexture("_MainTex", textureA);
    }

    private void RenderStroke(StrokePoint start, StrokePoint end)
    {
        SetMaterialProperties(start, end);
        Graphics.Blit(textureA, textureB, StrokeProcessingMat);
        Graphics.Blit(textureB, textureA);
    }

    private void SetMaterialProperties(StrokePoint start, StrokePoint end)
    {
        StrokeProcessingMat.SetVector(StartPosId, start.Pos);
        StrokeProcessingMat.SetVector(EndPosId, end.Pos);
        StrokeProcessingMat.SetColor(StartColorId, start.Color);
        StrokeProcessingMat.SetColor(EndColorId, end.Color);
        StrokeProcessingMat.SetFloat(StartWeightId, start.Weight);
        StrokeProcessingMat.SetFloat(EndWeightId, end.Weight);
    }

    private StrokePoint GetStrokePoint()
    {
        Vector2 pos = GetStrokePosition();
        float weight = GetStrokeWeight();
        return new StrokePoint(pos, weight, CurrentColor);
    }

    private float GetStrokeWeight()
    {
        if(Input.touchCount > 0)
        {
            return Input.GetTouch(0).pressure * CurrentWeight;
        }
        return 0;
    }

    private Vector2 GetStrokePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = new Ray(mousePos, Vector3.forward);
        RaycastHit hit;
        if (CanvasCollider.Raycast(ray, out hit, float.PositiveInfinity))
        {
            return hit.textureCoord;
        }
        return Vector2.zero;
    }

    private struct StrokePoint
    {
        public Vector2 Pos { get; }
        public float Weight { get; }
        public Color Color { get; }

        public StrokePoint(Vector2 pos, float weight, Color color)
        {
            Pos = pos;
            Weight = weight;
            Color = color;
        }
    }
}
