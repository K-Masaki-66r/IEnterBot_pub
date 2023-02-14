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
					// 別のアプリケーションがミューテックスを開放しないで終了したとき
					hasHandle = true;
				}

				if(!hasHandle) {
					MessageBox.Show("すでに起動しています。");
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