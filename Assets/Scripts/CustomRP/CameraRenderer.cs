﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRenderPipeline
{
    public partial class CameraRenderer
    {
        private ScriptableRenderContext _context;
        private Camera _camera;
        private CullingResults _cullingResult;
        
        private readonly CommandBuffer _commandBuffer = new CommandBuffer
            {name = bufferName};
        private const string bufferName = "Camera Render";
        
        private static readonly List<ShaderTagId> drawingShaderTagIds =
            new List<ShaderTagId>
            {
                new ShaderTagId("SRPDefaultUnlit"),
            };

        
        public void Render(ScriptableRenderContext context, Camera camera)
        {
            _camera = camera;
            _context = context;
            
            if(!Cull(out var parameters))
                return;
            
            Settings(parameters);
            DrawVisible();
            DrawUnsupportedShaders();
            DrawGizmos();
            Submit();
        }
        
        private void Settings(ScriptableCullingParameters parameters)
        {
            _cullingResult = _context.Cull(ref parameters);
            _context.SetupCameraProperties(_camera);
            _commandBuffer.ClearRenderTarget(true, true, Color.clear);
            _commandBuffer.BeginSample(bufferName);
            ExecuteCommandBuffer();
        }

        private void DrawVisible()
        {
            var drawingSettings = CreateDrawingSettings(drawingShaderTagIds, 
                SortingCriteria.CommonOpaque, out var sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref
                filteringSettings);
            _context.DrawSkybox(_camera);
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref
                filteringSettings);

        }
        
        private void Submit()
        {
            _commandBuffer.EndSample(bufferName);
            ExecuteCommandBuffer();
            _context.Submit();
        }
        
        private void ExecuteCommandBuffer()
        {
            _context.ExecuteCommandBuffer(_commandBuffer);
            _commandBuffer.Clear();
        }
        
        private bool Cull(out ScriptableCullingParameters parameters)
        {
            return _camera.TryGetCullingParameters(out parameters);
        }
        
        private DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTags, SortingCriteria
            sortingCriteria, out SortingSettings sortingSettings)
        {
            sortingSettings = new SortingSettings(_camera)
            {
                criteria = sortingCriteria,
            };
            var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
            for (var i = 1; i < shaderTags.Count; i++)
            {
                drawingSettings.SetShaderPassName(i, shaderTags[i]);
            }
            return drawingSettings;
        }
        
        partial void DrawGizmos()
        {
            if (!Handles.ShouldRenderGizmos())
            {
                return;
            }
            _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }
    
    partial class CameraRenderer
    {
        partial void DrawUnsupportedShaders();
#if UNITY_EDITOR
        private static readonly ShaderTagId[] _legacyShaderTagIds =
        {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };
        private static Material _errorMaterial = new
            Material(Shader.Find("Hidden/InternalErrorShader"));
        partial void DrawUnsupportedShaders()
        {
            var drawingSettings = new DrawingSettings(_legacyShaderTagIds[0], new
                SortingSettings(_camera))
            {
                overrideMaterial = _errorMaterial,
            };
            for (var i = 1; i < _legacyShaderTagIds.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, _legacyShaderTagIds[i]);
            }
            var filteringSettings = FilteringSettings.defaultValue;
            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref
                filteringSettings);
        }
#endif
        partial void DrawGizmos();
    }
}