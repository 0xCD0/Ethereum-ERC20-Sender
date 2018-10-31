using System;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.CQS;
using System.Numerics;
using Nethereum.StandardTokenEIP20.CQS;
using Nethereum.Util;

/// <summary>
/// Nethereum을 손쉽게 사용하기 위한 함수들이 모인 Utility 클래스입니다.
/// </summary>
namespace EthereumUtility.ETHUtility {
    public class ETHUtility {
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
        // 사용자의 Private Key 입니다. 반드시 입력하여 주십시오.
        public string PrivateKey { get; set; }

        // ERC-20의 고유 Smart Contract Address 입니다. ERC-20 토큰을 보낼 때 반드시 입력하여 주십시오.
        public string SmartContractAddress { get; set; }

        // RPC 서버를 선택합니다. 기본 값은 MainNet으로 되어있습니다.
        public EthereumServerList Server = EthereumServerList.MainNet_Ethereum_Network;
        #endregion

        #region 함수
        #region Ethereum 함수
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
                Console.WriteLine($"Error Occured ! \r\n Original Message : { ex.ToString()}");
            }

        }

        /// <summary>
        /// 인자로 입력하는 WalletAddress를 기반으로 Ethereum Wallet 잔액을 조회합니다.
        /// </summary>
        /// <param name="WalletAddress">Ethereum Wallet 주소를 입력합니다.</param>
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
                Console.WriteLine($"Error Occured ! \r\n Original Message : { ex.ToString()}");
            }
        }

        /// <summary>
        /// Ethereum을 전송합니다. 전송된 후에는 TransactionHashReady_Event 이벤트가 작동되어 Transaction Hash가 전달됩니다.
        /// </summary>
        /// <param name="ToAddress">Ethereum을 수신 받을 지갑 주소를 입력합니다.</param>
        /// <param name="Balance">얼만큼의 Ethereum을 보낼 것인지 float형태로 입력합니다.</param>
        /// <param name="Decimal">Ethereum의 Decimal(자릿수) 입니다. 기본값은 0입니다.</param>
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
                Console.WriteLine($"Error Occured ! \r\n Original Message : { ex.ToString()}");
            }
        }
        #endregion

        #region ERC-20 Token 함수
        /// <summary>
        /// 로컬 변수인 this.PrivateKey에 등록된 PrivateKey를 기반으로 ERC-20 Token의 잔액을 조회합니다.
        /// </summary>
        public async Task GetAccountTokenBalanceFromPrivateKey() {
            if (SmartContractAddress == string.Empty || SmartContractAddress == "") {
                throw (new Exception("Token을 전송하려면 SmartContract 주소부터 입력해야합니다."));
            }

            string serverLocation = GetServerLocationFormEnum(Server);

            try {
                var web3 = new Web3(serverLocation);

                var tokenService = new Nethereum.StandardTokenEIP20.StandardTokenService(web3, SmartContractAddress);
                var ownerBalanceTask = tokenService.GetBalanceOfAsync<BigInteger>(GetWalletAddressFromPrivateKey());
                var Balance = await ownerBalanceTask;

                Console.WriteLine(Balance.ToString());
            }
            catch (Exception ex) {
                Console.WriteLine($"Error Occured ! \r\n Original Message : { ex.ToString()}");
            }
        }

        /// <summary>
        /// 인자로 입력하는 WalletAddress를 기반으로 ERC-20 Token 잔액을 조회합니다.
        /// </summary>
        /// <param name="WalletAddress">Ethereum Wallet 주소를 입력합니다.</param>
        /// <param name="ContractAddress">SmartContract 주소(Token의 계약 고유 주소)를 입력합니다.</param>
        public async Task GetAccountTokenBalanceFromTargetWallet(string WalletAddress, string ContractAddress) {
            if (SmartContractAddress == string.Empty || SmartContractAddress == "") {
                throw (new Exception("Token을 전송하려면 SmartContract 주소부터 입력해야합니다."));
            }

            string serverLocation = GetServerLocationFormEnum(Server);

            try {
                var web3 = new Web3(serverLocation);

                var tokenService = new Nethereum.StandardTokenEIP20.StandardTokenService(web3, ContractAddress);
                var ownerBalanceTask = tokenService.GetBalanceOfAsync<BigInteger>(WalletAddress);
                var Balance = await ownerBalanceTask;

                Console.WriteLine(Balance.ToString());
            }
            catch (Exception ex) {
                Console.WriteLine($"Error Occured ! \r\n Original Message : { ex.ToString()}");
            }
        }

        /// <summary>
        /// ERC-20 토큰을 전송합니다. 전송된 후에는 TransactionHashReady_Event 이벤트가 작동되어 Transaction Hash가 전달됩니다.
        /// </summary>
        /// <param name="ToAddress">ERC-20 Token을 수신 받을 Ethereum 지갑 주소를 입력합니다.</param>
        /// <param name="Balance">얼만큼의 Token을 보낼 것인지 int 형태로 입력합니다.</param>
        /// <returns></returns>
        public async Task TransferTokens(string ToAddress, UInt64 Balance) {
            string serverLocation = GetServerLocationFormEnum(Server);

            try {
                var account = new Account(PrivateKey);
                var web3 = new Web3(account, serverLocation);

                var transactionMessage = new TransferFunction() {
                    Value = Balance,
                    FromAddress = account.Address,
                    To = ToAddress,
                    GasPrice = Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei)
                };

                var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

                // 최소 필요 가스 용량 계산
                var estimate = await transferHandler.EstimateGasAsync(transactionMessage, SmartContractAddress);
                transactionMessage.Gas = estimate.Value;

                var transactionHash = await transferHandler.SendRequestAsync(transactionMessage, SmartContractAddress);

                TransactionHashReady_Event(transactionHash.ToString());
            }
            catch (Exception ex) {
                Console.WriteLine($"Error Occured ! \r\n Original Message : { ex.ToString()}");
            }
        }

        /// <summary>
        /// ContractAddress를 별도로 입력하여 ERC-20 토큰을 전송합니다. 전송된 후에는 TransactionHashReady_Event 이벤트가 작동되어 Transaction Hash가 전달됩니다.
        /// </summary>
        /// <param name="ToAddress">ERC-20 Token을 수신 받을 Ethereum 지갑 주소를 입력합니다.</param>
        /// <param name="Balance">얼만큼의 Token을 보낼 것인지 int 형태로 입력합니다. Decimal은 자동 계산되므로 보낼 수량의 절대값만 입력하면 됩니다.</param>
        /// <param name="ContractAddress">SmartContract 주소(Token의 계약 고유 주소)를 입력합니다.</param>
        /// <returns></returns>
        public async Task TransferTokens(string ToAddress, UInt64 Balance, string ContractAddress) {
            string serverLocation = GetServerLocationFormEnum(Server);

            try {
                var account = new Account(PrivateKey);
                var web3 = new Web3(account, serverLocation);

                var transactionMessage = new TransferFunction() {
                    Value = (Balance * 100),
                    FromAddress = account.Address,
                    To = ToAddress,
                    GasPrice = Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei)
                };

                var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

                // 최소 필요 가스 용량 계산
                var estimate = await transferHandler.EstimateGasAsync(transactionMessage, ContractAddress);
                transactionMessage.Gas = estimate.Value;

                var transactionHash = await transferHandler.SendRequestAsync(transactionMessage, ContractAddress);

                TransactionHashReady_Event(transactionHash.ToString());
            }
            catch (Exception ex) {
                Console.WriteLine($"Error Occured ! \r\n Original Message : { ex.ToString()}");
            }
        }
        #endregion

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
