using System;
using EthereumUtility.ETHUtility;
using EthereumUtility.AccountUtility;

namespace EthereumBalance {
    class Program {
        public static ETHUtility eth;
        public static AccountUtility acc;

        static void Main(string[] args) {
            #region 기본 입력 코드
            eth = new ETHUtility();

            // 자신의 Ethereum Wallet의 Private Key를 입력
            eth.PrivateKey = "Input Your ethereum wallet private key";

            // ERC-20 Smart Contract Address 주소 입력 (ERC-20 토큰을 사용하지 않을 것이라면 입력하지 않아도 무관)
            eth.SmartContractAddress = "Input smart contract address";

            // RPC 서버 선택
            eth.Server = ETHUtility.EthereumServerList.Ropsten_Test_Network;

            // 보낼 상대방 지갑 주소
            string toAddr = "Input receiver ethereum wallet address";

            // Ethereum 전송 완료 후 Transaction Hash를 받기 위한 Callback 등록
            eth.TransactionHashReady_Event += Eth_TransactionHashReady_Event;

            #endregion

            #region Ethereum 지갑 생성 코드
            // Ethereum의 지갑 생성 코드입니다. 아래 주석을 풀고 실행 후 생성 성공시 PrivateKey와 WalletAccount 주소가 출력됩니다.
            // WalletAddress 주소는 랜덤 Hash 입니다.

            //acc = new AccountUtility();
            //AccountInfo temp = acc.CreateEthereumAccount();

            //Console.WriteLine($"Private Key : {temp.privateKey.ToString()} \r\nWalletAddress : {temp.accountAddress.ToString()}");
            #endregion

            #region Ethereum 전송 및 조회 샘플 코드
            // 잔액 조회 
            eth.GetAccountBalanceFromPrivateKey().Wait();

            //// Ethereum 전송 (toAddr에 전송할 Ethereum Wallet 주소 입력)
            // 보낼 금액
            //float amount = 0.5f;
            //eth.TransferEthereum(toAddr, amount).Wait();
            #endregion

            #region ERC-20 Token 전송 및 조회 샘플 코드
            //// Token 잔액 조회
            //eth.GetAccountTokenBalanceFromPrivateKey().Wait();

            //// 보낼 토큰 액수 (Decimal 자리수까지 포함해야 합니다.)
            //ulong amount = 30000;

            //eth.TransferTokens(toAddr, amount).Wait();
            #endregion

            Console.WriteLine("Processes all over.");
            Console.ReadLine();
        }

        #region Delegate Callback event
        /// <summary>
        /// Ethereum 송금이 완료 된 후 호출되는 Callback Event입니다. Transaction Hash를 출력합니다.
        /// </summary>
        /// <param name="Hash">송금 완료 후의 Transaction Hash 값입니다.</param>
        private static void Eth_TransactionHashReady_Event(string Hash) {
            Console.WriteLine($"Transaction Hash : {Hash}");

            #region Transaction Hash 수신 후 EtherScan.io에서 각 서버별 Transaction 확인
            string server = string.Empty;

            switch (eth.Server) {
                case ETHUtility.EthereumServerList.Ropsten_Test_Network:
                    server = "ropsten.";
                    break;

                case ETHUtility.EthereumServerList.Kovan_Test_Network:
                    server = "kovan.";
                    break;

                case ETHUtility.EthereumServerList.Rinkeby_Test_Network:
                    server = "rinkeby.";
                    break;
            }

            System.Diagnostics.Process.Start("explorer.exe", string.Format("https://{0}etherscan.io/tx/{1}", server, Hash));
            #endregion
        }
        #endregion
    }
}