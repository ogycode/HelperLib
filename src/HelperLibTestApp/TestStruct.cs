namespace HelperLibTestApp
{
    public class TestStruct : Verloka.HelperLib.Settings.ISettingStruct
    {
        public int a, b, c, g;
        
        public TestStruct()
        {

        }
        public TestStruct(string str)
        {
            SetValue(str);
        }

        public string GetValue()
        {
            return $"{a}|{b}|{c}|{g}";
        }

        public void SetValue(string value)
        {
            var str = value.Split('|');

            a = getInt(str[0]);
            b = getInt(str[1]);
            c = getInt(str[2]);
            g = getInt(str[3]);
        }
        public static int getInt(string num)
        {
            int i;
            int.TryParse(num, out i);
            return i;
        }
    }
}
