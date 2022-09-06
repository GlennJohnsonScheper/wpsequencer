using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// More I had to add:
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Concurrent;
using System.Windows.Forms;

// In V.S. Pkg. Mgr., browse mysql, install MySql.Data:
using MySql.Data.MySqlClient;


namespace wpsequencer
{
    public static class MySqlHandler
    {
        // focus on these cities:
        // hays 2093: WIMBERLEY
        // hays 2029: DRIPPING SPRINGS
        // comal 3434: CANYON LAKE
        // comal 1961: BULVERDE

        // 1 or 2, but go look around lines 130-150 for all the juicy details.
        static int tailoredFor = 1;
        // 1: db = Comal, city = CANYON LAKE, sort = AGE desc.
        // 2: db = Hays, mvv = mvv, sort = AGE desc.

        // This is the database connection
        static MySqlConnection sqlConnDoIt = null;

        static DateTime atop = DateTime.Now;

        // CTOR
        static MySqlHandler()
        {
            // Open my database connection.
			// TO DO: OBLITERATE THESE CREDENTIALS BEFORE SHARING:
            string connStr = "server=localhost; database=mvv; user=xxxxx; password=xxxxx";

            // I cannot use 'using' here, in this event driven app.
            // Don't be a dummy: declaration hides global-- MySqlConnection 
            sqlConnDoIt = new MySqlConnection(connStr);

            try
            {
                sqlConnDoIt.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception in SetUpMySqlConnection: " + ex.ToString());
                Application.Exit();
            }

            if (sqlConnDoIt.State != ConnectionState.Open)
            {
                MessageBox.Show("#1 sqlConnDoIt.State != ConnectionState.Open");
                Application.Exit();
            }

        }

        // After all these years, I still haaaate classes!
        private static Form1 theAppForm = null;

        // Perhaps a "first reference" to static class is required to load it? YES!
        public static void FirstReferenceToStartMe(Form1 mainForm)
        {
            theAppForm = mainForm;
            string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");
            // logLine(dts + " FirstReferenceToStartMe");

            // Oops. This is too early to update Form1 textbox...
        }
        public static void FormSaysAppExiting()
        {
            // Close my database connection.
            if (sqlConnDoIt != null)
            {
                sqlConnDoIt.Close();
                sqlConnDoIt = null;
            }
        }

        public static void eventFormShown()
        {
            // Prime the first MySQL record to query:
            // henceforth, only when the CS asks me to: SelectOneRecordLackingPhone();
        }

        static string name = ""; // already formatted as for Whitepages.com
        static string phone = "";
        static string street = "";
        static string city = "";
        static string zip5 = "";
        static string age = "";
        static string locn = ""; // City, TX, Zip5 = format for Whitepages.com

        static Random random = new Random();
        public static void WhatNameLocn(out string _name, out string _locn)
        {
            _name = name;
            _locn = locn;
        }
        public static void WhatNameLocnAge(out string _name, out string _locn, out string _age)
        {
            _name = name;
            _locn = locn;
            _age = age;
        }

        public static void SelectOneRecordLackingPhone()
        {
            if (sqlConnDoIt.State != ConnectionState.Open)
            {
                MessageBox.Show("#2 sqlConnDoIt.State != ConnectionState.Open");
                Application.Exit();
            }

            // Lookup the first record lacking a phone

            // This query is tailored for now to only get
            // CNB1 (City of NB district 1) from mvv.comal:
            string sqlSelectNextEmptyPhone = "";
            // below ... "SELECT name, phone, street, city, zip5 FROM mvv.comal WHERE phone = \"*\" AND mvv = \"CNB1\" ORDER BY age DESC LIMIT 1;";

            // this switch is tailored for (comal vs hays) and edited for all the other juicy details.
            switch (tailoredFor)
            {
                case 1:
                    // comal county
                    // mvv = CNB1
                    // sort by age descending
                    sqlSelectNextEmptyPhone =
                        // This CNB1 query is ALL USED UP! -- "SELECT name, phone, street, city, zip5, age FROM mvv.comal WHERE phone = \"*\" AND mvv = \"CNB1\" ORDER BY age DESC LIMIT 1;";
                        // This CANYON LAKE query is ALL USED UP! -- "SELECT name, phone, street, city, zip5, age FROM mvv.comal WHERE phone = \"*\" AND city = \"CANYON LAKE\" ORDER BY age DESC LIMIT 1;";
                        "SELECT name, phone, street, city, zip5, age FROM mvv.comal WHERE phone = \"*\" ORDER BY age DESC LIMIT 1;";
                    break;

                case 2:
                    // hays county
                    // mvv = mvv -- mvv can be "mvv" or "2nd"
                    // also throw in city = WIMBERLY
                    // AND city = \"WIMBERLY\" 
                    // sort by age descending
                    sqlSelectNextEmptyPhone =
                        "SELECT name, phone, street, city, zip5, age FROM mvv.hays WHERE phone = \"*\" AND mvv = \"mvv\" ORDER BY age DESC LIMIT 1;";
                    break;
            }

            // This first command will select a single record:

            using (MySqlCommand selectCmd = new MySqlCommand(sqlSelectNextEmptyPhone, sqlConnDoIt))
            {
                try
                {
                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                name = (string)reader["name"];
                                phone = (string)reader["phone"];
                                street = (string)reader["street"];
                                city = (string)reader["city"];
                                zip5 = (string)reader["zip5"];
                                age = (string)reader["age"];
                                locn = city + " TX " + zip5;
                            }
                        }
                        else
                        {
                            MessageBox.Show("No more rows remain in your query.");
                            throw new Exception("No more rows remain in your query.");
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Exception in SelectOneRecordLackingPhone: " + ex.ToString());
                    Application.Exit();
                }
            }

            // having init globals, transfer to Gui:

            string pretty = name + " (" + age + ") (" + street + ") " + locn;
            theAppForm.setTextBoxNameLocation(pretty);

            // The "?" marks phones that COULD NOT BE FOUND.
            // But "*" marks phones that are yet unsearched.

            // theAppForm.setTextBoxPhones("?"); // what to write back if none found
            theAppForm.setTextBoxPhones(phone); // initially, "*"
        }

        static int nCompleted0 = 0;
        static int nCompleted1 = 0;
        static int nCompleted2 = 0;


        // I cannot let just any garbage enter the database"
        // Allow "?" meaning no phones phone. Otherwise only:
        // Allow \d\d\d-\d\d\d-\d\d\d\d
        // Allow \d\d\d-\d\d\d-\d\d\d\d \d\d\d-\d\d\d-\d\d\d\d

        static readonly Regex reFoundOkay = new Regex(@"^(\?|\d\d\d-\d\d\d-\d\d\d\d|\d\d\d-\d\d\d-\d\d\d\d \d\d\d-\d\d\d-\d\d\d\d)$", RegexOptions.Compiled);

        public static void FoundNowUpdatePhones(string foundPhones)
        {
            // This second command will update (hopefully) a single record:

            // I cannot let just any garbage enter the database"
            if (! reFoundOkay.IsMatch(foundPhones))
            {
                MessageBox.Show("Invalid foundPhones [" + foundPhones + "]");
                return;
            }

            switch(foundPhones.Length)
            {
                case 1:
                    nCompleted0++;
                    break;
                case 12:
                    nCompleted1++;
                    break;
                case 25:
                    nCompleted2++;
                    break;
            }

            // tailor this to update either Comal or Hays table
            string sqlUpdatePhoneAtNameStreet = "";

            // this switch is only tailored for the table name (comal vs hays)
            switch (tailoredFor)
            {
                case 1:
                    sqlUpdatePhoneAtNameStreet =
                        "UPDATE mvv.comal SET phone = \"" + foundPhones + "\" WHERE name = \"" + name + "\" AND street = \"" + street + "\"";
                    break;
                case 2:
                    sqlUpdatePhoneAtNameStreet =
                        "UPDATE mvv.hays SET phone = \"" + foundPhones + "\" WHERE name = \"" + name + "\" AND street = \"" + street + "\"";
                    break;
            }

            using (MySqlCommand updateCmd = new MySqlCommand(sqlUpdatePhoneAtNameStreet, sqlConnDoIt))
            {
                try
                {
                    int nAffected = updateCmd.ExecuteNonQuery();
                    if (nAffected != 1)
                    {
                        MessageBox.Show("Rows affected not 1, but " + nAffected);
                    }
                    else
                    {
                        // Put the post confirmation prcess inside here:

                        int dt = (int)(DateTime.Now - atop).TotalMinutes;
                        theAppForm.Text = "Congratulations, You completed " + nCompleted2 + " doubles, " + nCompleted1 + " singles, " + nCompleted0 + " unfound in " + dt + " minutes.";

                        // Insert a further delay here before ack-on-saved to simulate a human being.
                        Thread.Sleep(random.Next(10000) + 5000);

                        // send a confirmation, to close the CS driven loop:
                        StdioJsonHandler.SendJsonCommand("phonesGotSaved", "okay");
                    }

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Exception in UpdatePhoneInDatabase: " + ex.ToString());
                    Application.Exit();
                }
            }


            /*
             * henceforth, CS will drive me, not C#+User driving cS....
             * 
            Thread.Sleep(3000); // show old phone numbers before updating

            // start another work cycle:
            // henceforth, only when the CS asks me to: SelectOneRecordLackingPhone();


            Thread.Sleep(3000 + random.Next(2000)); // allow time for the WP popup to be clicked. + some pseudo-human randomness

            // save me the next obvious click:

            // theAppForm.clickOnBlueButton();

            // 1. I cannot click the BLUE button.
            // 2. I cannot call the event handler, BLUE_Click
            // so just do what it does:

            // now for real:
            // these are local to me:
            // string name = "";
            // string locn = "";
            // MySqlHandler.WhatNameLocn(out name, out locn);
            StdioJsonHandler.SendJsonCommand("blue", name + "/" + locn);

            *
            */

        }


    }


}
