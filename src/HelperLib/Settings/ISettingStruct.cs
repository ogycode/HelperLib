/*
 * ISettingStruct.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

namespace Verloka.HelperLib.Settings
{
    /// <summary>
    /// Interface for saving custom struct and classes to register
    /// </summary>
    public interface ISettingStruct
    {
        /// <summary>
        /// Return all fields as string
        /// </summary>
        /// <returns>String line with all params</returns>
        string GetValue();
        /// <summary>
        /// Setting fields from input string
        /// </summary>
        /// <param name="value">String line with all params</param>
        void SetValue(string value);
    }
}
