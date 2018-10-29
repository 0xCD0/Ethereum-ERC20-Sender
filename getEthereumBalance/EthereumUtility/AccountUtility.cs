using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Security;

namespace EthereumUtility.AccountUtility {
    #region AccountInfo Struct
    public struct AccountInfo {
        public string privateKey;
        public string accountAddress;
    }
    #endregion

    /// <summary>
    /// Ethereum의 Account에 관련한 Utility 클래스입니다.
    /// </summary>
    public class AccountUtility {

        /// <summary>
        /// 새로운 Ethereum Wallet을 제작합니다.
        /// </summary>
        /// <returns>Account 생성 정보가 담긴 AccuntInfo 구조체를 반환합니다.</returns>
        public AccountInfo CreateEthereumAccount() {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var pKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var acc = new Account(pKey);

            AccountInfo info = new AccountInfo();
            info.privateKey = pKey;
            info.accountAddress = acc.Address;

            return info;
        }
    }
}
