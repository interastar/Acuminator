﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;

namespace ForceGraph
{
	/// <summary>
	/// Interaction logic for ForceGraphUserConrol.xaml
	/// </summary>
	public partial class ForceGraphUserConrol : UserControl
	{
		public ForceGraphUserConrol()
		{
			InitializeComponent();
		}


		OpenGL _gl;
		mat4 _projectionMatrix;
		mat4 _viewMatrix;
		mat4 _modelMatrix;

		ForceGraphScene _forceGraph;

		private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			_gl = args.OpenGL;
			_gl.ClearColor(0.2f, 0.2f, 0.2f, 0.0f);

			const float rads = (60.0f / 360.0f) * (float)Math.PI * 2.0f;
			_projectionMatrix = glm.perspective(rads, 800 / 600, 0.1f, 100.0f);
			_viewMatrix = glm.translate(new mat4(1.0f), new vec3(0.0f, 0.0f, -10.0f));
			_modelMatrix = glm.scale(new mat4(1.0f), new vec3(2.5f));

			_forceGraph = new ForceGraphScene();
			_forceGraph.Init(_gl);

			if (!(DataContext is ForceGraphViewModel forceGraphViewModel))
				return;

			forceGraphViewModel.ForceGraphScene = _forceGraph;
		}

		private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			_gl = args.OpenGL;
			_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

			_forceGraph.Render(_gl, _projectionMatrix, _viewMatrix, _modelMatrix);
		}

		private void OpenGLControl_Unloaded(object sender, RoutedEventArgs e)
		{
			_forceGraph.CleanUp(_gl);
		}
	}
}