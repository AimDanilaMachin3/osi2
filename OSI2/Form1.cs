using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSI2
{
    public partial class Form1 : Form
    {
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        //
        //апи функция для просмотра памяти
        [DllImport("kernel32.dll")]
        //для выделения памяти
        public static extern IntPtr GlobalAlloc(int con, int size);
        [DllImport("kernel32.dll")]
        //для освобождения
        public static extern int GlobalFree(IntPtr start);
        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);
        //
        public struct MEMORYSTATUS
        {
            public UInt32 dwLength;               //Размер структуры, в байтах-хз че это
            public UInt32 dwMemoryLoad;           //процент занятой памяти
            public UInt32 dwTotalPhys;            //общее кол-во физической(оперативной) памяти
            public UInt32 dwAvailPhys;            //свободное кол-во физической(оперативной) памяти
            public UInt32 dwTotalPageFile;        //предел памяти для системы или текущего процесса
            public UInt32 dwAvailPageFile;        //Максимальный объем памяти,который текущий процесс может передать в байтах.
            public UInt32 dwTotalVirtual;         //общее количество виртуальной памяти(файл подкачки)
            public UInt32 dwAvailVirtual;         //свободное количество виртуальной памяти(файл подкачки)
            public UInt32 dwAvailExtendedVirtual; //Зарезервировано. Постоянно 0.
        }
        //
        public Form1()
        {
            InitializeComponent();
            textBox2.Text += "0x";

            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", String.Empty);
            label1.Text = "Используемая память " + ramCounter.NextValue() + "%";



           // MEMORYSTATUS memStatus = new MEMORYSTATUS();
            //label1.Text = "Свободная память файлов подкачки: " + memStatus.dwTotalPageFile.ToString();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Int64 index = Convert.ToInt64(textBox2.Text, 16);
            Process process = Process.GetProcessesByName("notepad")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            int bytesRead = 0;
            byte[] buffer = new byte[24];
            ReadProcessMemory((int)processHandle, index, buffer, buffer.Length, ref bytesRead);
            String result = Encoding.Unicode.GetString(buffer);
            richTextBox1.Text = result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Int64 index = Convert.ToInt64(textBox2.Text, 16);
            Process process = Process.GetProcessesByName("notepad")[0];
            string buf = textBox1.Text;
            IntPtr processHandle = OpenProcess(0x1F0FFF, false, process.Id);
            int bytesWritten = 0;
            byte[] buffer = Encoding.Unicode.GetBytes(buf);
            WriteProcessMemory((int)processHandle, index, buffer, buffer.Length, ref bytesWritten);
        }
    }
}
