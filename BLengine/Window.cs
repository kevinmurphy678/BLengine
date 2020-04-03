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

namespace Dear_ImGui_Sample
{
    public class Window : GameWindow
    {
        ImGuiController _controller;

        public Window(GraphicsMode gMode) : base(1600, 900, gMode,
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

            _controller = new ImGuiController(Width, Height);
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

            base.OnRenderFrame(e);

            _controller.Update(this, (float)e.Time);

            GL.ClearColor(new Color4((float)Math.Sin(Util.TotalTime), (float)Math.Cos(Util.TotalTime)*2, 0.3f, 1)); //pretty colors :^)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //ImGui.ShowDemoWindow(); // remove the demo window
            ImGui.Begin("Window Test");
            ImGui.SetWindowSize(new System.Numerics.Vector2(500, 500));
            if(ImGui.Button("Quit", new System.Numerics.Vector2(100,100)))
            {
                base.Exit();
            }

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