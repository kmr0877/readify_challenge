using ReadifyBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ReadifyBank
{
    /// <summary>
    /// Readify Bank interface
    /// </summary>
    public class ReadifyBank : IReadifyBank
    {
        #region Variables and Properties

        /// <summary>
        /// Bank accounts list
        /// </summary>
        public IList<IAccount> AccountList { get; private set;}
        
        /// <summary>
        /// Transactions log of the bank
        /// </summary>
        public IList<IStatementRow> TransactionLog { get; set;}

        /// <summary>
        /// Open a home loan account
        /// </summary>
        private int _loanAccountCounter;

        /// <summary>
        /// Stored counter for savings account
        /// </summary>

        private int _savingsAccountCounter;


        //Account prefixes
        private const string LOAN_ACCOUNT_PREFIX = "LN-";
        private const string SAVINGS_ACCOUNT_PREFIX = "SV-";

        //Interest rates
        //IMP: This looks like an issue in the querstion. Monthly rate for Savings cannot be as high as 6% because otherwise if monthly is 6% then annual is 72% which is way too high
        private const decimal SAVINGS_RATE = 6 * 12; 
        private const decimal LOAN_RATE = 3.99M;

        #endregion

        #region Constructor

         //Default Constructor to initialise AccountList & TransactionLog
        public ReadifyBank()
        {
            AccountList = new List<IAccount>();
            TransactionLog = new List<IStatementRow>();
            this._loanAccountCounter = this._savingsAccountCounter = 1;
        }
        #endregion

        #region Methods
        


        /// <summary>
        /// Calculate interest rate for an account to a specific time
        /// The interest rate for Saving account is 6% monthly
        /// The interest rate for Home loan account is 3.99% annually
        /// </summary>
        /// <param name="account">Customer account</param>
        /// <param name="toDate">Calculate interest to this date</param>
        /// <returns>The added value</returns>

        public decimal CalculateInterestToDate(IAccount account, DateTimeOffset toDate)
        {
            if (!DoesAccountExists(account) || toDate.Date > DateTimeOffset.Now.Date)
            {
                return 0;
            }

            DateTimeOffset date = account.OpenedDate;

            IStatementRow lastTransactionLog = TransactionLog.Where(x => x.Account.AccountNumber == account.AccountNumber).OrderByDescending(x => x.Date).FirstOrDefault();

            if (lastTransactionLog != null)
                date = lastTransactionLog.Date;

            int days = (toDate - date.Date).Days;

            if (account.AccountNumber.StartsWith(LOAN_ACCOUNT_PREFIX))
                return ((account.Balance * LOAN_RATE / days) * 100) / 365; 
            else
                return ((account.Balance * SAVINGS_RATE / days) * 100) / 365;

        }

        /// <summary>
        /// Close an account
        /// </summary>
        /// <param name="account">Customer account</param>
        /// <param name="closeDate">Close Date</param>
        /// <returns>All transactions happened on the closed account</returns>
        public IEnumerable<IStatementRow> CloseAccount(IAccount account, DateTimeOffset closeDate)
        {
            if (!DoesAccountExists(account))
                return null;

            //when we close account we perform withdrawal.
            PerformWithdrawal(account, account.Balance, "Withdraw available balance on event of account closure", DateTimeOffset.Now);
            AccountList.Remove(account);

            IEnumerable<IStatementRow> closedAccountTransactionLogs = TransactionLog.Where(x => x.Account.AccountNumber == account.AccountNumber).OrderBy(x => x.Date);

            //also remove from the transaction logs
            TransactionLog = TransactionLog.Except(closedAccountTransactionLogs).ToList();

            return closedAccountTransactionLogs;
        }


        /// <param name="customerName">Customer name</param>
        /// <param name="openDate">The date of the transaction</param>
        /// <returns>Opened Account</returns>
        private IAccount OpenHomeLoanAccount(string customerName, DateTimeOffset openDate);

        /// <summary>
        /// Open a saving account
        /// </summary>
        /// <param name="customerName">Customer name</param>
        /// <param name="openDate">The date of the transaction</param>
        /// <returns>Opened account</returns>
        IAccount OpenSavingsAccount(string customerName, DateTimeOffset openDate);

        /// <summary>
        /// Deposit amount in an account
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="amount">Deposit amount</param>
        /// <param name="description">Description of the transaction</param>
        /// <param name="depositDate">The date of the transaction</param>
        void PerformDeposit(IAccount account, decimal amount, string description, DateTimeOffset depositDate);

        /// <summary>
        /// Withdraw amount in an account
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="amount">Withdrawal amount</param>
        /// <param name="description">Description of the transaction</param>
        /// <param name="withdrawalDate">The date of the transaction</param>
        void PerformWithdrawal(IAccount account, decimal amount, string description, DateTimeOffset withdrawalDate);

        /// <summary>
        /// Transfer amount from an account to an account
        /// </summary>
        /// <param name="from">From account</param>
        /// <param name="to">To account</param>
        /// <param name="amount">Transfer amount</param>
        /// <param name="description">Description of the transaction</param>
        /// <param name="transferDate">The date of the transaction</param>
        void PerformTransfer(IAccount from, IAccount to, decimal amount, string description, DateTimeOffset transferDate);

        /// <summary>
        /// Return the balance for an account
        /// </summary>
        /// <param name="account">Customer account</param>
        /// <returns></returns>
        decimal GetBalance(IAccount account);

        /// <summary>
        /// Calculate interest rate for an account to a specific time
        /// The interest rate for Saving account is 6% monthly
        /// The interest rate for Home loan account is 3.99% annually
        /// </summary>
        /// <param name="account">Customer account</param>
        /// <param name="toDate">Calculate interest to this date</param>
        /// <returns>The added value</returns>
        decimal CalculateInterestToDate(IAccount account, DateTimeOffset toDate);

        /// <summary>
        /// Get mini statement (the last 5 transactions occurred on an account)
        /// </summary>
        /// <param name="account">Customer account</param>
        /// <returns>Last five transactions</returns>
        IEnumerable<IStatementRow> GetMiniStatement(IAccount account);

        /// <summary>
        /// Close an account
        /// </summary>
        /// <param name="account">Customer account</param>
        /// <param name="closeDate">Close Date</param>
        /// <returns>All transactions happened on the closed account</returns>
        IEnumerable<IStatementRow> CloseAccount(IAccount account, DateTimeOffset closeDate);
    }
}
