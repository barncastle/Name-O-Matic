using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NameOMatic.Extensions
{
    internal static class BinaryReaderExtensions
    {
        public static T Read<T>(this BinaryReader reader) where T : struct
        {
            byte[] result = reader.ReadBytes(Unsafe.SizeOf<T>());
            return Unsafe.ReadUnaligned<T>(ref result[0]);
        }

        public static string ReadString(this BinaryReader reader, int size)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(size));
        }

        public static T[] ReadArray<T>(this BinaryReader reader, int size) where T : struct
        {
            byte[] result = reader.ReadBytes(Unsafe.SizeOf<T>() * size);
            return result.CopyTo<T>();
        }

        public static unsafe T[] CopyTo<T>(this byte[] src) where T : struct
        {
            T[] result = new T[src.Length / Unsafe.SizeOf<T>()];

            if (src.Length > 0)
                Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref result[0]), Unsafe.AsPointer(ref src[0]), (uint)src.Length);

            return result;
        }
    }
}
