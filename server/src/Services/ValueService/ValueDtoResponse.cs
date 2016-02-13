using System;
using System.IO;
using System.Text;
using ServiceStack.Service;

namespace Server.Services.ValueService
{
	public class ValueDtoResponse //: IStreamWriter
	{
		public ValueDtoResponse ()
		{
		}

		public ValueDtoResponse (ValueDto valueDto)
		{
			Key = valueDto.Key;
			Data = valueDto.Data;
		}

		public string Key { get; set; }
		// TODO: Change to support stream
		public string Data { get; set; }

		/*public void WriteTo (Stream responseStream)
		{
			UnicodeEncoding uniEncoding = new UnicodeEncoding ();
			var unicodeDataBytes = uniEncoding.GetBytes (Value);

			responseStream.Write (unicodeDataBytes, 0, unicodeDataBytes.Length);
		}*/
	}
}

