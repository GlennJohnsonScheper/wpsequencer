/*
 * wpsequencer . StdioJsonHandler
 * 
 * Always build to DEBUG, as it is path the Mozilla add-on will reference.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Concurrent;

// In V.S. Pkg. Mgr., browse Newtonsoft, install Newtonsoft.Json:
using Newtonsoft.Json;

namespace wpsequencer
{
    // Json deserializer produces a .NET class instance.

    // I will only have one member in each wrapper class,
    // then have a switch on that JSON ascii member name.

    // These classes serve the receive from BS to C# process:

    // here is a first, generic class,
    // useful to scrape a web page DOM:

    public class CaptureWrapper
    {
        // Class member name(s) must match JSON text:
        public string capture { get; set; }
    }


    // This class serves the transmit from C# to BS process:

    // Actually, use it for both directions, nice if echoed.

    // Here is such an example packet w/o the 4 count bytes:
    // {"cmd":"green","param":"was clicked"}

    public class CmdParam
    {
        // Serialized JSON text will be a dict {} with all class member name(s):
        // E.g., thus after 4 count bytes: ---{"cmd":"btn1","param":"clckd"}---.
        public string cmd { get; set; }
        public string param { get; set; }
    }


    // No point having main nor form1 instantiate this.
    // Make it static, and attach tx/rx queues to stdio.

    public static class StdioJsonHandler
    {
        const string topDirectory = @"C:\A\EXE\2021FirefoxAddons\wpsequencer";

        // optionally, save any rx data files here:
        static bool doSaveRawRxBinary = false;
        static bool doSaveRawArtifacts = false;

        // this is that always-good study aid:
        static bool doSaveCaptureArtifacts = true;

        // nicer, newer:
        static bool doSaveRxCmdParamArtifacts = false;
        static bool doSaveTxCmdParamArtifacts = false;

        static string rxDataFilesPath = topDirectory + @"\rxDataFiles";

        // debugging requires a log:
        static string logFilePath = topDirectory + @"\logFile.txt";
        static DateTime atop = DateTime.Now;

        // debugging with stdio tied up is tough!
        static object lockObj = new object();
        static void logLine(string line)
        {
            // Yeah, it seemed like a great idea to change all my artifact logging into this one logfile, huh?
            // System.IO.IOException: The process cannot access the file 'C:\A\EXE\2021FirefoxAddons\wpsequencer\logFile.txt' because it is being used by another process.
            lock (lockObj)
            {
                File.AppendAllText(logFilePath, line + "\r\n");
                // Supposedly, writes to stderr will appear in Mozilla console...?
                // Console.Error.WriteLine(line);
            }
        }

        static Thread SendQueueThread = null;
        static Thread ReceiveQueueThread = null;

        public static bool exiting = false;

        // After all these years, I still haaaate classes!
        private static Form1 theAppForm = null;

        // CTOR - a big mystery:
        // CTOR is not being called but app is running!
        // Aha! Is this a new rule?
        // A "first reference" to static class is required to load it!
        static StdioJsonHandler()
        {
            // Make topDir BEFORE catch might logLine!
            if (Directory.Exists(topDirectory) == false)
                Directory.CreateDirectory(topDirectory);
            try
            {
                string dts = atop.ToString("yyyyMMddTHHmmss.fff");
                logLine("-----------------------------------"); // empirically, a first dividor line atop each run.
                logLine(dts + " StdioJsonHandler ctor.");

                if (Directory.Exists(rxDataFilesPath) == false)
                    Directory.CreateDirectory(rxDataFilesPath);

                SendQueueThread = new Thread(new ThreadStart(SendQueueThreadProc));
                SendQueueThread.Start();

                ReceiveQueueThread = new Thread(new ThreadStart(ReceiveQueueThreadProc));
                ReceiveQueueThread.Start();
            }
            catch(Exception e)
            {
                string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");
                logLine(dts + " Exception in StdioJsonHandler ctor: " + e.ToString());
            }
        }

        // Perhaps a "first reference" to static class is required to load it? YES!
        public static void FirstReferenceToStartMe(Form1 mainForm)
        {
            theAppForm = mainForm;
            string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");
            logLine(dts + " FirstReferenceToStartMe");
        }

        // Although I will probably be the first to know, by -1 rx byte.
        public static void FormSaysAppExiting()
        {
            logLine("Clue is FormSaysAppExiting");
            AppExiting();
        }
        public static void AppExiting()
        {
            // called from form1
            string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");
            logLine(dts + " AppExiting");
            exiting = true;
            txReady.Release(); // this should run out the send thread
            ReceiveQueueThread.Abort();
            SendQueueThread.Abort();
        }


        // ===================================================
        // Section ONE - The Reception from BS to C# processes.
        // Which includes the Json RX class definitions above.
        // ===================================================


        static void ReceiveQueueThreadProc()
        {
            try
            {
                // to look for the first all-lowercase ascii name in rx json string:
                Regex reLcRun = new Regex(@"([a-z]+)", RegexOptions.Compiled);

                Stream si = Console.OpenStandardInput();

                while (!exiting)
                {
                    // loop until reading the first RX count byte fails.
                    for (; ; )
                    {
                        // int z=0, o=1, lazy8=o/z; // tests catch path

                        // Read the 32-bit little endian byte count.
                        int i0 = si.ReadByte(); // this is the lsb

                        if (i0 == -1)
                        {
                            // When I quit attached Firefox,
                            // this is the result in C# app:
                            // System.Exception: i0 == -1
                            // throw new Exception("i0 == -1");
                            //
                            // So rather, this is the time to exit app.
                            logLine("Clue is (i0 == -1), which is normal when FireFox is closed.");
                            AppExiting();
                            return;
                        }

                        int i1 = si.ReadByte();
                        if (i1 == -1)
                            throw new Exception("i1 == -1");
                        int i2 = si.ReadByte();
                        if (i2 == -1)
                            throw new Exception("i2 == -1");
                        int i3 = si.ReadByte();
                        if (i3 == -1)
                            throw new Exception("i3 == -1");

                        int nRx = i0;
                        nRx |= i1 << 8;
                        nRx |= i2 << 16;
                        nRx |= i3 << 24;

                        if (nRx == 0)
                        {
                            // Original Mozilla example had a
                            // zero-rx count rule to exit app.
                            // I don't think this will happen.
                            logLine("Clue is (nRx == 0)");
                            AppExiting();
                            return;
                        }

                        if (nRx <= 0)
                            throw new Exception("nRx <= 0 [" + nRx + "]");
                        // is ten million enough?
                        if (nRx > 10000000)
                            throw new Exception("nRx > 10000000 [" + nRx + "]");

                        // blocking read of the whole JSON packet
                        byte[] rxBa = new byte[nRx];
                        int nRead = si.Read(rxBa, 0, nRx);
                        if (nRead != nRx)
                        {
                            throw new Exception("nRead [" + nRead + "] != nRx [" + nRx + "]");
                        }

                        // Perhaps save each raw JSON string to a file to study:
                        string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");

                        // Nice to have some valid counted data to re-parse.
                        if (doSaveRawRxBinary)
                        {
                            byte[] rxBa4 = new byte[4 + nRx];
                            rxBa4[0] = (byte)i0;
                            rxBa4[1] = (byte)i1;
                            rxBa4[2] = (byte)i2;
                            rxBa4[3] = (byte)i3;
                            Array.Copy(rxBa, 0, rxBa4, 4, nRx);
                            string rawBinaryPath = Path.Combine(rxDataFilesPath, dts + "_stdin.txt");
                            File.WriteAllBytes(rawBinaryPath, rxBa4);
                        }

                        // This rxJson string does not yet have JSON string char escapes removed:
                        string rxJson = Encoding.UTF8.GetString(rxBa);

                        // The JSON object was arranged to have one top string,
                        // which identifies the type of data in rest of object,
                        // with no other top members competing for first place.
                        // E.g., "{\"lirlserp\":[...]}"

                        // I plan to have multiple data types.
                        // Peek at the first string in object:

                        Match m = reLcRun.Match(rxJson); // @"([a-z]+)"

                        if (m.Success)
                        {
                            string pktType = m.Captures[0].Value;

                            // How can this be happening before the form is up?
                            // Have BS and CS not send any fast hello messages.
                            theAppForm.setTextBoxRxCmd(pktType);
                            theAppForm.setTextBoxRxParam(""); // until helper parses it...

                            if (doSaveRawArtifacts)
                            {
                                string rawJsonPath = Path.Combine(rxDataFilesPath, dts + "_" + pktType + "_raw.txt");
                                File.WriteAllText(rawJsonPath, rxJson);
                            }

                            switch (pktType)
                            {
                                case "capture":
                                    ProcessGenericCaptureJson(rxJson, dts);
                                    break;

                                    // no guarantee cmd is first item serialized:
                                case "cmd":
                                case "param":
                                    ProcessCmdParamJson(rxJson, dts);
                                    break;

                                default:
                                    throw new Exception("Error: unknown alpha run in rxJSON packet: " + pktType);
                                    // break;
                            }
                        }
                        else
                        {
                            throw new Exception("Error: no alpha run in rxJSON packet");
                        }



                    }
                }
            }
            catch(Exception e)
            {
                if(!exiting)
                {
                    string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");
                    logLine(dts + " ReceiveQueueThreadProc exception: " + e.ToString());
                    logLine("Clue is ReceiveQueueThreadProc exception");
                    AppExiting();
                }
                return;
            }
        }

        // Receive helpers for different rx packet types...

        static void ProcessGenericCaptureJson(string rxJson, string dts)
        {
            // Might be any Web Page. Body Tags, Attributes, Text...

            // Deserialize one entire C# Class matching all the JSON:
            CaptureWrapper deserializedCaptureWrapper = JsonConvert.DeserializeObject<CaptureWrapper>(rxJson);

            // The 'capture' member string IS the whole thing!

            theAppForm.setTextBoxRxParam(deserializedCaptureWrapper.capture);

            if (doSaveCaptureArtifacts)
            {
                string capturedPath = Path.Combine(rxDataFilesPath, dts + "_capture.txt");
                File.WriteAllText(capturedPath, deserializedCaptureWrapper.capture);
            }
        }


        static void ProcessCmdParamJson(string rxJson, string dts)
        {
            // Deserialize one entire C# Class matching all the JSON:
            CmdParam deserializedCmdParam = JsonConvert.DeserializeObject<CmdParam>(rxJson);

            // The 'capture' message could have been done as cmd/param as well, but is not.
            // Well, in fact, let's revise it now to use this same cmd/param packet:

            theAppForm.setTextBoxRxCmd(deserializedCmdParam.cmd);
            theAppForm.setTextBoxRxParam(deserializedCmdParam.param);

            logLine("RX: " + deserializedCmdParam.cmd + ": " + deserializedCmdParam.param);

            if (doSaveRxCmdParamArtifacts)
            {
                string capturedPath = Path.Combine(rxDataFilesPath, dts + "_rxCmdParam.txt");
                File.WriteAllText(capturedPath, deserializedCmdParam.cmd + ": " + deserializedCmdParam.param);
            }

            if (deserializedCmdParam.cmd == "capture")
            {
                if (doSaveCaptureArtifacts)
                {
                    string capturedPath = Path.Combine(rxDataFilesPath, dts + "_capture.txt");
                    File.WriteAllText(capturedPath, deserializedCmdParam.param);
                }
            }

            // This packet still serves me during the new way, CS driven:

            if (deserializedCmdParam.cmd == "phones")
            {
                theAppForm.setTextBoxPhones(deserializedCmdParam.param);
                MySqlHandler.FoundNowUpdatePhones(deserializedCmdParam.param);
            }

            // New way, as the CS will orchestrate operations statefully:

            /*
	            // After CS sent: myPort.postMessage({cmd:"need", param:"nameLocnAge"});
	            // Process the expected C# native app response of the same name:
	            if(m.cmd == "nameLocnAge")
	            {
		            // Now, I expect 3 segments, with 2 slashes:
		            var nameLocnAge = m.param.split('/');
		            queryName = nameLocnAge[0];
		            queryLocn = nameLocnAge[1];
		            queryAge = nameLocnAge[2];
		            gotQuery = true;
	            }
             */
            if (deserializedCmdParam.cmd == "need")
            {
                MySqlHandler.SelectOneRecordLackingPhone(); // which will init some globals, and show pretty in GUI.
                                                            // now for real:
                string name = "";
                string locn = "";
                string age = "";
                MySqlHandler.WhatNameLocnAge(out name, out locn, out age);
                // make the translation as needed in Whitepages:
                // ... 40s 50s 60s 70s, but then 80+
                char a1 = age[0]; // '0' to '9'
                if (a1 >= '8')
                    age = "80+";
                else
                    age = "" + a1 + "0s";
                StdioJsonHandler.SendJsonCommand("nameLocnAge", name + "/" + locn + "/" + age);
            }
        }


        // ===================================================
        // Section TWO - The Transmit from C# to BS processes.
        // ===================================================


        static ConcurrentQueue<string> txQueue = new ConcurrentQueue<string>(); // thread-safe

        static Semaphore txReady = new Semaphore(0, int.MaxValue);

        public static void SendJsonCommand(string _cmd, string _param)
        {
            if(!exiting)
            {
                CmdParam cmdParam = new CmdParam() { cmd = _cmd, param = _param };
                string serialized = JsonConvert.SerializeObject(cmdParam);
                txQueue.Enqueue(serialized); // which by now includes all needful JSON escaping
                txReady.Release();
            }
        }

        static void SendQueueThreadProc()
        {
            try
            {
                Stream so = Console.OpenStandardOutput();
                while (!exiting)
                {
                    // block until SendJsonCommand indicates something has been queued.
                    txReady.WaitOne();

                    string toSend = "";
                    if (txQueue.TryDequeue(out toSend))
                    {
                        // toSend has JSON escaping, but needs UTF-8 conversion to bytes.
                        byte[] ba = Encoding.UTF8.GetBytes(toSend);
                        int len = ba.Length;
                        UInt32 ulen = (UInt32)len;
                        byte[] ba4 = new byte[len + 4];
                        ba4[0] = (byte)((ulen >> 0) & 0xff);
                        ba4[1] = (byte)((ulen >> 8) & 0xff);
                        ba4[2] = (byte)((ulen >> 16) & 0xff);
                        ba4[3] = (byte)((ulen >> 24) & 0xff);
                        Array.Copy(ba, 0, ba4, 4, len);
                        so.Write(ba4, 0, 4 + len);
                        so.Flush();

                        // it would be easier to let queuer update this,
                        // but I will do it here to prove it was sent:

                        CmdParam cmdParam = JsonConvert.DeserializeObject<CmdParam>(toSend);
                        theAppForm.setTextBoxTxCmd(cmdParam.cmd);
                        theAppForm.setTextBoxTxParam(cmdParam.param);

                        logLine("TX: " + cmdParam.cmd + ": " + cmdParam.param);

                        if (doSaveTxCmdParamArtifacts)
                        {
                            string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");
                            string capturedPath = Path.Combine(rxDataFilesPath, dts + "_TxCmdParam.txt");
                            File.WriteAllText(capturedPath, cmdParam.cmd + ": " + cmdParam.param);
                        }

                    }
                    else
                    {
                        throw new Exception("txQueue.TryDequeue returned false");
                    }
                }
            }
            catch (Exception e)
            {
                if (!exiting)
                {
                    logLine("Clue is Exception in SendQueueThreadProc");
                    string dts = DateTime.Now.ToString("yyyyMMddTHHmmss.fff");
                    logLine(dts + " Exception in SendQueueThreadProc: " + e.ToString());
                    AppExiting();
                }
            }
        }


    }
}
