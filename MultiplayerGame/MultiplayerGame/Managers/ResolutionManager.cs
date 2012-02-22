// -----------------------------------------------------------------------
// <copyright file="ResolutionManager.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MultiplayerGame.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ResolutionManager
    {
        #region Constants and Fields

        private GraphicsDeviceManager graphicsDeviceManager;

        private int height = 600;

        private bool isFullScreen;

        private bool matrixIsDirty = true;

        private Matrix scaleMatrix;

        private int vHeight = 768;

        private int vWidth = 1024;

        private int width = 800;

        #endregion

        #region Properties

        public Viewport DisplayViewport { get; private set; }

        public Vector2 VirtualResolution
        {
            get
            {
                return new Vector2(this.vWidth, this.vHeight);
            }
        }

        #endregion

        #region Public Methods

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

        public void FullViewport()
        {
            var vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = this.width;
            vp.Height = this.height;
            this.graphicsDeviceManager.GraphicsDevice.Viewport = vp;
        }

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
        /// <returns>aspect ratio</returns>
        public float GetVirtualAspectRatio()
        {
            return this.vWidth / (float)this.vHeight;
        }

        public void Init(ref GraphicsDeviceManager device)
        {
            this.width = device.PreferredBackBufferWidth;
            this.height = device.PreferredBackBufferHeight;
            this.graphicsDeviceManager = device;
            this.matrixIsDirty = true;
            this.ApplyResolutionSettings();
        }

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

        public void SetResolution(int newWidth, int newHeight, bool newFullScreen)
        {
            this.width = newWidth;
            this.height = newHeight;

            this.isFullScreen = newFullScreen;

            this.ApplyResolutionSettings();
        }

        public void SetVirtualResolution(int newWidth, int newHeight)
        {
            this.vWidth = newWidth;
            this.vHeight = newHeight;

            this.matrixIsDirty = true;
        }

        public Vector2 TransformToViewSpace(Vector2 windowCoordinate)
        {
            return Vector2.Transform(
                windowCoordinate - new Vector2(this.DisplayViewport.X, this.DisplayViewport.Y),
                Matrix.Invert(this.scaleMatrix));
        }

        #endregion

        #region Methods

        private void ApplyResolutionSettings()
        {
#if XBOX360
           this.isFullScreen = true;
#endif

            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (this.isFullScreen == false)
            {
                if ((this.width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) &&
                    (this.height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
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
