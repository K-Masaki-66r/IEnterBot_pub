namespace IEnterBot
{
	internal class Person : IEquatable<Person>
	{
		internal string Name { get; private set; } = "";
		internal uint Times { get; private set; } = 0;
		internal bool InRoom { get; private set; } = false;

		internal Person() : this("")
		{
		}

		internal Person(string name) : this(name, 1, true)
		{
		}
		internal Person(string name, uint times) : this(name, times, true)
		{
		}
		internal Person(string name, uint times, bool inRoom)
		{
			Name = name;
			Times = times;
			InRoom = inRoom;
		}

		internal void Come()
		{
			if(InRoom) {
				return;
			}
			Times++;
			InRoom = true;
			return;
		}

		internal void Return()
		{
			if(!InRoom) {
				return;
			}
			InRoom = false;
			return;
		}

		/// <summary>
		/// 在室状況を変更する。
		/// </summary>
		/// <returns>現在の在室状況。</returns>
		internal bool Change()
		{
			if (!InRoom) {
				Times++;
				InRoom = true;
			} else {
				InRoom = false;
			}
			
			return InRoom;
		}

		public bool Equals(Person? other)
		{
			if(other == null) {
				return false;
			}

			return Name == other.Name;
		}

		public override bool Equals(object? obj)
		{
			if(obj is not Person p) {
				return false;
			}

			return Equals(p);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator ==(Person? p1, Person? p2)
		{
			if(Object.ReferenceEquals(p1, null)) {
				if(Object.ReferenceEquals(p2, null)) {
					return true;
				} else {
					return false;
				}
			}

			return p1.Equals(p2);
		}

		public static bool operator !=(Person? p1, Person? p2)
		{
			return !(p1 == p2);
		}
	}
}
