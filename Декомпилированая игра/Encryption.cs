using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Encryption
{
	private const string INIT_VECTOR = "sayHiIfUReadThis";

	private const int KEY_SIZE = 256;

	public static string EncryptString(string pPlainText, string pPassPhrase)
	{
		byte[] tInitVectorBytes = Encoding.UTF8.GetBytes("sayHiIfUReadThis");
		byte[] tPlainTextBytes = Encoding.UTF8.GetBytes(pPlainText);
		using PasswordDeriveBytes tPassword = new PasswordDeriveBytes(pPassPhrase, null);
		byte[] tKeyBytes = tPassword.GetBytes(32);
		using RijndaelManaged tSymmetricKey = new RijndaelManaged
		{
			Mode = CipherMode.CBC
		};
		using ICryptoTransform tEncryptor = tSymmetricKey.CreateEncryptor(tKeyBytes, tInitVectorBytes);
		using MemoryStream tMemoryStream = new MemoryStream();
		using CryptoStream tCryptoStream = new CryptoStream(tMemoryStream, tEncryptor, CryptoStreamMode.Write);
		tCryptoStream.Write(tPlainTextBytes, 0, tPlainTextBytes.Length);
		tCryptoStream.FlushFinalBlock();
		return Convert.ToBase64String(tMemoryStream.ToArray());
	}

	public static string DecryptString(string pCipherText, string pPassPhrase)
	{
		byte[] tInitVectorBytes = Encoding.UTF8.GetBytes("sayHiIfUReadThis");
		byte[] tCipherTextBytes = Convert.FromBase64String(pCipherText);
		using PasswordDeriveBytes tPassword = new PasswordDeriveBytes(pPassPhrase, null);
		byte[] tKeyBytes = tPassword.GetBytes(32);
		using RijndaelManaged tSymmetricKey = new RijndaelManaged
		{
			Mode = CipherMode.CBC
		};
		using ICryptoTransform tDecryptor = tSymmetricKey.CreateDecryptor(tKeyBytes, tInitVectorBytes);
		using MemoryStream tMemoryStream = new MemoryStream(tCipherTextBytes);
		using CryptoStream tCryptoStream = new CryptoStream(tMemoryStream, tDecryptor, CryptoStreamMode.Read);
		byte[] tPlainTextBytes = new byte[tCipherTextBytes.Length];
		int tDecryptedByteCount = tCryptoStream.Read(tPlainTextBytes, 0, tPlainTextBytes.Length);
		return Encoding.UTF8.GetString(tPlainTextBytes, 0, tDecryptedByteCount);
	}
}
