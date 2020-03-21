using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoProjekt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SMSsender.SMSsenderServer SMSsenderServer = new SMSsender.SMSsenderServer(81);//Deklaracija in inicializacija SMSsender strežnika na portu 81


        private void button1_Click(object sender, EventArgs e)
        {
            if (SMSsenderServer.Is_On == false)
            {
                try
                {
                    SMSsenderServer.RecievedSMS += SMSsenderServer_RecievedSMS; //Naredi event, ki se zgodi, ko se v bazi doda novo sporočilo
                    SMSsenderServer.Commands = new string[] { "verzija", "datum", "čćžđš" }; //Nastavi spremenljivko commandi na vse ukaze, ki jih uporabljate v programu. Prosim, nastavite spremenljivko na VSE ukaze, ki jih uporabljate.
                    SMSsenderServer.StartInThread(); //Začene server v drugem threadu zato, da je Form še vedno odziven

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
               
            }
            else
            {
                MessageBox.Show("Strežnik že deluje.");
            }
        }

        private void SMSsenderServer_RecievedSMS(object sender, EventArgs e)
        {
            //Metoda se izvede, ko je v bazi dodano sporočilo
            List<SMSsender.SMS> sMs = SMSsender.SMSsender.ReturnReceivedSMS("testUser"); //Metoda vrne vsa nova sporočila, ki še niso bila obdelana. //Če ni novih sporočil, vrne NULL!!

            if (sMs != null) //Preveri, če je vsaj en SMS v Listu
            {
                foreach (var item in sMs) //Zanka se izvede tolikokrat, kolikor je sporočil, ki še niso bila obdelana.
                {
                    switch (item.Message.Split(' ')[0])
                    {
                        case "verzija": SMSsender.SMSsender.SendSMS("Verzija Demo projekta je 1.0", item.Phone, "testUser"); break; //Metoda pošlje sporočilo, na izbrano telefonsko številko preko uporabniškega imena
                        case "datum": SMSsender.SMSsender.SendSMS("Današnji datum: " + DateTime.Now, item.Phone, "testUser"); break; //Metoda pošlje sporočilo, na izbrano telefonsko številko preko uporabniškega imena
                        default: SMSsender.SMSsender.SendSMS(item.Message, item.Phone, "testUser"); break; //Metoda pošlje sporočilo, na izbrano telefonsko številko preko uporabniškega imena
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SMSsender.SMSsender.ReturnNumberOfSMStoday("testUser")); //Vrne število SMS, ki se jih še lahko pošlje danes
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SMSsender.SMSsender.ReturnLengthOfSMS("Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234Lorem ipsum 1234", "testUser"));
            //Vrne koliko SMS poročil, zasede message
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SMSsender.SMSsender.SendRequestToAPI("ihfiuwehiufhriuefe", "", "testUser", new SMSsender.Settings(false, true, true, false)));
            //Vrne raw API result
        }
    }
}
