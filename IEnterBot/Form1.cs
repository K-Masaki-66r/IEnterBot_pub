using PCSC.Monitoring;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;

namespace IEnterBot
{
	internal partial class Form1 : Form
	{
		private Room room = new();
		private MyDiscord discordClient = new();
		private IC ic = new();
		private NotifyIcon notifyIcon = new();
		private Icon openyIcon;
		private Icon closeyIcon;
		
		private const string PATH = @".\.settings";
		private readonly Color RED = Color.FromArgb(240, 96, 96);
		private readonly Color BLUE = Color.FromArgb(30, 144, 255);

		internal Form1(IC ic)
		{
			room = Initialize();
			_ = PostStateDiscord();
			this.ic = ic;

			SystemEvents.SessionEnding += (s, e) => {
				Save();
				Application.Exit();
			};

			ShowInTaskbar = false;
			Visible = true;
			InitializeComponent();
			SetComponents();

			room.RoomStateChanged += (isOpen) => { RoomStateChanged(isOpen); };
			ic.CardInserted += (sender, e) => { Inserted(e); };
			ic.ReaderExceptionOccured += () => {
				Save();
				MessageBox.Show("カードリーダーに以上が発生したため、終了します。");
				Application.Exit();
			};
			
			if(room.Master == new Person()) {
				var form = new Form2(room, ic, Mode.SetMaster, this);
				form.ShowDialog();
			}

			var i = 0;
			foreach(var item in room.Members) {
				AddRow(item.Key, item.Value);
				if(item.Value.InRoom) {
					CellColorChange(dataGridView1.Rows[i], Color.White);
				} else {
					CellColorChange(dataGridView1.Rows[i], RED);
				}
				i++;
			}
			
			ic.Start();

			ActiveControl = AddButton;
		}

		private void SetComponents()
		{
			notifyIcon = new();
			notifyIcon.Visible = true;
			notifyIcon.Text = "IEnterBot";

			openyIcon = new(@".\images\open.ico");
			closeyIcon = new(@".\images\close.ico");


			var contextMenuStrip = new ContextMenuStrip();
			var toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Text = "終了";
			toolStripMenuItem.Click += ToolStripMenuItem_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem);
			notifyIcon.ContextMenuStrip = contextMenuStrip;

			notifyIcon.Click += NotifyIcon_Click;

			if(room.IsOpen) {
				textBox2.BackColor = BLUE;
				notifyIcon.Icon = openyIcon;
			} else {
				textBox2.BackColor = RED;
				notifyIcon.Icon = closeyIcon;
			}
		}

		private void ToolStripMenuItem_Click(object? sender, EventArgs e)
		{
			Application.Exit();
		}

		private void NotifyIcon_Click(object? sender, EventArgs e)
		{
			if(e is not MouseEventArgs me) {
				return;
			}
			switch(me.Button) {
				case MouseButtons.Left:
					Visible = !Visible;
					break;
				default:
					break;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(e.CloseReason != CloseReason.ApplicationExitCall) {
				e.Cancel = true;
				Visible = false;
				return;
			}

			Exit();
		}

		private void changeMasterButton_Click(object sender, EventArgs e)
		{
			var form = new Form2(room, ic, Mode.ChangeMaster, this);
			form.ShowDialog();
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			var form = new Form2(room, ic, Mode.Add, this);
			form.ShowDialog();
		}

		private void RemoveButton_Click(object sender, EventArgs e)
		{
			var form = new Form2(room, ic, Mode.Remove, this);
			form.ShowDialog();
		}

		private void Form1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)Keys.Escape) {
				Close();
			}
		}

		private void TweetButton_Click(object sender, EventArgs e)
		{
			if(textBox1.Text == "") {
				MessageBox.Show("ツイート文を入力してください。");
				return;
			}
			
			var statusCode = Twitter.Tweet(textBox1.Text);
			switch(statusCode) {
				case 0:
					textBox1.Text = "";
					break;
				case 403:
					MessageBox.Show("すでに同じツイートがされています。");
					break;
				default:
					MessageBox.Show("予期せぬエラーが発生しました。");
					break;
			}
		}

		private async void PostButton_Click(object sender, EventArgs e)
		{
			var text = textBox1.Text;
			if(text == "") {
				MessageBox.Show("投稿文を入力してください。");
				return;
			}


			await discordClient.Post(text);
			var statusCode = Twitter.Tweet(text);
			switch(statusCode) {
				case 0:
					textBox1.Text = "";
					break;
				case 403:
					MessageBox.Show("すでに同じツイートがされています。");
					break;
				default:
					MessageBox.Show("予期せぬエラーが発生しました。");
					break;
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			Save();
		}

		internal void AddRow(Bytes uid, Person person)
		{
			var row = new string[] {
					person.Name,
					person.Times.ToString(),
					uid.ToString(),
				};
			dataGridView1.Rows.Add(row);
			dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.Black;
			dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.SelectionForeColor = Color.Black;
			CellColorChange(dataGridView1.Rows[dataGridView1.Rows.Count - 1], Color.White);
		}

		internal DataGridViewRow? GetRow(Bytes uid)
		{
			DataGridViewRow? row;
			try {
				row = dataGridView1.Rows.Cast<DataGridViewRow>().First(row => row.Cells["UidColumn"].Value.Equals(uid.ToString()));
			} catch(InvalidOperationException) {
				row = null;
			}
			return row;
		}

		private void CellColorChange(DataGridViewRow row, Color color)
		{
			row.DefaultCellStyle.BackColor = color;
			row.DefaultCellStyle.SelectionBackColor = color;
		}

		private async void RoomStateChanged(bool isOpen)
		{
			var text = "";
			if(textBox1.Text != "") {
				text = textBox1.Text;
				discordClient.StateText = text;

				text = string.Format("\n（{0}）", text);
			}
			if(isOpen) {
				text = "開室中です。" + text;
				Invoke(new Action(() => {
					textBox2.BackColor = BLUE;
					notifyIcon.Icon = openyIcon;
				}));
			} else {
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					CellColorChange(row, RED);
				}
				text = "閉室しました。" + text;
				Invoke(new Action(() => {
					textBox2.BackColor = RED;
					notifyIcon.Icon = closeyIcon;
				}));
			}

			await discordClient.ChangeState(isOpen);
			await PostStateDiscord(isOpen);
			Save();

			var statusCode = 0;
			var i = 1;
			var added = "";
			while((statusCode = Twitter.Tweet(text + added)) == 403) {
				added = string.Format("\n({0})", i);
				i++;
			}
			switch(statusCode) {
				case 0:
					break;
				default:
					await Task.Run(() => {
						MessageBox.Show(string.Format("予期せぬエラーが発生しました\nステータスコード：{0}", statusCode));
					});
					break;
			}

			Invoke(new Action(() => {
				textBox1.Text = "";
			}));
		}

		private async void Inserted(CardStatusEventArgs e)
		{
			if(!ic.ReadUid(e.ReaderName, out Bytes uid)) {
				return;
			}

			if(room.ChangeState(uid)) {
				for(int i = 0; i < 2; i++) {
					Console.Beep(800, 50);
				}

				var row = GetRow(uid);
				if(row == null) {
					return;
				}
				if(room.Members[uid].InRoom) {
					row.Cells["TimesColumn"].Value = room.Members[uid].Times;
					CellColorChange(row, Color.White);
				} else {
					CellColorChange(row, RED);
				}

				await PostStateDiscord();
				Save();
			} else {
				Console.Beep(1000, 1000);
			}
		}

		private Room Initialize()
		{
			XDocument xd;
			try {
				xd = XDocument.Load(PATH);
			}catch (Exception e) when (e is FileNotFoundException || e is XmlException) {
				return new();
			}
			var roomX = xd.Element("Room");
			if(roomX == null) {
				return new();
			}

			var masterX = roomX.Element("Master");
			(var masterUid, var master) = GetUidAndPerson(masterX);

			var members = new Dictionary<Bytes, Person>();
			var membersX = roomX.Element("Members");
			if(membersX == null) {
				return new(masterUid, master, members);
			}
			
			var membersXEnumerable = membersX.Elements("Member");
			foreach(var memberX in membersXEnumerable) {
				(var uid, var member) = GetUidAndPerson(memberX);
				if(uid == new Bytes() && member == new Person()) {
					break;
				}
				members.Add(uid, member);
			}

			var isOpenX = roomX.Element("IsOpen");
			var isOpen = false;
			if(isOpenX != null) {
				if(bool.TryParse(isOpenX.Value, out isOpen)) { 
				}
			}

			var discordMessageIdX = roomX.Element("DiscordMessageId");
			ulong discordMessageId = 0;
			if(discordMessageIdX != null) {
				if(ulong.TryParse(discordMessageIdX.Value, out discordMessageId)) {
				}
			}

			var stateTextX = roomX.Element("StateText");
			var stateText = "";
			if(stateTextX != null) {
				stateText = stateTextX.Value;
			}
			discordClient.StateText = stateText;
			discordClient.MessageId = discordMessageId;
			_ = discordClient.ChangeState(isOpen);

			return new(masterUid, master, members, isOpen);
		}

		private void Exit()
		{
			notifyIcon.Visible = false;
			ic.Cancel();

			Save();
		}

		internal void Save()
		{
			room.Sort();
			var setting = new XmlWriterSettings();
			setting.Indent = true;
			setting.IndentChars = "\t";
			using(var writer = XmlWriter.Create(PATH, setting)) {
				writer.WriteStartElement("Room");

				writer.WriteStartElement("Master");
				writer.WriteAttributeString("Uid", room.MasterUid.ToString());
				WritePerson(writer, room.Master);
				writer.WriteEndElement();

				writer.WriteStartElement("Members");
				foreach(var item in room.Members) {
					writer.WriteStartElement("Member");
					writer.WriteAttributeString("Uid", item.Key.ToString());
					WritePerson(writer, item.Value);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();

				writer.WriteElementString("IsOpen", room.IsOpen.ToString());
				writer.WriteElementString("DiscordMessageId", discordClient.MessageId.ToString());
				writer.WriteElementString("StateText", discordClient.StateText);

				writer.WriteEndElement();

			}
		}

		private async Task PostStateDiscord(bool reset = false)
		{
			room.Sort();
			await discordClient.PostState(room.ToString(), reset);
		}

		private (Bytes uid, Person person) GetUidAndPerson(XElement? xe)
		{
			var nullResult = (new Bytes(), new Person());

			if(xe == null) {
				return nullResult;
			}

			if(xe.Attribute("Uid") == null) {
				return nullResult;
			}
			var uid = Bytes.GetBytes(xe.Attribute("Uid").Value);

			if(xe.Element("Name") == null || xe.Element("Times") == null || xe.Element("InRoom") == null) {
				return nullResult;
			}
			var name = xe.Element("Name").Value;
			if(!uint.TryParse(xe.Element("Times").Value, out var times)) {
				return nullResult;
			}
			if(!bool.TryParse(xe.Element("InRoom").Value, out var inRoom)) {
				return nullResult;
			}

			return (uid, new(name, times, inRoom));
		}

		private void WritePerson(XmlWriter writer, Person person)
		{
			writer.WriteElementString("Name", person.Name);
			writer.WriteElementString("Times", person.Times.ToString());
			writer.WriteElementString("InRoom", person.InRoom.ToString());
		}

		private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{
			var i = (e.CellValue1 == null ? 0 : uint.Parse(e.CellValue1.ToString()));
			var j = (e.CellValue2 == null ? 0 : uint.Parse(e.CellValue2.ToString()));

			if(i < j) {
				e.SortResult = -1;
			} else if(i > j) {
				e.SortResult = 1;
			} else {
				e.SortResult = 0;
			}

			e.Handled = true;
		}
	}
}