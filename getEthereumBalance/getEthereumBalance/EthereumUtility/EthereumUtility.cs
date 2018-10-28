using System;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;

/// <summary>
/// Nethereum을 손쉽게 사용하기 위한 함수들이 모인 Utility 클래스입니다.
/// </summary>
namespace Ethereum {
    public class EthereumUtility {
        #region Transaction Hash Event
        public delegate void TransactionHashReady(string Hash);
        public event TransactionHashReady TransactionHashReady_Event;
        #endregion

        #region Ethereum Server Enum
        public enum EthereumServerList {
            MainNet_Ethereum_Network,
            Ropsten_Test_Network,
            Kovan_Test_Network,
            Rinkeby_Test_Network
        }
        #endregion

        #region 로컬 변수
        public string PrivateKey { get; set; }

        public EthereumServerList Server = EthereumServerList.MainNet_Ethereum_Network;
        #endregion

        #region 함수
        /// <summary>
        /// 로컬 변수인 this.PrivateKey에 등록된 PrivateKey를 기반으로 Ethereum Wallet의 잔액을 조회합니다.
        /// </summary>
        public async Task GetAccountBalanceFromPrivateKey() {
            string serverLocation = GetServerLocationFormEnum(Server);

            try {
                var web3 = new Web3(serverLocation);

                #region Ethereum의 잔액 조회 부분
                // Wei는 BitCoin의 사토시 처럼 존재하는 Ethereum의 최소 단위입니다.
                var balance = await web3.Eth.GetBalance.SendRequestAsync(GetWalletAddressFromPrivateKey());
                Console.WriteLine($"Wei 잔고 : {balance.Value}");

                var etherAmount = Web3.Convert.FromWei(balance.Value);
                Console.WriteLine($"Ethereum 잔고 : {etherAmount}");
                #endregion
            }
            catch (Exception ex) {
                Console.WriteLine($"Ethereum Wallet 주소가 잘못되었거나 잘못된 입력입니다. \r\n\r\n Original Message : { ex.ToString()}");
            }

        }

        /// <summary>
        /// 인자로 입력하는 WalletAddress를 기반으로 Ethereum Wallet 잔액을 조회합니다.
        /// </summary>
        /// <param name="WalletAddress"></param>
        /// <returns></returns>
        public async Task GetAccountBalanceFromTargetWallet(string WalletAddress) {
            string serverLocation = GetServerLocationFormEnum(Server);

            try {
                var web3 = new Web3(serverLocation);

                #region Ethereum의 잔액 조회 부분
                // Wei는 BitCoin의 사토시 처럼 존재하는 Ethereum의 최소 단위입니다.
                var balance = await web3.Eth.GetBalance.SendRequestAsync(WalletAddress);
                Console.WriteLine($"Wei 잔고 : {balance.Value}");

                var etherAmount = Web3.Convert.FromWei(balance.Value);
                Console.WriteLine($"Ethereum 잔고 : {etherAmount}");
                #endregion
            }
            catch (Exception ex) {
                Console.WriteLine($"Ethereum Wallet 주소가 잘못되었거나 잘못된 입력입니다. \r\n\r\n Original Message : { ex.ToString()}");
            }

        }

        /// <summary>
        /// Ethereum을 전송합니다. 전송된 후에는 TransactionHashReady_Event 이벤트가 작동되어 Transaction Hash가 전달됩니다.
        /// </summary>
        /// <param name="ToAddress">Ethereum을 수신 받을 지갑 주소를 입력합니다.</param>
        /// <param name="Balance">얼만큼의 Ethereum을 보낼 것인지 float형태로 입력합니다.</param>
        /// <param name="Decimal">Ethereum의 Decimal(자릿수) 입니다. 기본값은 0입니다.</param>
        /// <returns></returns>
        public async Task TransferEthereum(string ToAddress, float Balance, int Decimal = 0) {
            string serverLocation = GetServerLocationFormEnum(Server);

            try {
                var sendVal = Web3.Convert.ToWei(Balance);
                var account = new Account(PrivateKey);
                var web3 = new Web3(account, serverLocation);

                var transAction = await web3.Eth.TransactionManager.SendTransactionAsync(GetWalletAddressFromPrivateKey(), ToAddress, new HexBigInteger(sendVal));

                TransactionHashReady_Event(transAction);
            }

            catch (Exception ex) {
                Console.WriteLine($"Private Key가 잘못되었거나 서버가 응답하지 않습니다. \r\n\r\n Original Message : { ex.ToString()}");
            }

        }

        #region 내부 유틸리티 함수
        /// <summary>
        /// PrivateKey를 사용하여 Account(WalletAddress)를 얻습니다.
        /// </summary>
        /// <returns>Account(WalletAddress)를 반환합니다.</returns>
        private string GetWalletAddressFromPrivateKey() {
            var account = new Account(PrivateKey);

            return account.Address;
        }

        /// <summary>
        /// EthereumServerList 열거형으로 실제 서버 주소를 String으로 반환하는 함수입니다.
        /// </summary>
        /// <param name="server">ServerList</param>
        /// <returns>Ethereum 실제 Server의 주소를 string 형태로 반환합니다.</returns>
        private string GetServerLocationFormEnum(EthereumServerList server) {
            string _server = string.Empty;

            switch (server) {
                case EthereumServerList.MainNet_Ethereum_Network:
                    _server = "https://mainnet.infura.io";
                    break;

                case EthereumServerList.Ropsten_Test_Network:
                    _server = "https://ropsten.infura.io";
                    break;

                case EthereumServerList.Kovan_Test_Network:
                    _server = "https://kovan.infura.io";
                    break;

                case EthereumServerList.Rinkeby_Test_Network:
                    _server = "https://rinkeby.infura.io";
                    break;
            }

            return _server;
        }
        #endregion

        #endregion
    }
}
