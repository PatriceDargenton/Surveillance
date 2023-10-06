
using System;
using System.Diagnostics; // Process
using System.Windows.Forms; // DataObject, ...
using System.Security.Cryptography; // SHA256
using System.Text; // Encoding
using System.IO; // Path
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Surveillance
{
    public static class Util
    {
        public static readonly string crLf = Environment.NewLine;
        const string possibleWriteErrCause =
            "The file may be write-protected or locked by another software";

        public static void CopyClipBoard(string sInfo)
        {

            // Copy informations to Windows clipboard
            // (they will remain until the application is closed)

            try
            {
                DataObject dataObj = new DataObject();
                dataObj.SetData(DataFormats.Text, sInfo);
                Clipboard.SetDataObject(dataObj);
            }
            catch (Exception ex)
            {
                // Clipboard may be unavailable
                MessageBox.Show("Can't copy to the Clipboard:\n" + ex.Message, 
                    Const.appTitle + " : CopyClipBoard",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void StartProcess(string path, string arguments = "")
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("Can't find the application:\n" + path, 
                    Const.appTitle + " : StartProcess",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var p = new Process
            {
                StartInfo = new ProcessStartInfo(path)
            };
            p.StartInfo.Arguments = arguments;
            p.Start();
        }
        
        public static void ShowDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show("Can't find the directory:\n" + path,
                    Const.appTitle + " : ShowDirectory",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var p = new Process
            {
                StartInfo = new ProcessStartInfo(path)
            };
            p.Start();
        }

        /// <summary>
        /// Read file and return its content
        /// </summary>
        public static string[] ReadFile(string filePath, out string msgError,
            bool defaultEncoding = true, Encoding encode = null)
        {
            msgError = "";
            try
            {
                if (defaultEncoding) encode = Encoding.Default;
                return System.IO.File.ReadAllLines(filePath, encode);
            }
            catch (Exception ex)
            {
                msgError = "Can't read file: " + Path.GetFileName(filePath) + crLf +
                    filePath + crLf + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Write file from string
        /// </summary>
        public static bool WriteFile(string filePath, string content, out string msgError,
            bool append = false, bool bDefautEncoding = true, Encoding encode = null)
        {
            msgError = "";
            try
            {
                if (bDefautEncoding) encode = Encoding.Default;
                using (StreamWriter sw = new StreamWriter(filePath, append, encode))
                { sw.Write(content); }
                return true;
            }
            catch (Exception ex)
            {
                msgError = "Can't write file: " + Path.GetFileName(filePath) + crLf +
                    filePath + crLf + ex.Message + crLf + possibleWriteErrCause;
                return false;
            }
        }

        public static DialogResult ShowInputDialog(string title, ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(350, 100);
            var inputBox = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = Const.appTitle
            };

            var lbl = new Label
            {
                Size = new System.Drawing.Size(size.Width - 10, 30),
                Location = new System.Drawing.Point(5, 5),
                Text = title
            };
            inputBox.Controls.Add(lbl);

            var textBox = new TextBox
            {
                Size = new System.Drawing.Size(100, 23),
                Location = new System.Drawing.Point(5, 5 + 30),
                Text = input,
                PasswordChar = '*'
            };
            //inputBox.Controls.Add(textBox);

            var okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&OK",
                Location = new System.Drawing.Point(size.Width - 80 - 80, 39 + 30),
                Enabled = false // Don't accept a blank text
            };
            inputBox.Controls.Add(okButton);

            textBox.TextChanged += (sender, e) =>
            {
                if (textBox.Text.Length > 0) okButton.Enabled = true;
                else okButton.Enabled = false; // Don't accept a blank text
            };
            inputBox.Controls.Add(textBox);

            var cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&Cancel",
                Location = new System.Drawing.Point(size.Width - 80, 39 + 30)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }
    }

    public static class SymmetricEncryptionHelper
    {
        // Symmetric encryption key
        private static byte[] EncryptionKey = 
            Encoding.UTF8.GetBytes(Const.appSymmetricEncryptionKey);
        private const int minKeySizeBytes = 24;

        public static string Encrypt(string plainText)
        {
            byte[] encryptedBytes;

            try 
            {
                using (Aes aes = Aes.Create())
                {
                    // AES symmetric encryption key must be greater than or equal to 24 bytes
                    // (Below 24 bytes, not all lengths are allowed)
                    // Specified key is not a valid size for this algorithm
                    // https://github.com/dotnet/runtime/issues/21848

                    // HACK: Fill out key to 24 bytes required
                    if (EncryptionKey.Length < minKeySizeBytes)
                    {
                        List<byte> byteList = new List<byte>(EncryptionKey);
                        for (int i = EncryptionKey.Length; i < minKeySizeBytes; i++) byteList.Add(0);
                        EncryptionKey = byteList.ToArray();
                    }
                    aes.Key = EncryptionKey;

                    aes.GenerateIV();

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);

                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                            cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                        }

                        encryptedBytes = ms.ToArray();
                    }
                }
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Encrypt " + plainText + ": " + ex.Message);
                return "";
            }
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

                using (Aes aes = Aes.Create())
                {
                    // AES symmetric encryption key must be greater than or equal to 24 bytes
                    // (Below 24 bytes, not all lengths are allowed)
                    // Specified key is not a valid size for this algorithm
                    // https://github.com/dotnet/runtime/issues/21848

                    // HACK: Fill out key to 24 bytes required
                    if (EncryptionKey.Length < minKeySizeBytes)
                    {
                        List<byte> byteList = new List<byte>(EncryptionKey);
                        for (int i = EncryptionKey.Length; i < minKeySizeBytes; i++) byteList.Add(0);
                        EncryptionKey = byteList.ToArray();
                    }
                    aes.Key = EncryptionKey;

                    byte[] iv = new byte[aes.IV.Length];
                    Buffer.BlockCopy(encryptedBytes, 0, iv, 0, aes.IV.Length);

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedBytes, aes.IV.Length, encryptedBytes.Length - aes.IV.Length);
                        }

                        byte[] decryptedBytes = ms.ToArray();
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception) 
            {
                return ""; // Decryption failed
            }
        }
    }
}
