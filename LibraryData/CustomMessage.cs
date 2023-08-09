using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace LibraryData
{
    public class CustomMessage
    {
        #region Variables

        readonly List<byte> data = new();

        #endregion

        #region Constructor

        public CustomMessage(byte[] newdata,int size)
        {
            byte[] header = Encoding.Default.GetBytes(size.ToString().PadLeft(5,'0'));
            data.AddRange(header);
            data.AddRange(newdata);
        }

        public CustomMessage(byte[] receivedmessage)
        {
            data.AddRange(receivedmessage);
        }

        #endregion

        #region Getter

        public int GetSize()
        {
            return Convert.ToInt32(Encoding.Default.GetString(data.GetRange(0, 5).ToArray()))+5;
        }

        public byte[] GetData() { return data.ToArray(); }

        public List<byte> GetContent()
        {
            List<byte> content = data;
            content.RemoveRange(0, 5);
            return content;
        }

        #endregion
    }

    public class CustomMessageSender
    {
        #region Variables

        readonly List<CustomMessage> messages = new();

        #endregion

        #region Constructor

        public CustomMessageSender(byte[] message)
        {
            for (int i = 0; i < message.Length/65000+1; i++)
            {
                byte[] single = new byte[65000];
                int size = 65000;
                try {
                    Array.Copy(message, i * 65000, single, 0, 65000);
                }
                catch
                {
                    Array.Copy(message, i * 65000, single, 0, message.Length % 65000);
                    Array.Resize(ref single, message.Length % 65000);
                    size = message.Length % 65000;
                }
                messages.Add(new CustomMessage(single,size));
            }
        }

        #endregion

        #region Getter

        public List<CustomMessage> GetMessages(){ return messages; }

        #endregion
    }

    public class CustomMessageReceiver
    {
        #region Variables

        readonly List<byte> remainder = new();
        readonly List<CustomMessage> messages= new();
        readonly Socket socket;

        #endregion

        #region Constructor

        public CustomMessageReceiver(Socket socket)
        {
            this.socket = socket;
        }

        #endregion

        #region Receive Data

        public void Receive()
        {
            int messageSize;
            do
            {
                byte[] buf = new byte[66000];
                int ReceivedSize = socket.Receive(buf);
                Array.Resize(ref buf, ReceivedSize);
                List<byte> current = new();
                current.AddRange(remainder);
                current.AddRange(buf);
                messageSize = Convert.ToInt32(Encoding.Default.GetString(current.ToList().GetRange(0, 5).ToArray())) + 5;
                if (ReceivedSize >= messageSize)
                {
                    messages.Add(new CustomMessage(current.ToList().GetRange(0, messageSize).ToArray()));
                    remainder.Clear();
                    remainder.AddRange(current.ToList().GetRange(messageSize, ReceivedSize - messageSize));
                }
            } while (messageSize == 65005);
        }

        #endregion

        #region Getter

        public byte[] GetImageBytes()
        {
            List<byte> imageBytes = new();
            for (int i = 0; i < messages.Count; i++)
            {
                imageBytes.AddRange(messages[i].GetContent());
            }
            return imageBytes.ToArray();
        }

        #endregion
    }
}
