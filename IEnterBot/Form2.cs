namespace IEnterBot
{
	internal partial class Form2 : Form
	{
		private Room room;
		private IC ic;
		private Bytes uid = new();
		private Mode mode;
		private Form1 form1;
		private bool hasRead { get; set; } = false;
		internal Form2(Room room, IC ic, Mode mode, Form1 form1)
		{
			this.room = room;
			this.ic = ic;
			this.mode = mode;
			this.form1 = form1;

			ic.Cancel();

			InitializeComponent();

			foreach(var readerName in ic.ReaderNames) {
				comboBox1.Items.Add(readerName);
			}
			if(comboBox1.Items.Count > 0) {
				comboBox1.SelectedIndex = 0;
			}

			if(mode == Mode.Remove) {
				label2.Visible = false;
				textBox1.Visible = false;
				button1.Visible = false;
				button2.Text = "削除";
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if(hasRead) {
				return;
			}

			var readerName = comboBox1.Text;
			if(readerName == "") {
				MessageBox.Show("読み取り機器を選択してください。");
				return;
			}

			if(!ic.ReadUid(readerName, out var uid)) {
				MessageBox.Show("カードを読み取れませんでした。");
				return;
			}

			this.uid = uid;
			hasRead = true;

			if(mode == Mode.Remove) {
				button1_Click(this, new EventArgs());
				return;
			}

			button2.Text = "読み取り済み";
			button2.ForeColor = Color.Red;
			ActiveControl = textBox1;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(!hasRead) {
				MessageBox.Show("カードを読み取ってください。");
				ActiveControl = button2;
				return;
			}

			if(mode != Mode.Remove && textBox1.Text == "") {
				MessageBox.Show("名前を入力してください。");
				ActiveControl = textBox1;
				return;
			}
			var name = textBox1.Text;

			switch(mode) {
				case Mode.SetMaster:
				case Mode.ChangeMaster:
					if(!room.ChangeMaster(uid, new Person(name, 0))) {
						MessageBox.Show("このカードはすでにメンバーとして追加されています。");
						Reset();
						return;
					}
					break;
				case Mode.Add:
					if(!room.Add(uid, new Person(name))) {
						MessageBox.Show("このカードはすでに追加されています。");
						Reset();
						return;
					}
					form1.AddRow(uid, room[uid]);
					break;
				case Mode.Remove:
					if(!room.Remove(uid)) {
						MessageBox.Show("このカードはメンバーとして登録されていません。");
						Reset();
						return;
					}
					var row = form1.GetRow(uid);
					if(row != null) {
						form1.dataGridView1.Rows.Remove(row);
					}
					break;
			}

			form1.Save();
			Close();
		}

		private void Reset()
		{
			hasRead = false;
			ActiveControl = button2;

			if(mode == Mode.Remove) {
				return;
			}

			button2.Text = "読み取り";
			button2.ForeColor = Color.Black;
		}

		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(e.CloseReason == CloseReason.ApplicationExitCall) {
				return;
			}

			if(room.Master == new Person() && mode == Mode.SetMaster) {
				MessageBox.Show("マスターを登録してください。");
				e.Cancel = true;
			}

			ic.Start();
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)Keys.Enter) {
				button1_Click(sender, e);
			}

			if(e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape) {
				e.Handled = true;
			}
		}

		private void Form2_Load(object sender, EventArgs e)
		{
			if(mode == Mode.SetMaster) {
				MessageBox.Show("マスターを登録してください。");
			}
			ActiveControl = button2;
		}

		private void Form2_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)Keys.Escape) {
				Close();
			}
		}
	}
}
