using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MyLittleTeamspeakServerQuery
{
    public enum ParseType
    {
        Serverinfo,
        ServerGroupList,
        ClientList,
        ClientInfo,
        ChannelList,
        ChannelInfo
    }
    class Parser
    {

        /// <summary>
        /// Parses an array of byte chunks to keyvalue pairs and assigns the object members
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="bits"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T Parse<T>(Type t, byte[][] bits, object obj)
        {

            PropertyInfo[] properties = t.GetProperties();
            for (int i = 0; i < bits.Length; i++)
            {
                byte[][] halfsplit = HalfSplitByte(bits[i], (int)0x3D).ToArray();
                string val = Encoding.UTF8.GetString(TrimEnd(halfsplit[1]));
                string name = Encoding.UTF8.GetString(TrimEnd(halfsplit[0]));
                if (properties.Where(x => x.Name == name).Any())
                {
                    PropertyInfo p = properties.Where(x => x.Name == name).First();
                    if (val != null)
                        p.SetValue(obj, TConverter.ChangeType(p.PropertyType, val));
                    else
                        p.SetValue(obj, TConverter.ChangeType(p.PropertyType, null));
                }



            }

            return (T)obj;
        }

        /// <summary>
        /// Splits an given array of bytes by the given byte address and returns an ienumerable of type byte[][]
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static IEnumerable<byte[]> ByteToChunks(byte[] bytes, int address)
        {
            if (bytes.Length == 0)
                return null;
            List<byte[]> chunks = new List<byte[]>();


            int enter = 0;
            for (int i = 0; i < bytes.Length; i++)
            {

                if (bytes[i] == address)
                {

                    byte[] entry = new byte[i - enter];
                    if ((i - enter) > bytes.Length)
                        Buffer.BlockCopy(bytes, enter, entry, 0, i - (enter - 1));
                    else
                        Buffer.BlockCopy(bytes, enter, entry, 0, i - enter);
                    chunks.Add(entry);
                    enter = i + 1;
                }
            }
            //Console.WriteLine(enter + " " + bytes.Length);
            if (enter < bytes.Length)
            {
                byte[] last = new byte[bytes.Length - enter];
                Buffer.BlockCopy(bytes, enter, last, 0, bytes.Length - enter);
                chunks.Add(last);
            }

            return chunks.ToArray();
        }


        /// <summary>
        /// Get first occurance of a byte and split the given array at the
        /// position the address has been found
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static IEnumerable<byte[]> HalfSplitByte(byte[] bytes, int address)
        {
            int half = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == address)
                {
                    half = i;
                    break;
                }
            }
            if (half == 0)
                return new byte[][] { bytes, new byte[] { 0x00 } };

            byte[] key = new byte[half];
            byte[] value = new byte[bytes.Length - half];
            Array.Copy(bytes, 0, key, 0, half);
            Array.Copy(bytes, half + 1, value, 0, value.Length - 1);
            return new byte[][] { key, value };
        }




        /// <summary>
        /// Returns true if the seperator is present in a byte array.
        /// </summary>
        /// <param name="check"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static bool IsArray(byte[] check, int seperator)
        {
            for (int i = 0; i < check.Length; i++)
            {
                if (check[i] == seperator)
                    return true;
            }
            return false;
            //return check.Where(x => x == seperator).Any();
            //return false;
        }

        /// <summary>
        /// Returns true if the given length of B doesn't exceed the length of A
        /// and each byte of B is consistent with each byte of A.
        /// B can be longer than A but the byte signature must be consistent.
        /// </summary>
        /// <param name="a">Original byte array</param>
        /// <param name="b">The array to compare to</param>
        /// <returns></returns>
        public static bool ByteCompare(byte[] a, byte[] b)
        {
            if (a.Length < b.Length)
                return false;
            for (int i = 0; i < b.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Removes all the trailing 0
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }

        /// <summary>
        /// Removes the signature from the beginning.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] RemoveSignature(byte[] original, int length)
        {
            return original.Skip(length).ToArray();
        }

        /// <summary>
        /// Absolute comparison of two byte arrays. Length and bytes must be consistent.
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);
        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        [Obsolete("Method SplitByArray is deprecated, please use ByteToChunks instead.")]
        /// <summary>
        /// Obsolete
        /// </summary>
        /// <param name="source"></param>
        /// <param name="marker"></param>
        /// <returns></returns>
        public static IEnumerable<Byte[]> SplitByteArray(IEnumerable<Byte> source, byte marker)
        {
            if (null == source)
                throw new ArgumentNullException("source");

            List<byte> current = new List<byte>();

            foreach (byte b in source)
            {
                if (b == marker)
                {
                    if (current.Count > 0)
                        yield return current.ToArray();

                    current.Clear();
                }

                current.Add(b);
            }

            if (current.Count > 0)
                yield return current.ToArray();
        }

        /// <summary>
        /// returns the md5 sum of input as string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Unescapes a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UnescapeString(string input)
        {
            return input.Replace(@"\\", @"\").Replace(@"\/", "/").Replace(@"\s", " ").Replace(@"\0", "");
        }

    }
}
