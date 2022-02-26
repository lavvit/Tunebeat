namespace SeaDrop
{
    /// <summary>
    /// SeaDrop クラス
    /// </summary>
    public static class SeaDrop
    {
        public static void Init()
        {
            TextLog.Init();
        }
        public static void Update()
        {
            Key.Update();
            Mouse.Update();
            TextLog.Update();
        }
    }
}