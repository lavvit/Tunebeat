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
            Joypad.Init();
        }
        public static void Update()
        {
            Key.Update();
            Mouse.Update();
            Joypad.Update();
            TextLog.Update();
        }
    }
}