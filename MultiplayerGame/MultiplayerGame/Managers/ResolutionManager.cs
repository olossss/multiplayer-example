// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionManager.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Managers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ResolutionManager
    {
        #region Constants and Fields

        /// <summary>
        /// The graphics device manager.
        /// </summary>
        private GraphicsDeviceManager graphicsDeviceManager;

        /// <summary>
        /// The height.
        /// </summary>
        private int height = 600;

        /// <summary>
        /// The is full screen.
        /// </summary>
        private bool isFullScreen;

        /// <summary>
        /// The matrix is dirty.
        /// </summary>
        private bool matrixIsDirty = true;

        /// <summary>
        /// The scale matrix.
        /// </summary>
        private Matrix scaleMatrix;

        /// <summary>
        /// The v height.
        /// </summary>
        private int vHeight = 768;

        /// <summary>
        /// The v width.
        /// </summary>
        private int vWidth = 1024;

        /// <summary>
        /// The width.
        /// </summary>
        private int width = 800;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets DisplayViewport.
        /// </summary>
        public Viewport DisplayViewport { get; private set; }

        /// <summary>
        /// Gets VirtualResolution.
        /// </summary>
        public Vector2 VirtualResolution
        {
            get
            {
                return new Vector2(this.vWidth, this.vHeight);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sets the device to use the draw pump
        /// Sets correct aspect ratio
        /// </summary>
        public void BeginDraw()
        {
            // Start by reseting viewport to (0,0,1,1)
            this.FullViewport();

            // Clear to Black
            this.graphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            // Calculate Proper Viewport according to Aspect Ratio
            this.ResetViewport();

            // and clear that
            // This way we are gonna have black bars if aspect ratio requires it and
            // the clear color on the rest
            this.graphicsDeviceManager.GraphicsDevice.Clear(Color.Black);
        }

        /// <summary>
        /// The full viewport.
        /// </summary>
        public void FullViewport()
        {
            var vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = this.width;
            vp.Height = this.height;
            this.graphicsDeviceManager.GraphicsDevice.Viewport = vp;
        }

        /// <summary>
        /// The get transformation matrix.
        /// </summary>
        /// <returns>
        /// </returns>
        public Matrix GetTransformationMatrix()
        {
            if (this.matrixIsDirty)
            {
                this.RecreateScaleMatrix();
            }

            return this.scaleMatrix;
        }

        /// <summary>
        /// Get virtual Mode Aspect Ratio
        /// </summary>
        /// <returns>
        /// aspect ratio
        /// </returns>
        public float GetVirtualAspectRatio()
        {
            return this.vWidth / (float)this.vHeight;
        }

        /// <summary>
        /// The init.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public void Init(ref GraphicsDeviceManager device)
        {
            this.width = device.PreferredBackBufferWidth;
            this.height = device.PreferredBackBufferHeight;
            this.graphicsDeviceManager = device;
            this.matrixIsDirty = true;
            this.ApplyResolutionSettings();
        }

        /// <summary>
        /// The reset viewport.
        /// </summary>
        public void ResetViewport()
        {
            float targetAspectRatio = this.GetVirtualAspectRatio();

            // figure out the largest area that fits in this resolution at the desired aspect ratio
            int newWidth = this.graphicsDeviceManager.PreferredBackBufferWidth;
            var newHeight = (int)(newWidth / targetAspectRatio + .5f);
            bool changed = false;

            if (newHeight > this.graphicsDeviceManager.PreferredBackBufferHeight)
            {
                newHeight = this.graphicsDeviceManager.PreferredBackBufferHeight;

                // PillarBox
                newWidth = (int)(newHeight * targetAspectRatio + .5f);
                changed = true;
            }

            // set up the new viewport centered in the backbuffer
            this.DisplayViewport = new Viewport
                {
                    X = (this.graphicsDeviceManager.PreferredBackBufferWidth / 2) - (newWidth / 2), 
                    Y = (this.graphicsDeviceManager.PreferredBackBufferHeight / 2) - (newHeight / 2), 
                    Width = newWidth, 
                    Height = newHeight, 
                    MinDepth = 0, 
                    MaxDepth = 1
                };

            if (changed)
            {
                this.matrixIsDirty = true;
            }

            this.graphicsDeviceManager.GraphicsDevice.Viewport = this.DisplayViewport;
        }

        /// <summary>
        /// The set resolution.
        /// </summary>
        /// <param name="newWidth">
        /// The new width.
        /// </param>
        /// <param name="newHeight">
        /// The new height.
        /// </param>
        /// <param name="newFullScreen">
        /// The new full screen.
        /// </param>
        public void SetResolution(int newWidth, int newHeight, bool newFullScreen)
        {
            this.width = newWidth;
            this.height = newHeight;

            this.isFullScreen = newFullScreen;

            this.ApplyResolutionSettings();
        }

        /// <summary>
        /// The set virtual resolution.
        /// </summary>
        /// <param name="newWidth">
        /// The new width.
        /// </param>
        /// <param name="newHeight">
        /// The new height.
        /// </param>
        public void SetVirtualResolution(int newWidth, int newHeight)
        {
            this.vWidth = newWidth;
            this.vHeight = newHeight;

            this.matrixIsDirty = true;
        }

        /// <summary>
        /// The transform to view space.
        /// </summary>
        /// <param name="windowCoordinate">
        /// The window coordinate.
        /// </param>
        /// <returns>
        /// </returns>
        public Vector2 TransformToViewSpace(Vector2 windowCoordinate)
        {
            return Vector2.Transform(
                windowCoordinate - new Vector2(this.DisplayViewport.X, this.DisplayViewport.Y), 
                Matrix.Invert(this.scaleMatrix));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The apply resolution settings.
        /// </summary>
        private void ApplyResolutionSettings()
        {
#if XBOX360
           this.isFullScreen = true;
#endif

            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (this.isFullScreen == false)
            {
                if ((this.width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (this.height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    this.graphicsDeviceManager.PreferredBackBufferWidth = this.width;
                    this.graphicsDeviceManager.PreferredBackBufferHeight = this.height;
                    this.graphicsDeviceManager.IsFullScreen = this.isFullScreen;
                    this.graphicsDeviceManager.ApplyChanges();
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate through the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == this.width) && (dm.Height == this.height))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        this.graphicsDeviceManager.PreferredBackBufferWidth = this.width;
                        this.graphicsDeviceManager.PreferredBackBufferHeight = this.height;
                        this.graphicsDeviceManager.IsFullScreen = this.isFullScreen;
                        this.graphicsDeviceManager.ApplyChanges();
                        break;
                    }
                }
            }

            this.matrixIsDirty = true;

            this.width = this.graphicsDeviceManager.PreferredBackBufferWidth;
            this.height = this.graphicsDeviceManager.PreferredBackBufferHeight;
        }

        /// <summary>
        /// The recreate scale matrix.
        /// </summary>
        private void RecreateScaleMatrix()
        {
            this.matrixIsDirty = false;
            this.scaleMatrix =
                Matrix.CreateScale(
                    (float)this.graphicsDeviceManager.GraphicsDevice.Viewport.Width / this.vWidth, 
                    (float)this.graphicsDeviceManager.GraphicsDevice.Viewport.Width / this.vWidth, 
                    1f);
        }

        #endregion
    }
}