﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RenderingEngine
{
    class Material
    {
        public Shader shader;
        public Vector3 DiffuseColour = new Vector3(1.0f, 1.0f, 0.0f);
        Texture DiffuseMap;
        bool UseDiffuse = false;
        Texture NormalMap;
        bool UseNormal = false;
        public ShaderManager.ShaderFlags flags = 0;
        public MeshComponent owner;
        public Material(string diffuseTexture, string normalTexture, MeshComponent m)
        {
            owner = m;

            if(diffuseTexture != "")
            {
                UseDiffuse = true;
            }

            if(normalTexture != "")
            {
                UseNormal = true;
            }

            if(UseDiffuse)
            {
                DiffuseMap = new Texture(diffuseTexture);
                flags = ShaderManager.ShaderFlags.USE_DIFFUSE_TEXTURE;
            }

            if(UseNormal)
            {
                NormalMap = new Texture(normalTexture);
                flags = ShaderManager.ShaderFlags.USE_NORMAL_TEXTURE;
            }

            if(UseDiffuse && UseNormal)
            {
                flags = ShaderManager.ShaderFlags.USE_DIFFUSE_TEXTURE | ShaderManager.ShaderFlags.USE_NORMAL_TEXTURE;
            }

            shader = ShaderManager.get(ShaderManager.ShaderType_BL.Default, flags);
        }

        public void RenderMaterial()
        {
            shader.UseShader(); //Set shader
            shader.BindVector3("DiffuseColour", DiffuseColour);
            if (UseDiffuse)
            {
                DiffuseMap.UseTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
                //flags = ShaderManager.ShaderFlags.USE_DIFFUSE_TEXTURE;
                shader.BindInt("diffuseTexture", 0);
            }

            if (UseNormal)
            {
                NormalMap.UseTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture1);
                //flags = ShaderManager.ShaderFlags.USE_NORMAL_TEXTURE;
                shader.BindInt("normalTexture", 1);
            }
        }
    }
}