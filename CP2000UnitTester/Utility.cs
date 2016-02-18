using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CityParser2000;
using System.IO;

namespace CP2000UnitTester
{
	[TestClass]
	class Utility
	{
		/// <summary>
		/// Testing the StreamUtils Read4ByteInt method with a standard stream of four bytes to make sure it works.
		/// </summary>
		[TestMethod]
		static void StandardParseStreamToInt()
		{
			//Lets make sure four bytes, 02 D7 9D E2 work out to ‭47685090‬.
			byte[] theByte = { 0x02, 0xD7, 0x9D, 0xE2 };
			int expectedInt = 47685090;
			Console.WriteLine("Sanity check: the byte array = int(32): " + BitConverter.ToInt32(theByte, 0));

			Stream stream = new MemoryStream(theByte);
			stream.Read4ByteInt();
		}
	}
}
