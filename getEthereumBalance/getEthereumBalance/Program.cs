using System;
using System.Threading.Tasks;
using Nethereum.Web3;

namespace EthereumBalance {

    #region Ethereum Server Enum
    enum EthereumServerList {
        MainNet_Ethereum_Network,
        Ropsten_Test_Network,
        Kovan_Test_Network,
        Rinkeby_Test_Network
    }
    #endregion

    class Program {
        // Ethereum Wallet 주소를 입력합니다.
        public static string walletAddress = "Input your walletAddress here";

        static void Main(string[] args) {

            // EthereumServerList 열거형에서 서버 리스트를 선택하여 입력하십시오.
            GetAccountBalance(EthereumServerList.MainNet_Ethereum_Network, walletAddress).Wait();
            Console.ReadLine();
        }

        /// <summary>
        /// Ethereum의 잔액을 조회합니다.
        /// </summary>
        /// <param name="server">EthereumServerList 열거형에서 조회할 서버를 입력합니다.</param>
        /// <param name="walletAddress">지갑 주소를 입력합니다.</param>
        static async Task GetAccountBalance(EthereumServerList server, string walletAddress) {
            string serverLocation = string.Empty;

            switch (server) {
                case EthereumServerList.MainNet_Ethereum_Network:
                    serverLocation = "https://mainnet.infura.io";
                    break;

                case EthereumServerList.Ropsten_Test_Network:
                    serverLocation = "https://ropsten.infura.io";
                    break;

                case EthereumServerList.Kovan_Test_Network:
                    serverLocation = "https://kovan.infura.io";
                    break;

                case EthereumServerList.Rinkeby_Test_Network:
                    serverLocation = "https://rinkeby.infura.io";
                    break;
            }

            try {
                var web3 = new Web3(serverLocation);

                #region Ethereum의 잔액 조회 부분
                // Wei는 BitCoin의 사토시 처럼 존재하는 Ethereum의 최소 단위입니다.
                var balance = await web3.Eth.GetBalance.SendRequestAsync(walletAddress);
                Console.WriteLine($"Balance in Wei: {balance.Value}");

                var etherAmount = Web3.Convert.FromWei(balance.Value);
                Console.WriteLine($"Balance in Ethereum: {etherAmount}");
                #endregion
            }
            catch (Exception ex) {
                Console.WriteLine($"Ethereum wallet 주소가 잘못되었거나 잘못된 입력입니다. \r\n\r\n Original Message : { ex.ToString()}");
            }

        }
    }
}