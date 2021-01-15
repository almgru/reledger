using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace API.DAO
{
    public class DatabaseAccess
    {
        private readonly string connectionString;

        public DatabaseAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddTransaction(string debitAccount, decimal amount, string creditAccount, DateTime date,
                                   List<String> tags)
        {
            // 1. Insert debit account into database in case its missing
            // 2. Insert credit account into database in case its missing
            // 3. Add all tags into the database in case any are missing
            // 4. Insert the transaction into the database
            // 5. Link the transaction to the debited account by inserting its ID and the debit account name into the
            //    'Debits' table.
            // 6. Link the transaction to the credited account by inserting its ID and the credit account name into the
            //    'Credits' table.
            // 7. For each tag, link it to the transaction by inserting the transaction ID and the tag's name into the
            //    'Categorizes' table.
            // 8. For each attachment, link it to the transaction by inserting the transaction ID, the attachment name,
            //    and the binary blob into the 'Attachments' table.
        }

        /*
         * Insert all accounts specified in the accountString and registers any ancestors/descendants by
         * inserting into the AncestorTo table.
         *
         * The insertion happens in the order most ancestral to most descendant.
         *
         * Example:
         *
         * AddAccount("A.B.C") will first insert account 'A' to the 'Accounts' table and add 'B' and 'C' as descendants
         * of 'A' by adding them to the 'AncestorOf' table. Next, 'B' will be inserted into 'Accounts' and 'C' will be
         * added as its decendant. Finally, 'C' will be inserted into 'Accounts'. Since 'C' has no descendants, the
         * 'AncestorTo' table will not be further modified.
         */
        public void AddAccount(string accountString)
        {
            using (var connection = GetConnection())
            {
                SqliteTransaction transaction = connection.BeginTransaction();

                while (accountString != "") // Repeat until no more account names in the account string
                {
                    // Extract first account name as ancestor and the rest of the string as its descendants
                    int indexOfDot = accountString.IndexOf('.');
                    string accountName = accountString.Substring(0, indexOfDot);
                    string descendants = accountString.Substring(indexOfDot);

                    // Add the ancestor
                    this.ExecuteCommand(connection, "INSERT INTO Accounts(name) VALUES(:name);",
                                        (":name", accountName));

                    foreach (string descendant in descendants.Split('.')) // for each descendant
                    {
                        // Link the ancestor to the descendant
                        this.ExecuteCommand(connection, @"
                                    INSERT INTO AncestorTo(ancestorName, descendantName)
                                    VALUES(:ancestorName, :descendantName);
                                ",
                                (":ancestorName", accountName), (":descendantName", descendant));
                    }

                    // Remove the ancestor from the account string
                    accountString = accountString.Remove(0, indexOfDot).Trim();
                }

                transaction.Commit();
            }
        }

        public void CreateSchema()
        {
            using (var connection = GetConnection())
            {
                SqliteTransaction transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Transactions(
                        id INTEGER PRIMARY KEY NOT NULL,
                        date TEXT NOT NULL,
                        amount TEXT NOT NULL,
                        currency TEXT NOT NULL,
                        description TEXT
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Attachments(
                        name TEXT NOT NULL,
                        transactionId INTEGER NOT NULL,
                        data BLOB NOT NULL,
                        
                        PRIMARY KEY(name, transactionId),

                        FOREIGN KEY (transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Tags(
                        name TEXT PRIMARY KEY NOT NULL
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Categorizes(
                        tagName TEXT NOT NULL,
                        transactionId INTEGER NOT NULL,

                        PRIMARY KEY(tagName, transactionId),

                        FOREIGN KEY(tagName)
                        REFERENCES Tags(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Debits(
                        transactionId INTEGER NOT NULL,
                        accountName TEXT NOT NULL,

                        PRIMARY KEY(transactionId, accountName),

                        FOREIGN KEY(transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(accountName)
                        REFERENCES Accounts(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Credits(
                        transactionId INTEGER NOT NULL,
                        accountName TEXT NOT NULL,

                        PRIMARY KEY(transactionId, accountName),

                        FOREIGN KEY(transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(accountName)
                        REFERENCES Account(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    )
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Accounts(
                        name TEXT PRIMARY KEY NOT NULL
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE AncestorTo(
                        ancestorName TEXT NOT NULL,
                        descendantName TEXT NOT NULL,

                        PRIMARY KEY(ancestorName, descendantName),

                        FOREIGN KEY(ancestorName)
                        REFERENCES Accounts(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(descendantName)
                        REFERENCES Accounts(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE);
                ");

                transaction.Commit();
            }
        }

        private SqliteConnection GetConnection()
        {
            SqliteConnection connection = new SqliteConnection(this.connectionString);
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "PRAGMA foreign_keys = ON;";

            connection.Open();
            command.ExecuteNonQuery();

            return connection;
        }

        private void ExecuteCommand(SqliteConnection connection, string commandString)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = commandString;
            command.ExecuteNonQuery();
        }

        private void ExecuteCommand(SqliteConnection connection, string commandString,
                                    params (string, Object)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandString;

            foreach ((string, Object) tuple in parameters)
            {
                command.Parameters.AddWithValue(tuple.Item1, tuple.Item2);
            }

            command.ExecuteNonQuery();
        }
    }
}