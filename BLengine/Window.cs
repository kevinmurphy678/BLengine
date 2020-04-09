﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using static RenderingEngine.ShaderManager;

namespace RenderingEngine
{
    public class Window : GameWindow
    {
        ImGuiController _controller;
        public static int _Width = 1440;
        public static int _Height = 900;
        MeshComponent mesh1;
        double t = 0;
        Entity ent1;
        Texture tex;
        Texture tex2;
        Player player;

        public Window(GraphicsMode gMode) : base(_Width, _Height, gMode,
                                    "Legend286 and Boomer678's Rendering Engine",
                                    GameWindowFlags.Default,
                                    DisplayDevice.Default,
                                    4, 6, GraphicsContextFlags.ForwardCompatible)
        {
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ent1 = new Entity(new Vector3(0,0,0), new Quaternion(new Vector3(0,0,0)), new Vector3(1,1,1));
            mesh1 = new MeshComponent(ent1);
      
            tex = new Texture("Textures/test.png");
            tex2 = new Texture("Textures/testnormal.png");
            _controller = new ImGuiController(Width, Height);
            _controller.CreateSkin();
          
            player = new Player();
            
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Update the opengl viewport
            GL.Viewport(0, 0, Width, Height);

            // Tell ImGui of the new size
            _controller.WindowResized(Width, Height);
        }

    


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Util.TotalTime += (float)e.Time;  //TotalTime += deltaTime
            t += e.Time;
            base.OnRenderFrame(e);
            mesh1.mat.DiffuseColour = new Vector3((float)Math.Sin(t), (float)Math.Cos(t), (float)Math.Sin(t));

            _controller.Update(this, (float)e.Time);

            GL.ClearColor(new Color4(0.1f, 0.07f, 0.13f, 1.0f)); //pretty colors :^)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);


            #region Render Debug ImGUI
            ImGui.Begin("Render Debug");
            ImGui.SetWindowSize(new System.Numerics.Vector2(450, 365), ImGuiCond.Once);
            ImGui.Text("Shaders: " + ShaderManager.GetShaderCount());

            ImGui.Text("Camera Pos: " + player.GetCamera().Position);


            if (ImGui.Button("Change Shader DIFFUSE | NORMAL", new System.Numerics.Vector2(400, 32)))
            {
                mesh1.mat.shader = ShaderManager.get(ShaderType_BL.Default, ShaderFlags.LIT | ShaderFlags.USE_DIFFUSE_TEXTURE | ShaderFlags.USE_NORMAL_TEXTURE);
            }
            if (ImGui.Button("Change Shader DIFFUSE", new System.Numerics.Vector2(400, 32)))
            {
                mesh1.mat.shader = ShaderManager.get(ShaderType_BL.Default, ShaderFlags.USE_DIFFUSE_TEXTURE);
            }
            if (ImGui.Button("Change Shader NORMAL", new System.Numerics.Vector2(400, 32)))
            {
                mesh1.mat.shader = ShaderManager.get(ShaderType_BL.Default, ShaderFlags.USE_NORMAL_TEXTURE);
            }
            if (ImGui.Button("Change shader DEBUG_LIGHTING", new System.Numerics.Vector2(400, 32)))
            {
                mesh1.mat.shader = ShaderManager.get(ShaderType_BL.Default, ShaderFlags.DEBUG_LIGHTING | ShaderFlags.USE_NORMAL_TEXTURE);
            }
            if (ImGui.Button("Change Shader DEBUG_WORLDPOSITION", new System.Numerics.Vector2(400, 32)))
            {
                mesh1.mat.shader = ShaderManager.get(ShaderType_BL.Default, ShaderFlags.DEBUG_WORLDPOSITION);
            }
            if (ImGui.Button("Change Shader USE_NONE", new System.Numerics.Vector2(400, 32)))
            {
                mesh1.mat.shader = ShaderManager.get(ShaderType_BL.Default);
            }

            if (ImGui.Button("Quit", new System.Numerics.Vector2(100, 32)))
            {
                Exit();
            }
            #endregion


            #region Shader Live Coding
            ImGui.Begin("Shader Live Coding");
            ImGui.SetWindowSize(new System.Numerics.Vector2(530, 475), ImGuiCond.Once);
            if (ImGui.Button("Compile", new System.Numerics.Vector2(400, 32)))
            {
                mesh1.mat.shader = new Shader(mesh1.mat.shader.SourceCode_frag, mesh1.mat.shader.SourceCode_vert);
            }
            if (ImGui.CollapsingHeader("Fragment")){
                ImGui.InputTextMultiline("Fragment Shader", ref mesh1.mat.shader.SourceCode_frag, 4096, new System.Numerics.Vector2(800, 600));
            }
            if (ImGui.CollapsingHeader("Vertex"))
            {
                ImGui.InputTextMultiline("Vertex Shader", ref mesh1.mat.shader.SourceCode_vert, 4096, new System.Numerics.Vector2(800, 600));
            }
          
           
            ImGui.End(); 
            #endregion

            tex.UseTexture(TextureUnit.Texture0);
            tex2.UseTexture(TextureUnit.Texture1);


            player.GetCamera().ProcessInput();

            mesh1.Render();
            mesh1.mat.shader.BindMatrix4("model", mesh1.GetModelMatrix());
            mesh1.mat.shader.BindMatrix4("view", player.GetCamera().GetViewMatrix());
            mesh1.mat.shader.BindMatrix4("projection", player.GetCamera().GetProjectionMatrix());
            
            _controller.Render();
            
            Util.CheckGLError("End of frame");

            SwapBuffers();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            _controller.PressChar(e.KeyChar);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
