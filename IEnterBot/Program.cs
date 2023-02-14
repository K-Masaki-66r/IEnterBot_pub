namespace IEnterBot
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static async Task Main()
		{
			var mutexName = "IEnterBot";
			
			using(var mutex = new MutexWrapper(false, mutexName)) {
				var hasHandle = false;

				try {
					hasHandle = await mutex.WaitOneAsync(0, false);
				} catch(AbandonedMutexException) {
					// �ʂ̃A�v���P�[�V�������~���[�e�b�N�X���J�����Ȃ��ŏI�������Ƃ�
					hasHandle = true;
				}

				if(!hasHandle) {
					MessageBox.Show("���łɋN�����Ă��܂��B");
					return;
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				ApplicationConfiguration.Initialize();
				using(var ic = new IC()) {
					var form = new Form1(ic);
					Application.Run(form);
				}
			}
		}
	}
}