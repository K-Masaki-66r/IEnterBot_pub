using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;
using PCSC.Monitoring;

namespace IEnterBot
{
	internal class IC : IDisposable
	{
		internal string[] ReaderNames { get; private set; }
		internal ISCardMonitor Monitor { get; set; }
		internal ISCardContext Context { get; set; }

		internal IC()
		{
			Context = ContextFactory.Instance.Establish(SCardScope.System);
			ReaderNames = Context.GetReaders();
			Monitor = MonitorFactory.Instance.Create(SCardScope.System);
			Monitor.CardInserted += (sender, e) => { CardInserted(sender, e); };
		}

		internal CardInsertedEvent CardInserted = (sender, e) => { };
		internal Action ReaderExceptionOccured = () => { };

		public void Dispose()
		{
			Context.Dispose();
			Monitor.Dispose();
		}

		internal void Start()
		{
			try {
				Monitor.Start(ReaderNames);
			} catch(PCSCException e) {
				MessageBox.Show(e.ToString());
				ReaderExceptionOccured();
			}
		}

		internal void Cancel()
		{
			Monitor.Cancel();
		}

		internal bool ReadUid(int choice, out Bytes uid)
		{
			if(choice > ReaderNames.Length) {
				uid = new Bytes();
				return false;
			}

			var readerName = ReaderNames[choice];
			return ReadUid(readerName, out uid);
		}

		/// <summary>
		/// カードを読み取り、そのUIDを渡す。
		/// </summary>
		/// <param name="readerName">読み取るリーダーの名前。</param>
		/// <param name="uid">読み取りに成功した場合UIDを、失敗した場合空のBytesを返す。</param>
		/// <returns>UIDの読み取りに成功したか失敗したかを返す。</returns>
		internal bool ReadUid(string readerName, out Bytes uid)
		{
			try {
				using(var rfidReader = Context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any)) {
					var adpu = new CommandApdu(IsoCase.Case2Short, rfidReader.Protocol) {
						CLA = 0xFF,
						Instruction = InstructionCode.GetData,
						P1 = 0x00,
						P2 = 0x00,
						Le = 0,
					};

					using(rfidReader.Transaction(SCardReaderDisposition.Leave)) {
						var sendPci = SCardPCI.GetPci(rfidReader.Protocol);
						var receivePci = new SCardPCI();

						var receiveBuffer = new byte[256];
						var command = adpu.ToArray();

						var bytesReceived = rfidReader.Transmit(
							sendPci,
							command,
							command.Length,
							receivePci,
							receiveBuffer,
							receiveBuffer.Length
						);

						var responseApdu = new ResponseApdu(receiveBuffer, bytesReceived, IsoCase.Case2Short, rfidReader.Protocol);
						if(responseApdu.SW1 == 0x90 && responseApdu.SW2 == 0x00 && responseApdu.HasData) {
							uid = new Bytes(responseApdu.GetData());
							return true;
						}
					}
				}
			} catch(PCSCException) {
			}
			uid = new Bytes();
			return false;
		}
	}
}
