namespace MultiplayerGame.RandomNumbers
{
    public interface IRandomNumberGenerator
    {
        #region Public Methods

        int Next(int maxValue);

        int Next(int minValue, int maxValue);

        #endregion
    }
}