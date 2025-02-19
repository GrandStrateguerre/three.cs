﻿namespace Demo.examples
{
    using Demo.examples.cs.loaders;
    using Examples;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using ThreeCs.Cameras;
    using ThreeCs.Lights;
    using ThreeCs.Loaders;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Scenes;
    using ThreeCs.Textures;

    [Example("webgl_loader_obj", ExampleCategory.OpenTK, "loader")]
    class webgl_loader_obj : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private readonly Vector2 mouse = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(45, control.Width / (float)control.Height, 1, 2000);
            this.camera.Position.Z = 100;

            // scene

            scene = new Scene();

            scene.Add(camera);

            // light

            var ambient = new AmbientLight((Color)colorConvertor.ConvertFromString("#101030"));
            scene.Add(ambient);

            var directionalLight = new DirectionalLight((Color)colorConvertor.ConvertFromString("#ffeedd"));
            directionalLight.Position = new Vector3(0, 0, 1);
            scene.Add(directionalLight);

            // texture

            var manager = new LoadingManager();

            var texture = new Texture();

            var loader = new ImageLoader(manager);
            loader.Load(@"examples/textures/UV_Grid_Sm.jpg", image =>
            {
                texture.Image = image;
                texture.Format = ImageLoader.PixelFormatToThree(image.PixelFormat);
                texture.NeedsUpdate = true;
            });

            // model

            var modelLoader = new OBJLoader(manager);
            modelLoader.Load(@"examples\objs\male02\male02.obj", object3D =>
            {
                object3D.Traverse(child =>
                {
                    if (child is Mesh)
                    {
                        ((IMap)child.Material).Map = texture;
                    }
                });

                object3D.Position.Y = -80;
                scene.Add(object3D);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public override void Resize(Size clientSize)
        {
            Debug.Assert(null != this.camera);
            Debug.Assert(null != this.renderer);

            this.camera.Aspect = clientSize.Width / (float)clientSize.Height;
            this.camera.UpdateProjectionMatrix();

            this.renderer.Size = clientSize;

            //controls.handleResize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            this.mouse.X = (here.X - ((float)clientSize.Width / 2)) / 2;
            this.mouse.Y = (here.Y - ((float)clientSize.Height / 2)) / 2;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);

            camera.Position.X += (mouse.X - camera.Position.X) * .05f;
            camera.Position.Y += (-mouse.Y - camera.Position.Y) * .05f;

            camera.LookAt(scene.Position);

            renderer.Render(scene, camera);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Unload()
        {
            this.scene.Dispose();
            this.camera.Dispose();

            base.Unload();
        }
    }
}
