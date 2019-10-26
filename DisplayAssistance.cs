using System;
using System.Text;

namespace MyLittleTeamspeakServerQuery
{
    public static class DisplayAssistance
    {
        /// <summary>
        /// Gets the Hex values of a given byte array.
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString().ToUpper();
        }

        /// <summary>
        /// Gets the original byte values as a string represantation.
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string ByteToInt(byte[] f)
        {
            string _byterepresantation = "";
            for (int i = 0; i < f.Length; i++)
            {
                _byterepresantation += f[i].ToString() + " ";
            }
            return _byterepresantation;
        }

        /// <summary>
        /// Print's a byte signature string
        /// </summary>
        /// <param name="bytes"></param>
        public static void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i == bytes.Length - 1)
                    sb.Append(bytes[i]);
                else
                    sb.Append(bytes[i] + ", ");
            }
            sb.Append("}");
            //Console.WriteLine(sb.ToString());
        }
    }
}
