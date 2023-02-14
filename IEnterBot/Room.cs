namespace IEnterBot
{
	internal delegate void RoomStateChangedEvent(bool isOpen);

	internal class Room
	{
		internal Bytes MasterUid { get; private set; }
		internal Person Master { get; private set; }
		internal Dictionary<Bytes, Person> Members { get; private set; }
		private bool _isOpen = false;
		private uint _countInRoom = 0;
		private bool boot = true;

		internal Room() : this(new(), new(), new())
		{
		}

		internal Room(Bytes masterUid, Person master, Dictionary<Bytes, Person> members) : this(masterUid, master, members, false)
		{
		}

		internal Room(Bytes masterUid, Person master, Dictionary<Bytes, Person> members, bool isOpen)
		{
			MasterUid = masterUid;
			Master = master;
			IsOpen = isOpen;
			Members = members;
			foreach(var p in Members) {
				if(p.Value.InRoom) {
					CountInRoom++;
				}
			}

			boot = false;
		}

		internal event RoomStateChangedEvent RoomStateChanged = (isOpen) => { };

		internal Person this[Bytes uid] {
			get {
				if(uid == MasterUid) {
					return Master ?? new Person();
				}
				return Members[uid];
			}
		}

		internal bool IsOpen {
			get {
				return _isOpen; 
			}
			set {
				if(IsOpen == value) {
					return;
				}
				_isOpen = value;

				if(!boot) {
					RoomStateChanged(_isOpen);
				}
			}
		}

		internal uint CountInRoom {
			get {
				return _countInRoom;
			}
			private set {
				if(IsOpen) {
					if(value == 0) {
						IsOpen = false;
					}
				} else {
					if(value > 0) {
						IsOpen = true;
					}
				}
				_countInRoom = value;
			}
		}

		internal bool ChangeMaster(Bytes uid, Person master)
		{
			if(Members.ContainsKey(uid)) {
				return false;
			}

			MasterUid = uid;
			Master = master;
			return true;
		}

		internal bool Add(Bytes uid, Person person)
		{
			if(ContainsUid(uid)) {
				return false;
			}

			Members.Add(uid, person);
			if(person.InRoom) {
				CountInRoom++;
			}
			return true;
		}

		internal bool Remove(Bytes uid)
		{
			if(!Members.ContainsKey(uid)) {
				return false;
			}

			if(Members[uid].InRoom) {
				CountInRoom--;
			}
			Members.Remove(uid);
			
			return true;
		}

		internal bool ContainsUid(Bytes uid)
		{
			return MasterUid == uid | Members.ContainsKey(uid);
		}

		/// <summary>
		/// UIDがマスターのものであれば、部屋の開室状況を変更する。
		/// メンバーのものであれば、そのメンバーの所在状況を変更する。
		/// いずれでもなければなにもしない。
		/// </summary>
		/// <param name="uid">かざされたICカードのUID。</param>
		/// <returns>有効なUIDであったかどうか。</returns>
		internal bool ChangeState(Bytes uid)
		{
			if(uid == MasterUid) {
				if(IsOpen) {
					foreach(var p in Members) {
						p.Value.Return();
					}
					CountInRoom = 0;
					IsOpen = false;
				} else {
					IsOpen = true;
				}

				return true;
			}

			if(Members.ContainsKey(uid)) {
				var inRoom = Members[uid].Change();
				if(inRoom) {
					CountInRoom++;
				} else {
					CountInRoom--;
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// 来訪回数によってメンバーをソートする
		/// </summary>
		internal void Sort()
		{
			Members = Members.OrderByDescending(selector => {
				return selector.Value.Times;
			}).ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		public override string ToString()
		{
			var result = "";
			
			result += this.IsOpen ? "開室中" : "閉室中";
			result += "\n\n";

			var memberIn = "__在室__\n";
			var someoneIn = false;
			var memberOut = "__不在__\n";
			var someoneOut = false;

			foreach(var member in Members) {
				var name = member.Value.Name;
				var inRoom = member.Value.InRoom;
				var times = member.Value.Times;
				if(inRoom) {
					memberIn += $"{name}：{times}回\n";
					if(!someoneIn) {
						someoneIn = true;
					}
				} else {
					memberOut += $"{name}：{times}回\n";
					if(!someoneOut) {
						someoneOut = true;
					}
				}
			}

			if(someoneIn) {
				result += memberIn;
				result += "\n";
			}
			if(someoneOut) {
				result += memberOut;
			}

			return result;
		}
	}
}
