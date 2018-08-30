using ReadifyBank.Interfaces;
using System;

namespace ReadifyBank
{
    //Implementation of IAccount interface
    public class Account : IAccount
    {
        #region Properties

        public string AccountNumber { get; private set; }
       
        public decimal Balance { get; set; }
       
        public string CustomerName { get; private set; }
        
        public DateTimeOffset OpenedDate { get; private set; }
        
        #endregion

        #region Constructor
        /// <summary>
        /// Parameterised constructor for initiating Account
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="customerName"></param>
        /// <param name="openedDate"></param>
        public Account(string accountNumber, string customerName, DateTimeOffset openedDate)
        {
            this.AccountNumber = accountNumber;
            this.CustomerName = customerName;
            this.OpenedDate = openedDate;
            this.Balance = 0; //when account opens balance is obviously 0
        }
        #endregion

    }
}
