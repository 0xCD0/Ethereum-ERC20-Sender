using System;
using Ethereum;

namespace EthereumBalance {
    class Program {
        public static EthereumUtility eth;

        static void Main(string[] args) {
            eth = new EthereumUtility();

            // Ethereum 전송 완료 후 Transaction Hash를 받기 위한 Callback 등록
            eth.TransactionHashReady_Event += Eth_TransactionHashReady_Event;

            // 자신의 Ethereum Wallet의 Private Key를 입력
            eth.PrivateKey = "Input Your ethereum wallet private key";

            // RPC 서버 선택
            eth.Server = EthereumUtility.EthereumServerList.Ropsten_Test_Network;

            // 잔액 조회 
            eth.GetAccountBalanceFromPrivateKey().Wait();

            // Ethereum 송금 (toAddr에 송금할 Ethereum Wallet 주소 입력)
            string toAddr = "Input receive ethereum wallet address";
            eth.TransferEthereum(toAddr, 0.05f).Wait();

            Console.ReadLine();
        }

        /// <summary>
        /// Ethereum 송금이 완료 된 후 호출되는 Callback Event입니다. Transaction Hash를 출력합니다.
        /// </summary>
        /// <param name="Hash">송금 완료 후의 Transaction Hash 값입니다.</param>
        private static void Eth_TransactionHashReady_Event(string Hash) {
            Console.WriteLine($"Transaction Hash : {Hash}");

            #region Transaction Hash 수신 후 EtherScan.io에서 각 서버별 Transaction 확인
            string server = string.Empty;

            switch (eth.Server) {
                case EthereumUtility.EthereumServerList.Ropsten_Test_Network:
                    server = "ropsten.";
                    break;

                case EthereumUtility.EthereumServerList.Kovan_Test_Network:
                    server = "kovan.";
                    break;

                case EthereumUtility.EthereumServerList.Rinkeby_Test_Network:
                    server = "rinkeby.";
                    break;
            }

            System.Diagnostics.Process.Start("explorer.exe", string.Format("https://{0}etherscan.io/tx/{1}", server, Hash));
            #endregion

        }
    }
}