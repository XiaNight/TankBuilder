using System.IO;
using System.IO.Compression;
using System.Text;

public static class StringCompression
{
	public static byte[] Compress(string text)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		using var msi = new MemoryStream(bytes);
		using var mso = new MemoryStream();
		using (var gs = new GZipStream(mso, CompressionMode.Compress))
		{
			msi.CopyTo(gs);
		}
		return mso.ToArray();
	}

	public static string Decompress(byte[] bytes)
	{
		using var msi = new MemoryStream(bytes);
		using var mso = new MemoryStream();
		using (var gs = new GZipStream(msi, CompressionMode.Decompress))
		{
			gs.CopyTo(mso);
		}
		return Encoding.UTF8.GetString(mso.ToArray());
	}
}