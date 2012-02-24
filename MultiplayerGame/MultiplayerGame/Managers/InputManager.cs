// -----------------------------------------------------------------------
// <copyright file="InputManager.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerGame.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InputManager : GameComponent
    {
        #region Constants and Fields

        private readonly ResolutionManager resolutionManager;

        private KeyboardState keyboardState;

        private KeyboardState lastKeyboardState;

        private MouseState lastMouseState;

        private MouseState mouseState;

        #endregion

        #region Constructors and Destructors

        public InputManager(Game game, ResolutionManager resolutionManager)
            : base(game)
        {
            this.resolutionManager = resolutionManager;
            this.keyboardState = Keyboard.GetState();
            this.mouseState = Mouse.GetState();
        }

        #endregion

        #region Properties

        public Vector2 MousePosition
        {
            get
            {
                return this.resolutionManager.TransformToViewSpace(new Vector2(this.mouseState.X, this.mouseState.Y));
            }
        }

        #endregion

        #region Public Methods

        public void Flush()
        {
            this.lastKeyboardState = this.keyboardState;
            this.lastMouseState = this.mouseState;
        }

        public bool IsKeyDown(Keys keyToTest)
        {
            return this.keyboardState.IsKeyDown(keyToTest);
        }

        public bool IsKeyPressed(Keys keyToTest)
        {
            return this.keyboardState.IsKeyUp(keyToTest) && this.lastKeyboardState.IsKeyDown(keyToTest);
        }

        public bool IsKeyReleased(Keys keyToTest)
        {
            return this.keyboardState.IsKeyDown(keyToTest) && this.lastKeyboardState.IsKeyUp(keyToTest);
        }

        public bool LeftButtonIsClicked()
        {
            return this.mouseState.LeftButton == ButtonState.Released &&
                   this.lastMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool RightButtonIsClicked()
        {
            return this.mouseState.RightButton == ButtonState.Released &&
                   this.lastMouseState.RightButton == ButtonState.Pressed;
        }

        public override void Update(GameTime gameTime)
        {
            this.Flush();

            this.keyboardState = Keyboard.GetState(PlayerIndex.One);
            this.mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        #endregion
    }
}
