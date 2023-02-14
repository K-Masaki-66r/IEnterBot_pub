using System.Globalization;

namespace IEnterBot
{
	internal class Bytes : IEquatable<Bytes>
	{
		private byte[] _bytes = Array.Empty<byte>();
		private int _length = 0;

		internal Bytes()
		{
		}

		internal Bytes(Byte[] bytes)
		{
			_bytes = bytes;
			_length = bytes.Length;
		}

		internal byte this[int index] {
			get {
				if(index < 0 || index >= _bytes.Length) {
					throw new ArgumentOutOfRangeException("index");
				}
				return _bytes[index];
			}
		}

		internal int Length {
			get {
				return _length;
			}
		}

		public bool Equals(Bytes? bytes)
		{
			if(bytes == null) {
				return false;
			}

			if(Object.ReferenceEquals(this, bytes)) {
				return true;
			}

			if(this.Length != bytes.Length) {
				return false;
			}

			for(var i = 0; i < this.Length; i++) {
				if(this[i] != bytes[i]) {
					return false;
				}
			}

			return true;
		}

		public override bool Equals(Object? obj)
		{
			return Equals(obj as Bytes);
		}

		public override int GetHashCode()
		{
			int result = 0;
			foreach(var b in _bytes) {
				result ^= b.GetHashCode();
			}
			return result;
		}

		public static bool operator ==(Bytes? b1, Bytes? b2)
		{
			if(ReferenceEquals(b1, null)) {
				if(ReferenceEquals(b2, null)) {
					return true;
				} else {
					return false;
				}
			}
			return b1.Equals(b2);
		}

		public static bool operator !=(Bytes b1, Bytes b2)
		{
			return !(b1 == b2);
		}

		public override string ToString()
		{
			return BitConverter.ToString(_bytes);
		}

		internal static Bytes GetBytes(string bytesStr)
		{
			var splited = bytesStr.Split('-');
			var result = new byte[splited.Length];

			for(int i = 0; i < splited.Length; i++) {
				if(!byte.TryParse(splited[i], NumberStyles.AllowHexSpecifier, null, out var num)) {
					return new();
				}
				result[i] = num;
			}

			return new(result);
		}
	}
}
