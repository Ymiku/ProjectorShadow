﻿using UnityEngine;
using System.Collections;
using Assets.Script.Utils;
using System.Collections.Generic;

public class ShadowProjector : MonoBehaviour 
{
    private Projector _projector;
    //
	[SerializeField]
    private Camera _lightCamera = null;
    private RenderTexture _shadowTex;
    //
    private Camera _mainCamera;
    private List<Renderer> _shadowCasterList = new List<Renderer>();
    private BoxCollider _boundsCollider;
    public float boundsOffset = 1;//边界偏移，
    public Shader shadowReplaceShader;
	void Start () 
    {
        _projector = GetComponent<Projector>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //
        if(_lightCamera == null)
        {
            _lightCamera = gameObject.AddComponent<Camera>();
            _lightCamera.orthographic = true;
			_lightCamera.cullingMask = ~LayerMask.GetMask("Terrain");
            _lightCamera.clearFlags = CameraClearFlags.SolidColor;
            _lightCamera.backgroundColor = new Color(1,1,1,0);
			_lightCamera.orthographicSize = _projector.orthographicSize;
            _shadowTex = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
            _shadowTex.filterMode = FilterMode.Bilinear;
			_shadowTex.wrapMode = TextureWrapMode.Clamp;
            _lightCamera.targetTexture = _shadowTex;
            _lightCamera.SetReplacementShader(shadowReplaceShader, "RenderType");
            _projector.material.SetTexture("_ShadowTex", _shadowTex);
			_projector.ignoreLayers = ~LayerMask.GetMask("Terrain");
        }
         

        _boundsCollider = new GameObject("Test use to show bounds").AddComponent<BoxCollider>();
	}

    void LateUpdate()
    {
        //求阴影产生物体的包围盒
        Bounds b = new Bounds();
        for (int i = 0; i < _shadowCasterList.Count; i++)
        {
            if(_shadowCasterList[i] != null)
            {
                b.Encapsulate(_shadowCasterList[i].bounds);
            }
        }
        b.extents += Vector3.one * boundsOffset;
#if UNITY_EDITOR
        _boundsCollider.center = b.center;
        _boundsCollider.size = b.size;
#endif
        //根据mainCamera来更新lightCamera和projector的位置，和设置参数
        //ShadowUtils.SetLightCamera(b, _lightCamera);
        //_projector.aspectRatio = _lightCamera.aspect;
        //_projector.orthographicSize = _lightCamera.orthographicSize;
        //_projector.nearClipPlane = _lightCamera.nearClipPlane;
        //_projector.farClipPlane = _lightCamera.farClipPlane;
	}
}
