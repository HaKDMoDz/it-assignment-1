using System;
using System.Drawing;
using System.Globalization;
using System.Speech.Recognition;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public SpeechRecognitionEngine speechrecognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));

        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            // Instantiate a Choices object and fill it with directions
            Choices directions = new Choices();
            directions.Add(new string[] { "left", "right", "top", "bottom", "stop" });

            // Instantiate a GrammarBuilder object and fill it with the Choices object
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(directions);

            // Instantiate a Grammar object and load the Grammar into the speech engine
            Grammar g = new Grammar(gb);
            speechrecognizer.LoadGrammar(g);

            // Set the input to the default audio input device
            speechrecognizer.SetInputToDefaultAudioDevice();

            // Send recognized speech to the EvenHandler

            speechrecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(rSpeechRecognized);

            // Default button 2 to be disabled and allow cancellation of the background worker
            button2.Enabled = false;
            backgroundWorker1.WorkerSupportsCancellation = true;
            label1.TextAlign = ContentAlignment.BottomLeft;
            label1.Text = "Press the Start button to begin";
            
            label2.Text = "";
        }

        public void rSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "left":
                    MoveMouse(-1, 0);
                    break;
                case "right":
                    MoveMouse(+1, 0);
                    break;
                case "top":
                    MoveMouse(0, -1);
                    break;
                case "bottom":
                    MoveMouse(0, +1);
                    break;
                case "stop":
                    backgroundWorker1.CancelAsync();
                    break;
            }
        }

        public void MoveMouse(int x, int y)
        {
            CoordinatePackage input = new CoordinatePackage { x = x, y = y };
            if(!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync(input);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Start the Speech Recognizer' async task
            speechrecognizer.RecognizeAsync(RecognizeMode.Multiple);

            button1.Enabled = false;
            button2.Enabled = true;

            label1.TextAlign = ContentAlignment.TopLeft;
            label1.Text = "Use the voice commands \"bottom\", \"top\", \"left\" and \"right\" to move the mouse around.";

            label2.Text = "Say \"stop\" to stop the movement";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Stop the Speech Recognizer' async task
            speechrecognizer.RecognizeAsyncStop();
            backgroundWorker1.CancelAsync();

            button1.Enabled = true;
            button2.Enabled = false;

            label1.TextAlign = ContentAlignment.BottomLeft;
            label1.Text = "Press the Start button to begin";

            label2.Text = "";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            CoordinatePackage value_received = e.Argument as CoordinatePackage;
            for (int i = 0; i < 2500; i++)
            {
                Cursor.Position = new Point(Cursor.Position.X + value_received.x, Cursor.Position.Y + value_received.y);
                Thread.Sleep(8);
                if (backgroundWorker1.CancellationPending)
                {
                    return;
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

    }

    public class CoordinatePackage
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}