using System.Runtime.InteropServices;

namespace Verloka.HelperLib.Settings
{
    [Guid("c441ec2a-76e5-4442-a4ac-4a63a0c4cc21")]
    [CoClass(typeof(SettingStruct))]
    [ComImport]
    public interface ISettingStruct
    {
        string GetValue();
        void SetValue(string value);
    }
}
