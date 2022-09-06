using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * We are almost there, closing the loop in input and scrape tasks:
 * 
wpsequencer-background.js entered wpsequencer-background.js:18:9
wpsequencer-background.js exited wpsequencer-background.js:116:9
The Web Console logging API (console.log, console.info, console.warn, console.error) has been disabled by a script on this page.
BS heard your Click on addon icon wpsequencer-background.js:54:10
CS has executed in active tab: wpsequencer-background.js:47:10
BS received from CS: cmd=(greeting), param=(hello from content script) wpsequencer-background.js:104:10
BS received from CS: cmd=(greeting), param=(hi there content script!) wpsequencer-background.js:104:10
BS received from C#: cmd=(red), param=(JOSEPH MATTHEW BOZE JR/WOODCREEK TX 78676) wpsequencer-background.js:80:10
BS received from CS: cmd=(hey), param=(You clicked the webpage) wpsequencer-background.js:104:10
BS received from CS: cmd=(red), param=(JOSEPH MATTHEW BOZE JR/WOODCREEK TX 78676) wpsequencer-background.js:104:10
BS received from CS: cmd=(hey), param=(You clicked the webpage) 3 wpsequencer-background.js:104:10
BS received from C#: cmd=(green), param=(was clicked) wpsequencer-background.js:80:10
BS received from CS: cmd=(phones), param=(512-847-3517 281-334-9128) wpsequencer-background.js:104:10
BS received from CS: cmd=(green), param=(was clicked) wpsequencer-background.js:104:10
*/


namespace wpsequencer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BLUE_Click(object sender, EventArgs e)
        {
            // send something
            // StdioJsonHandler.SendJsonCommand("blue", "was clicked");

            // now for real:
            string name = "";
            string locn = "";
            MySqlHandler.WhatNameLocn(out name, out locn);
            StdioJsonHandler.SendJsonCommand("blue", name + "/" + locn);

            /*
             * Here's what the CS javascript will do with my param:
             * 	if(m.cmd == "red")
	            {
		            document.body.style.border = "15px solid red"
		            var nameLocn = m.param.split('/');
		            PasteTwoTextsAndSubmit(nameLocn[0], nameLocn[1]);
	            }
             */
        }

        private void GOLD_Click(object sender, EventArgs e)
        {
            // send something
            StdioJsonHandler.SendJsonCommand("gold", "to send back phone numbers");
        }

        // TX & RX threads must .Invoke to do work here in the app main thread:

        public void setTextBoxTxCmd(string s)
        {
            // Running on the TX thread
            TextBoxTxCmd.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                TextBoxTxCmd.Text = s;
            });
            // Returning to the TX thread
        }

        public void setTextBoxTxParam(string s)
        {
            // Running on the TX thread
            TextBoxTxParam.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                TextBoxTxParam.Text = s;
            });
            // Returning to the TX thread
        }

        public void setTextBoxRxCmd(string s)
        {
            // Running on the RX thread
            TextBoxRxCmd.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                TextBoxRxCmd.Text = s;
            });
            // Returning to the RX thread
        }

        public void setTextBoxRxParam(string s)
        {
            // Running on the RX thread
            TextBoxRxParam.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                TextBoxRxParam.Text = s;
            });
            // Returning to the RX thread
        }


        // Those 4 are good for Stdio Work.
        // Now do some more for MySql work:

        public void setTextBoxNameLocation(string s)
        {
            // Running on the other thread
            TextBoxNameLocation.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                TextBoxNameLocation.Text = s;
            });
            // Returning to the other thread
        }

        public void setTextBoxPhones(string s)
        {
            // Running on the other thread
            TextBoxPhones.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                TextBoxPhones.Text = s;
            });
            // Returning to the other thread
        }

        // After the GOLD button process finishes,
        // BLUE button always happens next.
        // So automate that (from MySqlH*...)
        private void Form1_Shown(object sender, EventArgs e)
        {
            MySqlHandler.eventFormShown();
        }

        private void none_Click(object sender, EventArgs e)
        {
            // The "?" marks phones that COULD NOT BE FOUND.
            // But "*" marks phones that are yet unsearched.
            TextBoxPhones.Text = "?";
            MySqlHandler.FoundNowUpdatePhones("?");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
