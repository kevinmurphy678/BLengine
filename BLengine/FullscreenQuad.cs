﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using static RenderingEngine.ShaderManager;

namespace RenderingEngine
{
    class FullscreenQuad
    {
        public Shader shader;
        float[] vertices =
                           {
                            -1.0f,  1.0f, 0.0f,  0.0f, 1.0f,
                            -1.0f, -1.0f, 0.0f,  0.0f, 0.0f, 
                             1.0f, -1.0f, 0.0f,  1.0f, 0.0f,
                             1.0f,  1.0f, 0.0f,  1.0f, 1.0f,
                           };

        uint[] indices =
                         {
                          0, 1, 3,
                          1, 2, 3,
                         };

        int VertexBufferObject;
        int VertexArrayObject;
        int ElementBufferObject;

        public FullscreenQuad(ShaderType_BL type, ShaderFlags flags)
        {
            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();
            ElementBufferObject = GL.GenBuffer();
            Initialise(type, flags);
        }
        void Initialise(ShaderType_BL type, ShaderFlags flags)
        {
            shader = ShaderManager.get(type, flags);
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);

            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        public void Render()
        {
            shader.UseShader();
            shader.BindInt("GBufferDiffuse", 0);
            shader.BindInt("GBufferNormal", 1);
            shader.BindInt("GBufferSpecular", 2);
            shader.BindInt("GBufferDepth", 3);
            shader.BindInt("GBufferPosition", 4);
            shader.BindInt("LightingBuffer", 5);

            shader.BindMatrix4("view",CameraManager.GetActiveCamera().GetViewMatrix());
            shader.BindMatrix4("projection", CameraManager.GetActiveCamera().GetProjectionMatrix());

            DeferredRenderer.BindGBufferTextures();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        public void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(ElementBufferObject);
        }
    }
}
