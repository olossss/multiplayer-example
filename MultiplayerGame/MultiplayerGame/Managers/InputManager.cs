// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputManager.cs" company="">
//   
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiplayerGame.Managers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InputManager : GameComponent
    {
        #region Constants and Fields

        /// <summary>
        /// The resolution manager.
        /// </summary>
        private readonly ResolutionManager resolutionManager;

        /// <summary>
        /// The keyboard state.
        /// </summary>
        private KeyboardState keyboardState;

        /// <summary>
        /// The last keyboard state.
        /// </summary>
        private KeyboardState lastKeyboardState;

        /// <summary>
        /// The last mouse state.
        /// </summary>
        private MouseState lastMouseState;

        /// <summary>
        /// The mouse state.
        /// </summary>
        private MouseState mouseState;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputManager"/> class.
        /// </summary>
        /// <param name="game">
        /// The game.
        /// </param>
        /// <param name="resolutionManager">
        /// The resolution manager.
        /// </param>
        public InputManager(Game game, ResolutionManager resolutionManager)
            : base(game)
        {
            this.resolutionManager = resolutionManager;
            this.keyboardState = Keyboard.GetState();
            this.mouseState = Mouse.GetState();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets MousePosition.
        /// </summary>
        public Vector2 MousePosition
        {
            get
            {
                return this.resolutionManager.TransformToViewSpace(new Vector2(this.mouseState.X, this.mouseState.Y));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The flush.
        /// </summary>
        public void Flush()
        {
            this.lastKeyboardState = this.keyboardState;
            this.lastMouseState = this.mouseState;
        }

        /// <summary>
        /// The is key down.
        /// </summary>
        /// <param name="keyToTest">
        /// The key to test.
        /// </param>
        /// <returns>
        /// The is key down.
        /// </returns>
        public bool IsKeyDown(Keys keyToTest)
        {
            return this.keyboardState.IsKeyDown(keyToTest);
        }

        /// <summary>
        /// The is key pressed.
        /// </summary>
        /// <param name="keyToTest">
        /// The key to test.
        /// </param>
        /// <returns>
        /// The is key pressed.
        /// </returns>
        public bool IsKeyPressed(Keys keyToTest)
        {
            return this.keyboardState.IsKeyUp(keyToTest) && this.lastKeyboardState.IsKeyDown(keyToTest);
        }

        /// <summary>
        /// The is key released.
        /// </summary>
        /// <param name="keyToTest">
        /// The key to test.
        /// </param>
        /// <returns>
        /// The is key released.
        /// </returns>
        public bool IsKeyReleased(Keys keyToTest)
        {
            return this.keyboardState.IsKeyDown(keyToTest) && this.lastKeyboardState.IsKeyUp(keyToTest);
        }

        /// <summary>
        /// The left button is clicked.
        /// </summary>
        /// <returns>
        /// The left button is clicked.
        /// </returns>
        public bool LeftButtonIsClicked()
        {
            return this.mouseState.LeftButton == ButtonState.Released
                   && this.lastMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// The right button is clicked.
        /// </summary>
        /// <returns>
        /// The right button is clicked.
        /// </returns>
        public bool RightButtonIsClicked()
        {
            return this.mouseState.RightButton == ButtonState.Released
                   && this.lastMouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
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