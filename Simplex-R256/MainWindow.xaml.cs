using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace SimplexNamespace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string input;
        public string[] instructions;
        public bool Bf1 = false;
        public bool Bf2 = false;
        public bool Bf3 = false;
        public bool Bf4 = false;
        public bool stopped = false;
       
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".prog"; // Default file extension
            ofd.Filter = "Text documents (.prog)|*.prog"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                instructions = Regex.Split(input, @"\s+");       // split into array of words
                for(int pc = 0; pc <= 63; pc++)
                {
                    parseInstructions(instructions, pc);
                    
                }
               

            }

        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            stopped = false;
            canvas.Children.Clear();
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".prog"; // Default file extension
            ofd.Filter = "Text documents (.prog)|*.prog"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                instructions = Regex.Split(input, @"\s+");       // split into array of words
                for (int pc = 0; pc <= 63; pc++)
                {
                    parseInstructions(instructions, pc);

                }


            }
        }

        private void stop()
        {
            Console.WriteLine("Program stopped.");
            while (true) { 
                // do nothing lol
            }
        }

        private void memoryDump(object sender, RoutedEventArgs e) { 
        for(int i = 0; i < instructions.Length; i++)
            {
                Console.WriteLine(i + ": " + instructions[i]);
            }
        }

        private void parseInstructions(string[] words, int pc)
        {
            //Console.WriteLine(words.Length);
            if (words.Length != 128)
            {
                Console.WriteLine("Error #02: Invalid program length. Must be 128 lines.");
                stop();
                
            }
            
            {
                //Console.WriteLine(words[pc]);
                if (words[pc].Length != 16) {
                    Console.WriteLine("Error #01: Invalid instruction length. Must be 16 bits long.");
                    stop();
                }

                //add
                if(words[pc].Substring(0,3) == "001")
                {
                    int addr = Convert.ToInt32(words[pc].Substring(3,7), 2);
                    //Console.WriteLine(addr);
                    int pAddr = Convert.ToInt32(words[pc].Substring(10, 6), 2);
                    //Console.WriteLine(pAddr);

                    int valueAtAddr = Convert.ToInt32(words[addr], 2);
                    //Console.WriteLine(words[addr]);
                    int valueAtpAddr = Convert.ToInt32(words[pAddr], 2);
                   // Console.WriteLine(words[64+pAddr]);

                    int result = valueAtpAddr + valueAtAddr;
                        
                    string binaryResult = Convert.ToString(result, 2);

                    if(binaryResult.Length > 16 || result > 65536)
                    {
                        Console.WriteLine("Error #03: Integer overflow. Numbers are constrained to 16 bits.");
                        stop();
                    } 

                    while(binaryResult.Length != 16)
                    {
                      binaryResult = "0" + binaryResult;
                    }

                    words[pAddr] = binaryResult;


                }
                // Sub
                else if (words[pc].Substring(0, 3) == "010")
                {

                    int addr = Convert.ToInt32(words[pc].Substring(3, 7), 2);
                    //Console.WriteLine(addr);
                    int pAddr = Convert.ToInt32(words[pc].Substring(10, 6), 2);
                    //Console.WriteLine(pAddr);

                    int valueAtAddr = Convert.ToInt32(words[addr], 2);
                    //Console.WriteLine(words[addr]);
                    int valueAtpAddr = Convert.ToInt32(words[pAddr], 2);
                    // Console.WriteLine(words[64+pAddr]);

                    int result = valueAtpAddr - valueAtAddr;

                    string binaryResult = Convert.ToString(result, 2);

                    if (binaryResult.Length > 16 || result > 65536 || result < 0)
                    {
                        Console.WriteLine("Error #03: Integer overflow. Numbers are constrained to 16 bits.");
                        stop();
                    }

                    while (binaryResult.Length != 16)
                    {
                        binaryResult = "0" + binaryResult;
                    }

                    words[pAddr] = binaryResult;



                }
                
                // Copy
                else if (words[pc].Substring(0,3) == "100")
                {
                    int addr = Convert.ToInt32(words[pc].Substring(3, 7), 2);
                    //Console.WriteLine(addr);
                    int pAddr = Convert.ToInt32(words[pc].Substring(10, 6), 2);
                    //Console.WriteLine(pAddr);

                    words[addr] = words[pAddr];

                }
                // Copy word
                else if (words[pc].Substring(0, 3) == "101")
                {
                    int addr = Convert.ToInt32(words[pc].Substring(3, 7), 2);
                    //Console.WriteLine(addr);
                    int pAddr = Convert.ToInt32(words[pc].Substring(10, 6), 2);
                    //Console.WriteLine(pAddr);

                    words[addr] = words[pAddr];
                    words[addr+1] = words[pAddr+1];

                    if(addr > 127)
                    {
                        Console.WriteLine("Error #05: Out of bounds. Address accessed was not between 0-127.");
                    }


                }
                // Jump Next if Less
                else if (words[pc].Substring(0, 3) == "110")
                {
                    int addr = Convert.ToInt32(words[pc].Substring(3, 7), 2);
                    //Console.WriteLine(addr);
                    int pAddr = Convert.ToInt32(words[pc].Substring(10, 6), 2);
                    //Console.WriteLine(pAddr);

                    int valueAtAddr = Convert.ToInt32(words[addr], 2);
                    //Console.WriteLine(words[addr]);
                    int valueAtpAddr = Convert.ToInt32(words[pAddr], 2);
                    // Console.WriteLine(words[64+pAddr]);

                    if(valueAtAddr > valueAtpAddr)
                    {
                        pc += 1;
                        if (pc >= 63)
                        {
                            Console.WriteLine("Error #06: Out of bounds of PROM");
                            stop();
                        }
                    }
                }


                // Jump Next if Equal
                else if (words[pc].Substring(0, 3) == "111")
                {
                    int addr = Convert.ToInt32(words[pc].Substring(3, 7), 2);
                    //Console.WriteLine(addr);
                    int pAddr = Convert.ToInt32(words[pc].Substring(10, 6), 2);
                    //Console.WriteLine(pAddr);

                    int valueAtAddr = Convert.ToInt32(words[addr], 2);
                    //Console.WriteLine(words[addr]);
                    int valueAtpAddr = Convert.ToInt32(words[pAddr], 2);
                    // Console.WriteLine(words[64+pAddr]);
                    if (valueAtAddr == valueAtpAddr)
                    {
                        pc += 1;
                        if (pc >= 63)
                        {
                            Console.WriteLine("Error #06: Out of bounds of PROM");
                            stop();
                        }
                    }
                }

                // goto
                else if (words[pc].Substring(0, 3) == "011")
                {
                    int pAddr = Convert.ToInt32(words[pc].Substring(3, 6), 2);
                    pc = pAddr - 1;
                }
                

                // post-instruction processing
                
                if(pc >= 63)
                {
                    pc = 0;
                }

                // set program counter

                string pcBinary = Convert.ToString(pc, 2);

                while (pcBinary.Length != 16)
                {
                    pcBinary = "0" + pcBinary;
                }
                words[119] = pcBinary;

                // check if 'stop' button is pressed
                if (stopped == true) {
                    pc = 63;
                }

                // update buttons
                if (Bf1 == true) {
                    instructions[118] = "0000000000000001";
                } else
                {
                    instructions[118] = "0000000000000000";
                }

                if (Bf2 == true)
                {
                    instructions[117] = "0000000000000001";
                }
                else
                {
                    instructions[117] = "0000000000000000";
                }
                if (Bf3 == true)
                {
                    instructions[116] = "0000000000000001";
                }
                else
                {
                    instructions[116] = "0000000000000000";
                }
                if (Bf4 == true)
                {
                    instructions[115] = "0000000000000001";
                }
                else
                {
                    instructions[115] = "0000000000000000";
                }

                // update display

                // seg a
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[120].Substring(k, 1);
                    //Console.WriteLine("BIT: " + bit);
                    if (bit == "1")
                    {
                        drawPixel(133, 102 + (k * 10));
                    }
                }

                // seg b
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[120].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(143, 102 + ((k-8) * 10));
                    }
                }

                // seg c
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[121].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(193, 102 + (k * 10));
                    }
                }

                // seg d
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[121].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(203, 102 + ((k-8) * 10));
                    }
                }

                // seg e
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[122].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(153, 102 + (k * 10));
                    }
                }

                // seg f
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[122].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(163, 102 + ((k-8) * 10));
                    }
                }

                // seg g
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[123].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(173, 102 + (k * 10));
                    }
                }

                // seg h
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[123].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(183, 102 + ((k-8) * 10));
                    }
                }

                // seg i
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[124].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(213, 102 + (k * 10));
                    }
                }

                // seg j
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[124].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(223, 102 + ((k - 8) * 10));
                    }
                }

                // seg k
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[125].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(233, 102 + (k * 10));
                    }
                }

                // seg l
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[125].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(243, 102 + ((k-8) * 10));
                    }
                }

                // seg m
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[126].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(253, 102 + (k * 10));
                    }
                }

                // seg n
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[126].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(263, 102 + ((k - 8) * 10));
                    }
                }

                // seg o
                for (int k = 0; k < 8; k++)
                {
                    string bit = words[127].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(273, 102 + (k * 10));
                    }
                }

                // seg p
                for (int k = 8; k < 16; k++)
                {
                    string bit = words[127].Substring(k, 1);
                    if (bit == "1")
                    {
                        drawPixel(283, 102 + ((k-8) * 10));
                    }
                }




            }
        }

        

        private void drawPixel(int x, int y) {
            line(x, y, x + 10, y, 48, 48, 48, 10, canvas);
        }

        private void line(int x1, int y1, int x2, int y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }

        private void clear_click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }
        private void F1_Click(object sender, RoutedEventArgs e)
        {
            Bf1 = !Bf1;
        }
        private void F2_Click(object sender, RoutedEventArgs e)
        {
            Bf2 = !Bf2;
        }
        private void F3_Click(object sender, RoutedEventArgs e)
        {
            Bf3 = !Bf3;
        }
        private void F4_Click(object sender, RoutedEventArgs e)
        {
            Bf4 = !Bf4;
        }

    }

}
